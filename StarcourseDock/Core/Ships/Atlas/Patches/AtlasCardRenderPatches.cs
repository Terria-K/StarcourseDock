using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AtlasCardRenderPatches : IPatchable
{
    [OnPrefix<Card>(nameof(Card.Render))]
    public static void Card_Render_Prefix(Card __instance, G g, State? fakeState, Vec? posOverride, double? overrideWidth, bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return;
        }

        State s = fakeState is not null ? fakeState : g.state;

        if (!s.HasArtifactFromColorless<GyroscopeEngine>())
        {
            return;
        }

        if (s.route is not Combat combat)
        {
            return;
        }

        if (combat.routeOverride is not null)
        {
            return;
        }

        int handCount = combat.hand.Count;
        int handPosition = combat.hand.FindIndex(c => c.uuid == __instance.uuid);
        if (handPosition == -1)
        {
            return;
        }

        double offset = __instance.hoverAnim > 0.0 ? 2 : 0;

        double center = (handCount - 1) / 2.0;
        double distance = handPosition - center;

        int dir = (int)Math.Round(distance, MidpointRounding.AwayFromZero);

        if ((handCount & 1) == 0 && dir == 0)
        {
            dir = (distance > 0) ? 1 : -1;
        }

        var position = posOverride ?? __instance.pos;
        position += new Vec(
            0.0, 
            __instance.hoverAnim * -2.0 + Mutil.Parabola(__instance.flipAnim) * -10.0 + 
                Mutil.Parabola(Math.Abs(__instance.flopAnim)) * -10.0 * Math.Sign(__instance.flopAnim));

        position += new Vec(((overrideWidth ?? 59) - 21) / 2.0, 15 - offset);


        Draw.Sprite(Sprites.cardShared_atlas_move_icon.Sprite, position.x, position.y, color: Colors.white);
        if (dir > 0)
        {
            Draw.Sprite(StableSpr.icons_moveRight, position.x + 2, position.y + 4, color: Colors.white);
        }
        else
        {
            Draw.Sprite(StableSpr.icons_moveLeft, position.x + 2, position.y + 4, color: Colors.white);
        }

        BigNumbers.Render(Math.Abs(dir), position.x + 14, position.y + 4, Colors.textMain);
    }
}