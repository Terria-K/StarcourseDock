using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;

namespace Teuria.StarcourseDock;

public interface IAmFloppableThreeTimes
{
    int FlipIndex { get; set; }
    bool KnownFlipped { get; set; }
    // note to self: do not forget to mark this as [JsonIgnore] (Newtonsoft Edition)
    bool Flipped { get; }
    bool Active { get; }

    void HandleFlip()
    {
        if (KnownFlipped == Flipped)
        {
            return;
        }

        FlipIndex = (FlipIndex + 1) % 3;
        KnownFlipped = Flipped;
    }
}

internal sealed class Release : Card, IRegisterable, IAmFloppableThreeTimes
{
	public StuffBase thing = new FakeDrone();
    public bool isLeft;
    public bool isCentered;
    public bool KnownFlipped { get; set; }
    public int FlipIndex { get; set; }
    [JsonIgnore]
    public bool Flipped => flipped;

    [JsonIgnore]
    public bool Active => isCentered; 

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
        var cardData = new CardData()
        {
            cost = GetCost(state),
            temporary = true,
            floppable = true,
            singleUse = true
        };
        if (isCentered)
        {
            (this as IAmFloppableThreeTimes).HandleFlip();
            return cardData with
            {
                art = FlipIndex switch
                {
                    1 => Sprites.cards_IORelease_Triple1.Sprite,
                    2 => Sprites.cards_IORelease_Triple2.Sprite,
                    _ => Sprites.cards_IORelease_Triple0.Sprite
                }
            };
        }

        return cardData with
        {
            art = flipped ? Sprites.cards_IORelease_Bottom.Sprite : Sprites.cards_IORelease_Top.Sprite
        };
	}

    public override void OnFlip(G g)
    {
        base.OnFlip(g);
        (this as IAmFloppableThreeTimes).HandleFlip();
    }

	public override List<CardAction> GetActions(State s, Combat c)
	{
		StuffBase thingActual = Mutil.DeepCopy(thing);
		thingActual.targetPlayer = false;

        if (isCentered)
        {
            return [
                new AWrapperSpawn() { thing = thingActual, disabled = FlipIndex != 0, isLeft = true },
                new AWrapperSpawn() { thing = thingActual, disabled = FlipIndex != 1, isLeft = false },
                new ASingleUseDummyAction() { disabled = FlipIndex != 2 }
            ];
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
        if (isCentered)
        {
            if (s.HasArtifactFromColorless<TrashHatch>() && FlipIndex % 3 == 2)
            {
                return 0;
            }
        }
        else if (flipped && s.HasArtifactFromColorless<TrashHatch>())
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
