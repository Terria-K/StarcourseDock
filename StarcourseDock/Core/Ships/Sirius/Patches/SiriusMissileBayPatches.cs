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
        return new ILCursor(generator, instructions)
            .GotoNext([ILMatch.LdlocaS(), ILMatch.Ldarg(3)])
            .ExtractOperand(0, out object? unknownType)
            .GotoNext([ILMatch.Ldloc(0), ILMatch.Ldfld("w")])
            .ExtractOperand(1, out object? fld)
            .Reset()
            .GotoNext([ILMatch.Ldloc(0), ILMatch.Ldfld("action")])
            .ExtractOperand(1, out object? action)
            .GotoNext(MoveType.After, [ILMatch.Isinst(), ILMatch.Stloc(3)])
            .Emits(
                [
                    new CodeInstruction(OpCodes.Ldloca, unknownType),
                    new CodeInstruction(OpCodes.Ldfld, action),
                    new CodeInstruction(OpCodes.Ldloca, unknownType),
                    new CodeInstruction(OpCodes.Ldflda, fld),
                ]
            )
            .EmitDelegate(RenderAction_Lambda)
            .Generate();
    }

    private static void RenderAction_Lambda(CardAction action, ref int e)
    {
        if (action is AToggleMissileBay)
        {
            e -= 9;
        }
    }
}
