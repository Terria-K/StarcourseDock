using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;

namespace Teuria.StarcourseDock;

public interface IAmFloppableThreeTimesAndFlippable
{
    int FlipIndex { get; set; }
    bool KnownFlipped { get; set; }
    bool ShouldFlipIconFlipX { get; set; }
    // note to self: do not forget to mark this as [JsonIgnore] (Newtonsoft Edition)
    bool Flipped { get; }
    bool Flippable { get; }
    bool FlipOverrides { get; set; }

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

internal sealed class Release : Card, IRegisterable, IAmFloppableThreeTimesAndFlippable
{
	public StuffBase thing = new FakeDrone();
    public bool isLeft;
    public bool isCentered;
    public bool KnownFlipped { get; set; }
    public int FlipIndex { get; set; }
    [JsonIgnore]
    public bool Flipped => flipped;

    public bool ShouldFlipIconFlipX { get; set; }

    [JsonIgnore]
    public bool Flippable => FlipOverrides || isCentered;

    public bool FlipOverrides { get; set; }

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
                Art = Sprites.cards_IORelease_Triple0.Sprite,
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
            singleUse = true,
            flippable = Flippable
        };

        (this as IAmFloppableThreeTimesAndFlippable).HandleFlip();

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

    public override void OnFlip(G g)
    {
        base.OnFlip(g);
        (this as IAmFloppableThreeTimesAndFlippable).HandleFlip();

        if (Flippable && FlipIndex == 0)
        {
            flipAnim = 1.0;
            flopAnim = 0;
            ShouldFlipIconFlipX = !ShouldFlipIconFlipX;
        }
    }

	public override List<CardAction> GetActions(State s, Combat c)
	{
		StuffBase thingBackwards = Mutil.DeepCopy(thing);
		thingBackwards.targetPlayer = !thingBackwards.targetPlayer;

		return 
        [
            new AWrapperSpawn
            {
                thing = thingBackwards,
                isLeft = Flippable ? !ShouldFlipIconFlipX : isLeft,
                disabled = FlipIndex != 0
            },
            new AWrapperSpawn
            {
                thing = thing,
                isLeft = Flippable ? !ShouldFlipIconFlipX : isLeft,
                disabled = FlipIndex != 1
            },
            new ASingleUseDummyAction() { disabled = FlipIndex != 2 }
        ];
	}

    private int GetCost(State s)
    {
        if (s.HasArtifactFromColorless<TrashHatch>() && FlipIndex == 2)
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
