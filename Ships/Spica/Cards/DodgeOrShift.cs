using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class DodgeOrShift : Card, IRegisterable
{
    private static ISpriteEntry dodgeOrShiftBottom = null!;
    private static ISpriteEntry dodgeOrShiftTop = null!;

	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new() 
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new() 
            {
                deck = Deck.colorless,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "card", "DodgeOrShift", "name"]).Localize
        });

        dodgeOrShiftBottom = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Bottom.png"));
        dodgeOrShiftTop = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Top.png"));
    }

    public override CardData GetData(State state)
    {
        CardData result = upgrade switch 
        {
            Upgrade.A => new()
            {
                cost = 0,
                floppable = true
            },
            Upgrade.B => new()
            {
                cost = 1,
                floppable = true
            },
            _ => new()
            {
                cost = 1,
                floppable = true
            },
        };
        result.art = this.flipped ? dodgeOrShiftBottom.Sprite : dodgeOrShiftTop.Sprite;
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch {
            Upgrade.A => [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = !flipped
                },
            ],
            Upgrade.B => [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 2,
                    targetPlayer = true,
                    disabled = flipped
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 2,
                    targetPlayer = true,
                    disabled = !flipped
                },
            ],
            _ => [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = !flipped
                },
            ],
        };
    }
}