using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SpicaKit : IRegisterable
{
    internal static IDeckEntry SpicaDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SpicaDeck = helper.Content.Decks.RegisterDeck(
            "Spica",
            new()
            {
                Definition = new() { color = new Color("5a7752"), titleColor = Colors.white },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.cardShared_border_spica.Sprite,
                Name = Localization.ship_Spica_name(),
            }
        );
    }
}
