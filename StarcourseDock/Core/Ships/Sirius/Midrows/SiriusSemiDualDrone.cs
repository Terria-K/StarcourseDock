using System.Runtime.CompilerServices;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusSemiDualDrone : StuffBase
{
    public static List<string> droneNames = new List<string> { "sirius", "traitorLater" };
    public Upgrade upgrade;
    public bool hitByEnemy;

    public override bool IsHostile()
    {
        return targetPlayer;
    }

    public override bool Invincible()
    {
        if (
            AAttack_Global_Patches.Global_AAttack == null
            || !AAttack_Global_Patches.Global_AAttack.targetPlayer
        )
        {
            return CheckForOtherPossibleCauses();
        }

        return !targetPlayer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckForOtherPossibleCauses()
    {
        var state = MG.inst.g.state;

        var combat = state.route as Combat;
        if (combat is null)
        {
            return false;
        }

        if (combat.isPlayerTurn)
        {
            return targetPlayer;
        }

        return !targetPlayer;
    }

    public override List<string> PossibleDroneNames()
    {
        return droneNames;
    }

    public override void Render(G g, Vec v)
    {
        Vec offset = v + GetOffset(g);
        DrawWithHilight(
            g,
            upgrade switch
            {
                Upgrade.A => Sprites.drones_siriusSemiDualDroneMKII.Sprite,
                _ => Sprites.drones_siriusSemiDualDrone.Sprite,
            },
            offset,
            flipY: targetPlayer
        );
    }

    public override List<Tooltip> GetTooltips()
    {
        List<Tooltip> ttItems =
        [
            upgrade switch
            {
                Upgrade.A => new GlossaryTooltip(
                    $"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusSemiDualDrone"
                )
                {
                    Title = ModEntry.Instance.Localizations.Localize(
                        ["ship", "Sirius", "midrow", "SiriusSemiDualDroneMKII", "name"]
                    ),
                    TitleColor = Colors.midrow,
                    Description = ModEntry.Instance.Localizations.Localize(
                        ["ship", "Sirius", "midrow", "SiriusSemiDualDroneMKII", "description"]
                    ),
                    Icon = Sprites.icons_siriusDroneMkII.Sprite,
                    flipIconY = targetPlayer,
                },
                _ => new GlossaryTooltip(
                    $"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusDrone"
                )
                {
                    Title = ModEntry.Instance.Localizations.Localize(
                        ["ship", "Sirius", "midrow", "SiriusSemiDualDrone", "name"]
                    ),
                    TitleColor = Colors.midrow,
                    Description = ModEntry.Instance.Localizations.Localize(
                        ["ship", "Sirius", "midrow", "SiriusSemiDualDrone", "description"]
                    ),
                    Icon = Sprites.icons_siriusDrone.Sprite,
                    flipIconY = targetPlayer,
                },
            },
        ];

        if (bubbleShield)
        {
            ttItems.Add(new TTGlossary("midrow.bubbleShield", Array.Empty<object>()));
        }
        return ttItems;
    }

    public override Spr? GetIcon()
    {
        return upgrade switch
        {
            Upgrade.A => Sprites.icons_siriusSemiDualDroneMkII.Sprite,
            _ => Sprites.icons_siriusSemiDualDrone.Sprite,
        };
    }

    public override List<CardAction>? GetActionsOnBonkedWhileInvincible(
        State s,
        Combat c,
        bool wasPlayer,
        StuffBase thing
    )
    {
        hitByEnemy = !wasPlayer;
        return [new AChangeTeam() { target = this, targetPlayer = !wasPlayer }];
    }

    public override List<CardAction>? GetActionsOnShotWhileInvincible(
        State s,
        Combat c,
        bool wasPlayer,
        int damage
    )
    {
        hitByEnemy = !wasPlayer;
        return [new AChangeTeam() { target = this, targetPlayer = !wasPlayer }];
    }
}
