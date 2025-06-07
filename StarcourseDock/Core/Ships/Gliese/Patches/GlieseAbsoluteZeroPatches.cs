using CutebaltCore;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed partial class GlieseAbsoluteFreezePatches : IPatchable
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
        ref int __result
    )
    {
        if (action is not AFreezeCardWrapper freezeCard || freezeCard.action is null)
        {
            return true;
        }

        var position = g.Push(rect: new()).rect.xy;
        var initialX = (int)position.x;

        if (!dontDraw)
        {
            bool isDisabled = freezeCard.action.disabled;
            Color color;
            if (isDisabled)
            {
                color = Colors.disabledIconTint;
            }
            else
            {
                color = Colors.white;
            }
            if (freezeCard.rightSide)
            {
                Draw.Sprite(
                    Sprites.icons_right_freeze.Sprite,
                    position.x,
                    position.y,
                    color: color
                );
            }
            else
            {
                Draw.Sprite(Sprites.icons_left_freeze.Sprite, position.x, position.y, color: color);
            }
        }

        position.x += 10;
        g.Push(rect: new(position.x - initialX, 0));
        position.x += Card.RenderAction(
            g,
            state,
            freezeCard.action,
            dontDraw,
            shardAvailable,
            stunChargeAvailable,
            bubbleJuiceAvailable
        );
        g.Pop();
        __result = (int)position.x - initialX;

        g.Pop();

        return false;
    }
}
