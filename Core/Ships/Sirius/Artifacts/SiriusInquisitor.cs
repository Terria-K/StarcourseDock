using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class SiriusInquisitor : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var inquisitorSprite = helper.Content.Sprites.RegisterAnimation(new SiriusInquisitorSpriteAnimation());

        helper.Content.Artifacts.RegisterArtifact("SiriusInquisitor", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.Common],
                unremovable = true,
            },
            Sprite = inquisitorSprite.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusInquisitor", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusInquisitor", "description"]).Localize
        });
    }

    public override void OnReceiveArtifact(State state)
    {
        var businesses = state.deck.Where(x => x is SiriusBusiness).ToList();

        foreach (var business in businesses)
        {
            state.GetCurrentQueue().QueueImmediate(
                new AAddCard() { card = new SiriusQuestion() { upgrade = business.upgrade }, destination = CardDestination.Deck }
            );
        }

        state.deck = state.deck.Where(x => x is not SiriusBusiness).ToList();
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new SiriusQuestion() }
        ];
    }

    internal class SiriusInquisitorSpriteAnimation : Animation
    {
        private Spr[] frames = [
            Sprites.SiriusInquisitor1.Sprite,
            Sprites.SiriusInquisitor2.Sprite,
            Sprites.SiriusInquisitor3.Sprite,
            Sprites.SiriusInquisitor4.Sprite,
        ];

        public override int MaxFrameLength => 4;

        public override Texture2D Update(G g, State? s)
        {
            if (s == null)
            {
                return SpriteLoader.Get(frames[0])!;
            }
            return SpriteLoader.Get(frames[CurrentFrame])!;
        }
    }
}