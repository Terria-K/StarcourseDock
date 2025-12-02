using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class BurstDrone : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(
            MethodBase.GetCurrentMethod()!.DeclaringType!.Name,
            new()
            {
                CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    deck = IoKit.IoDeck.Deck,
                    rarity = Rarity.rare,
                    dontOffer = true,
                },
                Art = StableSpr.cards_Catch,
                Name = Localization.ship_Io_card_BurstDrone_name(),
            }
        );
    }

    public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = 0,
            temporary = true,
            singleUse = true,
            retain = true,
			art = StableSpr.cards_GoatDrone
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
        return [new ASpawn() { thing = new IoDrone() }];
	}
}

