using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class SiriusMissileBayPatches : IPatchable
{
    [OnPrefix<ASpawn>(nameof(ASpawn.Begin))]
    private static void ASpawn_Begin_Prefix(ASpawn __instance, State s, Combat c)
    {
        global::Ship target = (__instance.fromPlayer ? s.ship : c.otherShip);
        if (target.Get(BayPowerDownStatus.BayPowerDownEntry.Status) > 0)
        {
            ModEntry.Instance.Helper.ModData.SetModData(__instance.thing, "powerdown", true);
            target.Add(BayPowerDownStatus.BayPowerDownEntry.Status, -1);
        }
    }

    [OnPostfix<StuffBase>(nameof(StuffBase.Render))]
    private static void StuffBase_Render_Postfix(StuffBase __instance, G g, Vec v)
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__instance, "powerdown", out bool data))
        {
            if (data)
            {
                var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
                Vec offset = v + __instance.GetOffset(g);
                Draw.Sprite(
                    Sprites.icons_power_down.Sprite,
                    offset.x + 7,
                    offset.y + 16,
                    color: color
                );
            }
        }
    }

    [OnPostfix<StuffBase>(nameof(StuffBase.GetTooltips))]
    private static void StuffBase_GetTooltips_Postfix(
        StuffBase __instance,
        ref List<Tooltip> __result
    )
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__instance, "powerdown", out bool data))
        {
            if (data)
            {
                __result.Add(BayPowerDownStatus.PowerDownTooltip());
            }
        }
    }

    [OnPrefix<AAttack>(nameof(AAttack.Begin))]
    private static void AAttack_Begin_Prefix(AAttack __instance, State s, Combat c)
    {
        if (__instance.fromDroneX != null)
        {
            if (!c.stuff.TryGetValue(__instance.fromDroneX.Value, out StuffBase? midrow))
            {
                return;
            }

            if (ModEntry.Instance.Helper.ModData.TryGetModData(midrow, "powerdown", out bool data))
            {
                if (data)
                {
                    __instance.damage -= 1;
                }
            }
        }
    }

    [OnTranspiler<Card>(nameof(Card.RenderAction))]
    private static IEnumerable<CodeInstruction> Card_RenderAction_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var cursor = new ILCursor(generator, instructions);

        object? unknownType = null;

        cursor.TryGotoNext(
            instr => instr.MatchExtract(OpCodes.Ldloca_S, out unknownType),
            instr => instr.Match(OpCodes.Ldarg_3)
        );

        object? fld = null;

        cursor.TryGotoNext(
            instr => instr.Match(OpCodes.Ldloc_0),
            instr => instr.MatchContainsAndExtract("w", out fld)
        );

        cursor.Index = 0;

        object? action = null;

        cursor.TryGotoNext(
            instr => instr.Match(OpCodes.Ldloc_0),
            instr => instr.MatchContainsAndExtract("action", out action)
        );

        cursor.TryGotoNext(
            MoveType.After,
            instr => instr.Match(OpCodes.Isinst),
            instr => instr.Match(OpCodes.Stloc_3)
        );

        cursor.Emit(OpCodes.Ldloca, unknownType);
        cursor.Emit(OpCodes.Ldfld, action);
        cursor.Emit(OpCodes.Ldloca, unknownType);
        cursor.Emit(OpCodes.Ldflda, fld);
        cursor.EmitDelegate(RenderAction_Lambda);

        return cursor.Generate();
    }

    private static void RenderAction_Lambda(CardAction action, ref int e)
    {
        if (action is AToggleMissileBay)
        {
            e -= 9;
        }
    }
}
