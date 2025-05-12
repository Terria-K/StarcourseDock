using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;


internal class RerouteCannon : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var alphergDeck = helper.Content.Decks.RegisterDeck("Alpherg", new() 
        {
            Definition = new() 
            {
                color = new Color("6bc6d3"),
                titleColor = Colors.white
            },
            DefaultCardArt = StableSpr.cards_colorless,
            BorderSprite = Sprites.border_alpherg.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "name"]).Localize
        });

        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new() 
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new() 
            {
                deck = alphergDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Art = StableSpr.cards_BlockerBurnout,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "card", "RerouteCannon", "name"]).Localize
        });
    }

    public override CardData GetData(State state)
    {
        CardData result = upgrade switch 
        {
            Upgrade.A => new()
            {
                recycle = true,
                cost = 0,
                description = ModEntry.Instance.Localizations.Localize(["ship", "Alpherg", "card", "RerouteCannon", "description"]),
                unremovableAtShops = true
            },
            Upgrade.B => new() 
            {
                description = ModEntry.Instance.Localizations.Localize(["ship", "Alpherg", "card", "RerouteCannon", "B", "description"]),
                cost = 1,
                unremovableAtShops = true
            },
            _ => new()
            {
                description = ModEntry.Instance.Localizations.Localize(["ship", "Alpherg", "card", "RerouteCannon", "description"]),
                cost = 1,
                unremovableAtShops = true
            },
        };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AModifyCannon() { active = true }
        ];
    }
}
