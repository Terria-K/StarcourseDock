using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class Release : Card, IRegisterable
{
	public StuffBase thing = new FakeDrone();
    public bool isLeft;
    public bool isCentered;

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
                    rarity = Rarity.uncommon,
                    dontOffer = true,
                    upgradesTo = [Upgrade.A]
                },
                Art = StableSpr.cards_Catch,
                Name = Localization.ship_Io_card_PulledAndRelease_name(),
            }
        );
    }

    public override CardData GetData(State state)
	{
		return new CardData
		{
			cost = GetCost(state),
            temporary = true,
            flippable = isCentered,
			singleUse = true,
			art = StableSpr.cards_Release
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		StuffBase thingActual = Mutil.DeepCopy(thing);
		thingActual.targetPlayer = false;

        if (isCentered)
        {
            return upgrade switch 
            {
                Upgrade.A => [
                    new AWrapperSpawn() { thing = thingActual, isLeft = !flipped }, 
                    new ADrawCard() { count = 1 }
                ],
                _ => [new AWrapperSpawn() { thing = thingActual, isLeft = !flipped }],
            };
        }

		return upgrade switch
        {
            Upgrade.A => [
                new AWrapperSpawn() { thing = thingActual, isLeft = isLeft }, 
                new ADrawCard() { count = 1 }
            ],
            _ => [
                new AWrapperSpawn
                {
                    thing = thingActual,
                    isLeft = isLeft
                }
            ]
        };
	}

    private int GetCost(State s)
    {
        if (s.HasArtifactFromColorless<AsteroidAirlock>() && thing.GetType() == typeof(Asteroid))
        {
            return 0;
        }

        return 1;
    }
}
