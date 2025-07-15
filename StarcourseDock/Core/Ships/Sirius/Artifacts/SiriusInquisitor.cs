using System.Reflection;
using System.Runtime.InteropServices;
using CutebaltCore;
using Microsoft.Xna.Framework.Graphics;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusInquisitor : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SiriusInquisitor",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_SiriusInquisitor.Sprite,
                Name = Localization.ship_Sirius_artifact_SiriusInquisitor_name(),
                Description = Localization.ship_Sirius_artifact_SiriusInquisitor_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        List<Card> newDeck = new List<Card>(state.deck.Count);

        var deckSpan = CollectionsMarshal.AsSpan(state.deck);
        for (int i = 0; i < deckSpan.Length; i++)
        {
            var card = deckSpan[i];
            if (card is SiriusBusiness)
            {
                state
                    .GetCurrentQueue()
                    .QueueImmediate(
                        new AAddCard()
                        {
                            card = new SiriusQuestion() { upgrade = card.upgrade },
                            destination = CardDestination.Deck,
                        }
                    );

                continue;
            }
            newDeck.Add(card);
        }

        state.deck = newDeck;
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard() { card = new SiriusQuestion() }];
    }
}
