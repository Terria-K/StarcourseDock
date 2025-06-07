using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class AbsoluteFreeze : Card, IRegisterable, IHasCustomCardTraits
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
                    deck = Deck.trash,
                    rarity = Rarity.rare,
                    dontOffer = true,
                },
                Art = Sprites.cards_AbsoluteFreeze.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "card", "AbsoluteFreeze", "name"]
                    )
                    .Localize,
            }
        );
    }

    public override CardData GetData(State state)
    {
        CardData result = new() { cost = 1, temporary = true };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            ModEntry
                .Instance.KokoroAPI.V2.OnTurnEnd.MakeAction(
                    new AFreezeCardWrapper()
                    {
                        action = new AFreezeCard()
                        {
                            selectedCard = GetLeftCardOnHand(c),
                            increment = 2,
                        },
                    }
                )
                .SetShowOnTurnEndIcon(false)
                .SetShowOnTurnEndTooltip(false)
                .AsCardAction,
            ModEntry
                .Instance.KokoroAPI.V2.OnTurnEnd.MakeAction(
                    new AFreezeCardWrapper()
                    {
                        rightSide = true,
                        action = new AFreezeCard()
                        {
                            selectedCard = GetRightCardOnHand(c),
                            increment = 2,
                        },
                    }
                )
                .SetShowOnTurnEndIcon(false)
                .SetShowOnTurnEndTooltip(false)
                .AsCardAction,
        ];
    }

    private Card? GetLeftCardOnHand(Combat c)
    {
        int index = c.hand.IndexOf(this);
        if (index > 0)
        {
            return c.hand[index - 1];
        }
        return null;
    }

    private Card? GetRightCardOnHand(Combat c)
    {
        int index = c.hand.IndexOf(this);
        if (index >= 0 && index < c.hand.Count - 1)
        {
            return c.hand[index + 1];
        }
        return null;
    }

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        return new HashSet<ICardTraitEntry>() { GlieseKit.TurnEndTriggerTrait };
    }
}
