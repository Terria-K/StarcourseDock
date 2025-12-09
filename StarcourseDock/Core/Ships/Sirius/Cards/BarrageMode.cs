using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class BarrageMode : Card, IRegisterable
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
                    deck = SiriusKit.SiriusDeck.Deck,
                    rarity = Rarity.rare,
                    dontOffer = true,
                },
                Art = StableSpr.cards_Terminal,
                Name = Localization.ship_Sirius_card_BarrageMode_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return new()
        {
            description = Localization.Str_ship_Sirius_card_BarrageMode_description(),
            temporary = true,
            retain = true,
            singleUse = true,
            flippable = true,
            cost = 1
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AActivateAllPartsWrapper() { partType = PType.missiles }
        ];
    }

    public override void OnFlip(G g)
    {
        Combat? c = g.state.route as Combat;
        if (c != null)
        {
            c.QueueImmediate(new AToggleMissileBay());
        }
    }
}
