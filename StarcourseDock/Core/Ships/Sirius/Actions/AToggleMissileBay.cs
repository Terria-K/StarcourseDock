using System.Runtime.InteropServices;
using FSPRO;
using Nickel;

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
        List<int> partSlots = [];

        var partSpan = CollectionsMarshal.AsSpan(target.parts);

        for (int i = 0; i < partSpan.Length; i++)
        {
            var part = partSpan[i];
            if (part.type == PType.missiles)
            {
                partSlots.Add(i);
            }
        }

        if (partSlots.Count == 0)
        {
            return null;
        }
        int currentPartX = target.parts.FindIndex(p => p.type == PType.missiles && p.active);
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
                Title = Localization.Str_ship_Sirius_icon_toggleMissileBay_name(),
                Description = Localization.Str_ship_Sirius_icon_toggleMissileBay_description(),
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
