using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class WolfRayetKit : IRegisterable
{
    internal static IDeckEntry WolfRayetDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        WolfRayetDeck = helper.Content.Decks.RegisterDeck(
            "WolfRayet",
            new()
            {
                Definition = new() { color = new Color("ff5b21"), titleColor = Colors.black },
                DefaultCardArt = StableSpr.cards_hacker,
                BorderSprite = Sprites.cardShared_border_wolfrayet.Sprite,
                Name = Localization.ship_WolfRayet_name(),
            }
        );
    }
}
