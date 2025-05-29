using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class ToggleMissileBay : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = SiriusKit.SiriusDeck.Deck,
                rarity = Rarity.common,
                dontOffer = true
            },
            Art = StableSpr.cards_GoatDrone,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "card", "ToggleMissileBay", "name"]).Localize,
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Card), nameof(Card.RenderAction)),
            transpiler: new HarmonyMethod(Card_RenderAction_Transpiler)
        );
    }

    public override CardData GetData(State state)
    {
        return new()
        {
            temporary = true,
            retain = true,
            singleUse = true,
            cost = 0,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AToggleMissileBay(),
            new AStatus() { status = BayPowerDownStatus.BayPowerDownEntry.Status, statusAmount = 1, targetPlayer = true }
        ];
    }

    private static IEnumerable<CodeInstruction> Card_RenderAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
