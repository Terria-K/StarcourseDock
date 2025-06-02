using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class ShrinkMechanism : Artifact, IRegisterable
{
    public List<Part>? tempParts;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "ShrinkMechanism",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_ShrinkMechanism.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Spica", "artifact", "ShrinkMechanism", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Spica", "artifact", "ShrinkMechanism", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard { card = new Shrink() }];
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (!combat.HasCardOnHand<Shrink>())
        {
            combat.Queue(new AAddCard { card = new Shrink(), destination = CardDestination.Hand });
        }

        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
            return;
        }

        Reset(state);
    }

    private static void Reset(State state)
    {
        if (state.ship.IsPartExists("toRemove"))
        {
            return;
        }

        int cannonIndex = state.ship.FindPartIndex("closeToScaffold");

        state.ship.parts.Insert(
            cannonIndex + 1,
            new Part()
            {
                type = PType.empty,
                skin = SpicaShip.SpicaScaffold.UniqueName,
                key = "toRemove",
            }
        );

        if (state.ship.IsTouchingRightWall(state))
        {
            state.ship.x -= 1;
            state.ship.xLerped -= 1;
        }
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new AResetShip { parts = tempParts });
    }
}
