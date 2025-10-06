using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class IoKit : IRegisterable
{
    internal static IDeckEntry IoDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        IoDeck = helper.Content.Decks.RegisterDeck(
            "Io",
            new()
            {
                Definition = new() { color = new Color("ce8e5b"), titleColor = Colors.black },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.cardShared_border_io.Sprite,
                Name = Localization.ship_Io_name(),
            }
        );
    }
}
