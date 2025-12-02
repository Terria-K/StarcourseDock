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
                    dontOffer = true
                },
                Art = Sprites.cards_IORelease_Top.Sprite,
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
            floppable = true,
			singleUse = true,
            art = flipped ? Sprites.cards_IORelease_Bottom.Sprite : Sprites.cards_IORelease_Top.Sprite
		};
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		StuffBase thingActual = Mutil.DeepCopy(thing);
		thingActual.targetPlayer = false;

        if (isCentered)
        {
            return [new AWrapperSpawn() { thing = thingActual, isRandom = true, disabled = flipped }, new ADummyAction(), new ASingleUseDummyAction() { disabled = !flipped}];
        }

		return 
        [
            new AWrapperSpawn
            {
                thing = thingActual,
                isLeft = isLeft,
                disabled = flipped
            },
            new ADummyAction(),
            new ASingleUseDummyAction() { disabled = !flipped }
        ];
	}

    private int GetCost(State s)
    {
        if (flipped && s.HasArtifactFromColorless<TrashHatch>())
        {
            return 0;
        }

        if (s.HasArtifactFromColorless<AsteroidAirlock>() && thing.GetType() == typeof(Asteroid))
        {
            return 0;
        }

        return 1;
    }
}
