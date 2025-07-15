using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusKit : IRegisterable
{
    internal static IDeckEntry SiriusDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SiriusDeck = helper.Content.Decks.RegisterDeck(
            "Sirius",
            new()
            {
                Definition = new() { color = new Color("6c9ebd"), titleColor = Colors.black },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.cardShared_border_sirius.Sprite,
                Name = Localization.ship_Sirius_name(),
            }
        );
    }
}
