using FSPRO;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class AToggleMissileBay : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        this.timer = 0.3;
        bool anyIsActive = false;
        HashSet<string> validSiriusParts = ["firstBay", "weak"];

        foreach (Part p in s.ship.parts)
        {
            if (p.key == null || !validSiriusParts.Contains(p.key))
            {
                continue;
            }

            p.active = !p.active;

            if (p.active)
            {
                anyIsActive = true;
            }
        }

        if (!anyIsActive)
        {
            int cannonX = s.ship.parts.FindIndex(
                (Part p) => p.key != null && validSiriusParts.Contains(p.key) && !p.active
            );

            if (cannonX != -1)
            {
                s.ship.parts[cannonX].active = true;
            }
        }
        Audio.Play(Event.TogglePart, true);
    }

    private AToggleMissileBay.Res? GetNextEnabledPartX(State s, Combat c)
    {
        global::Ship target = s.ship;
        if (target == null)
        {
            return null;
        }
        List<int> partSlots = target
            .parts.AsValueEnumerable()
            .Select((Part p, int i) => new { part = p, x = i })
            .Where(x => x.part.type == PType.cannon)
            .Select(x => x.x)
            .ToList();

        if (partSlots.Count == 0)
        {
            return null;
        }
        int currentPartX = target.parts.FindIndex(p => p.type == PType.cannon && p.active);
        int nextEnabledPartSlot =
            (partSlots.FindIndex(x => x == currentPartX) + 1) % partSlots.Count;
        return new AToggleMissileBay.Res
        {
            target = target,
            currentPartX = currentPartX,
            nextPartX = partSlots[nextEnabledPartSlot],
        };
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        Combat? c = s.route as Combat;
        if (c != null)
        {
            AToggleMissileBay.Res? res = GetNextEnabledPartX(s, c);
            if (res != null && res.target != null)
            {
                res.target.parts[res.nextPartX].hilightToggle = true;
                res.target.parts[res.currentPartX].hilightToggle = true;
            }
        }
        return
        [
            new GlossaryTooltip(
                $"{ModEntry.Instance.Package.Manifest.UniqueName}::ToggleMissileBayIcon"
            )
            {
                Title = ModEntry.Instance.Localizations.Localize(
                    ["ship", "Sirius", "icon", "toggleMissileBay", "name"]
                ),
                Description = ModEntry.Instance.Localizations.Localize(
                    ["ship", "Sirius", "icon", "toggleMissileBay", "description"]
                ),
                TitleColor = Colors.action,
                IsWideIcon = true,
                Icon = Sprites.icons_sirius_toggle.Sprite,
            },
        ];
    }

    public override Icon? GetIcon(State s) =>
        new Icon(Sprites.icons_sirius_toggle.Sprite, null, Colors.white);

    private class Res
    {
        public global::Ship? target;
        public int currentPartX;
        public int nextPartX;
    }
}
