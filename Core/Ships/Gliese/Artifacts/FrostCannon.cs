using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FrostCannon : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
		helper.Content.Artifacts.RegisterArtifact("FrostCannon", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.FrostCannon.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "FrostCannon", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "FrostCannon", "description"]).Localize
		});

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AStunPart), nameof(AStunPart.Begin)),
            prefix: new HarmonyMethod(AStunPart_Begin_Prefix)
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        state.ship.Add(Status.stunCharge, 3);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
			new TTGlossary("status.stunCharge", ["3"]),
			new TTGlossary("action.stun", Array.Empty<object>()),
            ..StatusMeta.GetTooltips(ColdStatus.ColdEntry.Status, 1)
        ];
    }

    private static void AStunPart_Begin_Prefix(AStunPart __instance, State s, Combat c)
    {
        FrostCannon? frostCannon = s.GetArtifact<FrostCannon>();
        if (frostCannon is null)
        {
            return;
        }

        Part? part = c.otherShip.GetPartAtWorldX(__instance.worldX);
        if (part != null && part.stunModifier != PStunMod.unstunnable)
        {
            if (part.intent != null)
            {
                c.Queue(new AStatus() 
                {
                    targetPlayer = false,
                    status = ColdStatus.ColdEntry.Status,
                    statusAmount = 1,
                });

                frostCannon.Pulse();
            }
        }
    }
}