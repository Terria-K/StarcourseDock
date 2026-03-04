using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CloseIn : Card, IRegisterable
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
                    deck = SpicaKit.SpicaDeck.Deck,
                    rarity = Rarity.common,
                    dontOffer = true,
                },
                Art = StableSpr.cards_ScootLeft,
                Name = Localization.ship_Spica_card_CloseIn_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return new()
        {
                temporary = true,
                flippable = true,
                retain = true,
                cost = 1,
                art = flipped ? StableSpr.cards_ScootRight : StableSpr.cards_ScootLeft,
                singleUse = true,
                description = flipped
                    ? Localization.Str_ship_Spica_card_CloseIn_Flipped_description()
                    : Localization.Str_ship_Spica_card_CloseIn_description(),
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [new AMergeScaffold() { flipped = flipped }];
    }
}
