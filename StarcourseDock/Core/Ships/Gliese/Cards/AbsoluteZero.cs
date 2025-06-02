using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class AbsoluteZero : Card, IRegisterable
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
                Art = StableSpr.cards_FreezeDry,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "card", "AbsoluteZero", "name"]
                    )
                    .Localize,
            }
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActualDamage)),
            postfix: new HarmonyMethod(Card_GetActualDamage_Postfix)
        );
    }

    public override CardData GetData(State state)
    {
        CardData result = new()
        {
            cost = 0,
            temporary = true,
            description = ModEntry.Instance.Localizations.Localize(
                ["ship", "Gliese", "card", "AbsoluteZero", "description"]
            ),
        };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [new AShuffleShip() { targetPlayer = true }];
    }

    private static void Card_GetActualDamage_Postfix(State s, bool targetPlayer, ref int __result)
    {
        if (targetPlayer)
        {
            return;
        }

        Combat? combat = s.route as Combat;

        if (combat == null)
        {
            return;
        }

        if (combat.hand.OfType<AbsoluteZero>().Any())
        {
            __result = 0;
        }
    }
}
