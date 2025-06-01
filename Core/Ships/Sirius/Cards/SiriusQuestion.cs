using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal class SiriusQuestion : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(
            MethodBase.GetCurrentMethod()!.DeclaringType!.Name,
            new()
            {
                CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    deck = SiriusKit.SiriusDeck.Deck,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true,
                },
                Art = StableSpr.cards_GoatDrone,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "card", "SiriusQuestion", "name"]
                    )
                    .Localize,
            }
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            postfix: new HarmonyMethod(AAttack_Begin_Postfix),
            transpiler: new HarmonyMethod(AAttack_Begin_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
            new HarmonyMethod(AAttack_GetTooltips_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.DoWeHaveCannonsThough)),
            new HarmonyMethod(AAttack_DoWeHaveCannonsThough_Prefix)
        );
    }

    private static void AAttack_GetTooltips_Prefix(State s)
    {
        Combat? c = s.route as Combat;
        if (c == null)
        {
            return;
        }

        foreach (StuffBase thing in c.stuff.Values)
        {
            if (thing is not SiriusSemiDualDrone)
            {
                continue;
            }
            thing.hilight = 2;
        }
    }

    private static bool AAttack_DoWeHaveCannonsThough_Prefix(State s, ref bool __result)
    {
        Combat? c = s.route as Combat;
        if (c == null)
        {
            return true;
        }

        foreach (var (x, stuff) in c.stuff)
        {
            if (stuff is SiriusSemiDualDrone dualDrone && !dualDrone.targetPlayer)
            {
                __result = true;
                return false;
            }
        }

        return true;
    }

    private static void AAttack_Begin_Postfix(AAttack __instance, Combat c)
    {
        if (__instance.fromDroneX != null)
        {
            if (
                c.stuff.TryGetValue(__instance.fromDroneX.Value, out var drone)
                && drone is SiriusSemiDualDrone
            )
            {
                drone.pulse = 1.0;
            }
        }
    }

    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(
            MoveType.After,
            instr => instr.MatchCall("DeepCopy"),
            instr => instr.MatchStfld("attackCopy"),
            instr => instr.MatchCallvirt("QueueImmediate")
        );

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_3);
        cursor.EmitDelegate(
            (AAttack __instance, Combat combat) =>
            {
                combat.QueueImmediate(
                    new ASiriusInquisitorShoot()
                    {
                        attackCopy = Mutil.DeepCopy<AAttack>(__instance),
                    }
                );
            }
        );

        return cursor.Generate();
    }

    public override CardData GetData(State state)
    {
        return new CardData()
        {
            recycle = true,
            cost = 1,
            retain = true,
            buoyant = true,
            unremovableAtShops = true,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.B =>
            [
                new ASpawn
                {
                    thing = new SiriusSemiDualDrone() { bubbleShield = true, hitByEnemy = true },
                },
            ],
            _ =>
            [
                new ASpawn
                {
                    thing = new SiriusSemiDualDrone() { upgrade = this.upgrade, hitByEnemy = true },
                },
            ],
        };
    }
}
