using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed class AlbireoPolarityRenderCardPatches
{
    [HarmonyPatch(typeof(Combat), nameof(Combat.RenderCards)), HarmonyPostfix]
    private static void Combat_RenderCards_Postfix(Combat __instance, G g)
    {
        var status = g.state.ship.Get(Polarity.PolarityStatus.Status);
        if (status == 0)
        {
            return;
        }

        if (__instance.hand.Count <= 1)
        {
            return;
        }

        const int YOffset = 85;

        if (__instance.hand.Count <= 3)
        {
            Card leftCard = __instance.hand[0];
            Rect leftRect = leftCard.GetScreenRect() + leftCard.pos + new Vec(0, leftCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push.Sprite, leftRect.x - 8, leftRect.y + YOffset);

            Card rightCard = __instance.hand[__instance.hand.Count -1];
            Rect rightRect = rightCard.GetScreenRect() + rightCard.pos + new Vec(0, rightCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_orange.Sprite, rightRect.x + rightRect.w -1, rightRect.y + YOffset);
        }
        else
        {
            Card leftCard = __instance.hand[0];
            Rect leftRect = leftCard.GetScreenRect() + leftCard.pos + new Vec(0, leftCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push.Sprite, leftRect.x - 8, leftRect.y + YOffset);

            Card leftSecondCard = __instance.hand[1];
            Rect leftSecondRect = leftSecondCard.GetScreenRect() + leftSecondCard.pos + new Vec(0, leftSecondCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_second.Sprite, leftSecondRect.x - 8, leftSecondRect.y + YOffset);

            Card rightCard = __instance.hand[__instance.hand.Count -1];
            Rect rightRect = rightCard.GetScreenRect() + rightCard.pos + new Vec(0, rightCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_orange.Sprite, rightRect.x + rightRect.w - 1, rightRect.y + YOffset);

            Card rightSecondCard = __instance.hand[__instance.hand.Count -2];
            Rect rightSecondRect = rightSecondCard.GetScreenRect() + rightSecondCard.pos + new Vec(0, rightSecondCard.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_orange_second.Sprite, rightSecondRect.x + rightSecondRect.w - 1, rightSecondRect.y + YOffset);
        }
    }
}
