using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal class SiriusBusiness : Card, IRegisterable
{
    internal static IDeckEntry SiriusDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SiriusDeck = helper.Content.Decks.RegisterDeck("Sirius", new() 
        {
            Definition = new() 
            {
                color = new Color("6c9ebd"),
                titleColor = Colors.black
            },
            DefaultCardArt = StableSpr.cards_colorless,
            BorderSprite = Sprites.border_sirius.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "name"]).Localize
        });

        helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = SiriusDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Art = StableSpr.cards_GoatDrone,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "card", "SiriusBusiness", "name"]).Localize,
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AJupiterShoot), nameof(AJupiterShoot.Begin)),
            new HarmonyMethod(AJupiterShoot_Begin_Prefix)
        );
    }

    private static void AJupiterShoot_Begin_Prefix(AJupiterShoot __instance, State s, Combat c)
    {
        bool damageUpgraded = false;
        bool damageUpgradedAgain = false;
        SortedList<int, CardAction> siriusAttacks = [];
        foreach ((int x, StuffBase midRow) in c.stuff)
        {
            if (midRow is not SiriusDrone sirius)
            {
                continue;
            }

            if (sirius.upgrade == Upgrade.A && !damageUpgraded)
            {
                __instance.attackCopy.damage += 1;
                damageUpgraded = true;
            }

            int partX = s.ship.parts.FindIndex(p => p.type == PType.comms);
            if (partX >= 0 && midRow.x == partX + s.ship.x)
            {
                if (sirius.upgrade == Upgrade.A && !damageUpgradedAgain)
                {
                    __instance.attackCopy.damage += 1;
                    damageUpgradedAgain = true;
                }
                continue;
            }

            AAttack copy = Mutil.DeepCopy<AAttack>(__instance.attackCopy);
            copy.fast = true;
            copy.fromX = null;
            copy.fromDroneX = midRow.x;
            copy.targetPlayer = !midRow.targetPlayer;
            copy.shardcost = 0;
            int beforeDamage = copy.damage;
            foreach (Artifact r in s.EnumerateAllArtifacts())
            {
                copy.damage += r.ModifyBaseJupiterDroneDamage(s, c, midRow);
                if (copy.damage > beforeDamage)
                {
                    copy.artifactPulse = r.Key();
                    beforeDamage = copy.damage;
                }
            }
            siriusAttacks.Add(midRow.x, copy);
        }
        c.QueueImmediate(siriusAttacks.Values);
    }

    public override CardData GetData(State state)
    {
        return new CardData()
        {
            recycle = true,
            cost = 1,
            retain = true,
            buoyant = true,
            unremovableAtShops = true
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.B => [
                new ASpawn
                {
                    thing = new SiriusDrone() { bubbleShield = true }
                }
            ],
            _ => [
                new ASpawn
                {
                    thing = new SiriusDrone() { upgrade = this.upgrade }
                }
            ],
        };

    }
}