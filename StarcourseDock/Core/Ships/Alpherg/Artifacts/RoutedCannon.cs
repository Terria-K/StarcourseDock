using System.Reflection;
using System.Reflection.Emit;
using CutebaltCore;
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
                Name = Localization.ship_Alpherg_artifact_RoutedCannon_name(),
                Description = Localization.ship_Alpherg_artifact_RoutedCannon_description(),
            }
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

    internal static bool AAttack_GetFromX_b__23_0_Prefix(Part p, ref bool __result)
    {
        var state = AAttack_Global_Patches.Global_State;
        var aAttack = AAttack_Global_Patches.Global_AAttack;
        if (state is null || aAttack is null || aAttack.targetPlayer)
        {
            return true;
        }

        var routedCannon = state.GetArtifactFromColorless<RoutedCannon>();

        if (routedCannon is not null && !routedCannon.disabled)
        {
            __result = (p.type == PType.empty || p.type == PType.cannon) && p.active;
            return false;
        }

        return true;
    }
}
