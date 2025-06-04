using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AlphergKit : IRegisterable
{
    internal static IDeckEntry AlphergDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        AlphergDeck = helper.Content.Decks.RegisterDeck(
            "Alpherg",
            new()
            {
                Definition = new() { color = new Color("6bc6d3"), titleColor = Colors.white },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.cardShared_border_alpherg.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Alpherg", "name"])
                    .Localize,
            }
        );
    }
}
