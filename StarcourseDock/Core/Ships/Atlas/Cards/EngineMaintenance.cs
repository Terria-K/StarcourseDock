using System.Collections.Frozen;
using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class EngineMaintenance : Card, IRegisterable, IHasCustomCardTraits
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
                    rarity = Rarity.uncommon,
                    dontOffer = true,
                    upgradesTo = [Upgrade.A]
                },
                Art = Sprites.cards_EngineMaintenance.Sprite,
                Name = Localization.ship_Atlas_card_EngineMaintenance_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 1,
            temporary = true,
            art = upgrade switch
            {
                Upgrade.A => Sprites.cards_EngineMaintenanceA.Sprite,
                _ => Sprites.cards_EngineMaintenance.Sprite
            }
        };
    }

    public override void OnDraw(State s, Combat c)
    {
        base.OnDraw(s, c);
        if (upgrade == Upgrade.A)
        {
            c.QueueImmediate(new AStatus()
            {
                targetPlayer = true,
                status = Status.engineStall,
                statusAmount = 3
            });
        }
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.A)
        {
            return 
            [
                new AStatus() { status = Status.engineStall, statusAmount = 0, mode = AStatusMode.Set, targetPlayer = true, omitFromTooltips = true },
                new ADummyAction(),
                ModEntry.Instance.KokoroAPI.V2.SpoofedActions.MakeAction(
                    new AStatus() { status = Status.engineStall, statusAmount = 2, targetPlayer = true },
                    new ADummyActionButHasTooltip()
                ).AsCardAction,
            ];
        }
        return
        [
            ModEntry
                .Instance.KokoroAPI.V2.OnTurnEnd.MakeAction(
                    new AStatus() 
                    {
                        targetPlayer = true,
                        status = Status.lockdown,
                        statusAmount = 2
                    }
                )
                .SetShowOnTurnEndIcon(false)
                .SetShowOnTurnEndTooltip(false)
                .AsCardAction
        ];
    }

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        if (upgrade == Upgrade.A)
        {
            return new HashSet<ICardTraitEntry>()
            {
                AtlasKit.OnDrawTriggerTrait
            };
        }
        return new HashSet<ICardTraitEntry>()
        {
            AtlasKit.TurnEndTriggerTrait
        }.ToFrozenSet();
    }
}