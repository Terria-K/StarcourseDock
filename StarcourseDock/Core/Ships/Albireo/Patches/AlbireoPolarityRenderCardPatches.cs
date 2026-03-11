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

        Card leftCard = __instance.hand[0];
        Rect leftRect = leftCard.GetScreenRect() + leftCard.pos + new Vec(0, leftCard.hoverAnim * -2.0);
        Draw.Sprite(Sprites.icons_polarity_push.Sprite, leftRect.x - 8, leftRect.y + YOffset);

        Card rightCard = __instance.hand[__instance.hand.Count -1];
        Rect rightRect = rightCard.GetScreenRect() + rightCard.pos + new Vec(0, rightCard.hoverAnim * -2.0);
        Draw.Sprite(Sprites.icons_polarity_push_orange.Sprite, rightRect.x + rightRect.w -1, rightRect.y + YOffset);
    }

    [HarmonyPatch(typeof(Card), nameof(Card.Render)), HarmonyPostfix]
    public static void Card_Render_Postfix(Card __instance, G g, State? fakeState, Vec? posOverride, double? overrideWidth, bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return;
        }

        State s = fakeState is not null ? fakeState : g.state;

        var status = g.state.ship.Get(Polarity.PolarityStatus.Status);
        if (status == 0)
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

        if (combat.hand.Count <= 3)
        {
            return;
        }

        const int YOffset = 85;

        int handCount = combat.hand.Count;
        int handPosition = combat.hand.FindIndex(c => c.uuid == __instance.uuid);

        if (handPosition == 1)
        {
            Rect leftSecondRect = __instance.GetScreenRect() + __instance.pos + new Vec(0, __instance.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_second.Sprite, leftSecondRect.x - 8, leftSecondRect.y + YOffset);
        }
        else if (handPosition == handCount - 2)
        {
            Rect rightSecondRect = __instance.GetScreenRect() + __instance.pos + new Vec(0, __instance.hoverAnim * -2.0);
            Draw.Sprite(Sprites.icons_polarity_push_orange_second.Sprite, rightSecondRect.x + rightSecondRect.w - 1, rightSecondRect.y + YOffset);
        }
    }
}
