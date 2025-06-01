using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal class RoutedCannon : Artifact, IRegisterable
{
    public bool disabled;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "RoutedCannon",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_RoutedCannon.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Alpherg", "artifact", "RoutedCannon", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Alpherg", "artifact", "RoutedCannon", "description"]
                    )
                    .Localize,
            }
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(AAttack_Begin_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            prefix: new HarmonyMethod(AAttack_Begin_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(
                typeof(AVolleyAttackFromAllCannons),
                nameof(AVolleyAttackFromAllCannons.Begin)
            ),
            postfix: new HarmonyMethod(AVolleyAttackFromAllCannons_Begin_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
            prefix: new HarmonyMethod(AAttack_GetTooltips_Prefix)
        );

        MethodInfo? info = null!;

        foreach (var nestedType in typeof(AAttack).GetNestedTypes())
        {
            foreach (var method in nestedType.GetMethods())
            {
                if (method.Name.Contains("<GetFromX>"))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var p = parameters[0];
                        if (p.ParameterType == typeof(Part))
                        {
                            info = method;
                        }
                    }
                }
            }
        }

        ModEntry.Instance.Harmony.Patch(
            info,
            prefix: new HarmonyMethod(AAttack_GetFromX_b__23_0_Prefix)
        );
    }

    public override Spr GetSprite()
    {
        if (disabled)
        {
            return Sprites.artifacts_RoutedCannonInactive.Sprite;
        }
        return Sprites.artifacts_RoutedCannon.Sprite;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (disabled)
        {
            disabled = false;
        }

        combat.Queue(new AModifyCannon() { active = false });
    }

    public override void OnPlayerPlayCard(
        int energyCost,
        Deck deck,
        Card card,
        State state,
        Combat combat,
        int handPosition,
        int handCount
    )
    {
        if (card is RerouteCannon rerouteCannon)
        {
            if (rerouteCannon.upgrade == Upgrade.B)
            {
                return;
            }
            disabled = true;
        }
    }

    private static State? state;
    private static AAttack? aAttack;

    internal static bool AAttack_GetFromX_b__23_0_Prefix(Part p, ref bool __result)
    {
        if (state is null || aAttack is null || aAttack.targetPlayer)
        {
            return true;
        }

        var routedCannon = state.GetArtifact<RoutedCannon>();

        if (routedCannon is not null && !routedCannon.disabled)
        {
            __result = (p.type == PType.empty || p.type == PType.cannon) && p.active;
            return false;
        }

        return true;
    }

    internal static void AAttack_Begin_Prefix(AAttack __instance, State s)
    {
        state = s;
        aAttack = __instance;
    }

    internal static void AAttack_GetTooltips_Prefix(State s)
    {
        var routedCannon = s.GetArtifact<RoutedCannon>();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        int partX = s.ship.x;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                if (
                    s.route is Combat combat
                    && combat.stuff.TryGetValue(partX, out StuffBase? value)
                )
                {
                    value.hilight = 2;
                }
                p.hilight = true;
            }
            partX++;
        }
    }

    internal static void AVolleyAttackFromAllCannons_Begin_Postfix(
        AVolleyAttackFromAllCannons __instance,
        State s,
        Combat c
    )
    {
        var routedCannon = s.GetArtifact<RoutedCannon>();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        List<AAttack> listOfAttacks = new List<AAttack>();
        int i = 0;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                __instance.attack.fromX = new int?(i);
                listOfAttacks.Add(Mutil.DeepCopy(__instance.attack));
            }
            i++;
        }

        c.QueueImmediate(listOfAttacks);
    }

    internal static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt("GetPartTypeCount"));

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_2);
        cursor.EmitDelegate(
            (int x, AAttack aAttack, State s) =>
            {
                if (aAttack.targetPlayer)
                {
                    return x;
                }
                var routedCannon = s.GetArtifact<RoutedCannon>();
                if (routedCannon is null || routedCannon.disabled)
                {
                    return x;
                }

                int cannon = s.ship.GetPartTypeCount(PType.cannon, false);
                int empty = s.ship.GetPartTypeCount(PType.empty, false);

                return cannon + empty;
            }
        );

        return cursor.Generate();
    }
}
