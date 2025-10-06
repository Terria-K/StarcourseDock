using Nickel;

namespace Teuria.StarcourseDock;

internal class AWrapperSpawn : ASpawn
{
    public bool isLeft;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        if (isLeft)
        {
            int? fromX = s.ship.GetLocalXOfPart("bayleft");
            if (fromX is { } x)
            {
                thing.x = x;
                c.QueueImmediate(new ASpawn() { thing = thing, multiBayVolley = true, fromX = x, timer = 0.0 });
            }
        }
        else 
        {
            int? fromX = s.ship.GetLocalXOfPart("bayright");
            if (fromX is { } x)
            {
                thing.x = x;
                c.QueueImmediate(new ASpawn() { thing = thing, multiBayVolley = true, fromX = x, timer = 0.0 });
            }
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> ttItems = [];
        int partX = s.ship.x;
        int? fromX;
        if (isLeft)
        {
            fromX = s.ship.GetLocalXOfPart("bayleft");
        }
        else 
        {
            fromX = s.ship.GetLocalXOfPart("bayright");
        }

        if (fromX is { } x) 
        {
            var p = s.ship.GetPartAtLocalX(x);
            if (p is not null)
            {
                if (p.type == PType.missiles && p.active)
                {
                    if (s.route is Combat c && c.stuff.TryGetValue(partX, out StuffBase? value))
                    {
                        value.hilight = 2;
                    }
                    p.hilight = true;
                }
            }
        }


        if (isLeft)
        {
            ttItems.Add(
                new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::baydirleft")
                {
                    Title = Localization.Str_ship_Io_icon_launchleft_name(),
                    TitleColor = Colors.action,
                    Description = Localization.Str_ship_Io_icon_launchleft_description(),
                    Icon = Sprites.icons_left_bay_spawn.Sprite
                }
            );
        }
        else 
        {
            ttItems.Add(
                new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::baydirright")
                {
                    Title = Localization.Str_ship_Io_icon_launchright_name(),
                    TitleColor = Colors.action,
                    Description = Localization.Str_ship_Io_icon_launchright_description(),
                    Icon = Sprites.icons_right_bay_spawn.Sprite
                }
            );
        }

        ttItems.AddRange(thing.GetTooltips());
        return ttItems;
    }
}

