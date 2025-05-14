using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FrostCannon : Artifact, IRegisterable
{
    public int count;

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
        count = 0;
        int stunCharges = state.ship.Get(Status.stunCharge);
        if (stunCharges < 3)
        {
            state.ship.Add(Status.stunCharge, 3);
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
			new TTGlossary("status.stunCharge", ["3"]),
			new TTGlossary("status.lockdown"),
			new TTGlossary("action.stun", Array.Empty<object>()),
			new TTGlossary("action.stunShip"),
        ];
    }

    private static void AStunPart_Begin_Prefix(AStunPart __instance, State s, Combat c)
    {
        FrostCannon? frostCannon = s.EnumerateAllArtifacts().OfType<FrostCannon>().FirstOrDefault();
        if (frostCannon is null)
        {
            return;
        }

        Part? part = c.otherShip.GetPartAtWorldX(__instance.worldX);
        if (part != null && part.stunModifier != PStunMod.unstunnable)
        {
            int stunStatus = s.ship.Get(Status.stunCharge);
            if (part.intent != null && stunStatus > 0)
            {
                frostCannon.count += 1;

                if (frostCannon.count >= 3)
                {
                    c.otherShip.Add(Status.lockdown, 1);
                    frostCannon.count = 0;
                    c.Queue(new AStunShip());
                }
            }
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }
}
