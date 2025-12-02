using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class IoCardRender : IPatchable
{
    [OnPrefix<Card>(nameof(Card.RenderAction))]
    private static bool Card_RenderAction_Prefix(
        G g, 
        State state, 
        CardAction action, 
        bool dontDraw, 
        int shardAvailable, 
        int stunChargeAvailable, 
        int bubbleJuiceAvailable, 
        ref int __result)
    {
        if (action is not AWrapperSpawn spawn)
        {
            return true;
        }
        
		var position = g.Push(rect: new()).rect.xy;
		var initialX = (int)position.x;
        Color? color = action.disabled ? Colors.disabledText : null;

        if (spawn.isLeft)
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_left_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_left_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }
        else if (spawn.isRandom)
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_random_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_random_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }
        else 
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_right_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_right_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }

        g.Push(rect: new(position.x - initialX, 0));

        var icon = spawn.GetIcon(state);
        if (icon is {} i)
        {
            if (!dontDraw)
            {
                Draw.Sprite(i.path, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(i.path)?.Width ?? 0;
        }

		g.Pop();

		__result = (int)position.x - initialX;
		g.Pop();

        return false;
    }
}

