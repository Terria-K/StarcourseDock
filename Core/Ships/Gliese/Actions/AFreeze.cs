using FSPRO;
using Nickel;

namespace Teuria.StarcourseDock;

internal class AFreeze : CardAction
{
    public bool targetPlayer;

    public override void Begin(G g, State s, Combat c)
    {
        Ship target = (this.targetPlayer ? s.ship : c.otherShip);
        if (target == null)
        {
            return;
        }

        c.QueueImmediate(new AStunShip() { targetPlayer = target.isPlayerShip });

        this.timer = 1.0;
        target.Set(ColdStatus.ColdEntry.Status, 0);
        if (targetPlayer)
        {
            target.Add(Status.energyLessNextTurn, 1);
        }
        target.Add(Status.lockdown, 1);

        Audio.Play(Event.Hits_ShieldPop);
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(Sprites.icons_freeze.Sprite, null, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return GetTooltipsGlobal();
    }

    public static List<Tooltip> GetTooltipsGlobal()
    {
        return
        [
            new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::freeze")
            {
                Title = ModEntry.Instance.Localizations.Localize(["action", "Freeze", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(
                    ["action", "Freeze", "description"]
                ),
                TitleColor = Colors.action,
                Icon = Sprites.icons_freeze.Sprite,
            },
            new TTGlossary("status.lockdown"),
            new TTGlossary("action.stunShip"),
        ];
    }
}
