using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoCardRenderPatches : IPatchable
{
    [OnPrefix<Card>(nameof(Card.Render))]
    private static void Card_Render_Prefix(Card __instance, G g, Vec? posOverride = null, State? fakeState = null, bool ignoreAnim = false, bool ignoreHover = false, bool hideFace = false, bool hilight = false, bool showRarity = false, bool autoFocus = false, UIKey? keyOverride = null, OnMouseDown? onMouseDown = null, OnMouseDownRight? onMouseDownRight = null, OnInputPhase? onInputPhase = null, double? overrideWidth = null, UIKey? leftHint = null, UIKey? rightHint = null, UIKey? upHint = null, UIKey? downHint = null, int? renderAutopilot = null, bool? forceIsInteractible = null, bool reportTextBoxesForLocTest = false, bool isInCombatHand = false)
    {
        if (__instance.flipAnim <= 0 && __instance.TryGetLinkedCard(out Card? linkedCard))
        {
            double offset = __instance.hoverAnim > 0.0 ? 5 : 0;
            Vec? posOverrideAlt = null;
            if (posOverride != null)
            {
                posOverrideAlt = new Vec(posOverride.Value.x, posOverride.Value.y - (10 + offset));
            }
            var pos = __instance.pos with
            {
                y = __instance.pos.y - (10 + offset)
            };

            linkedCard.drawAnim = __instance.drawAnim;
            linkedCard.hoverAnim = __instance.hoverAnim;
            linkedCard.flipAnim = __instance.flipAnim;
            linkedCard.flopAnim = __instance.flopAnim;
            linkedCard.pos = pos;

            linkedCard.Render(g, posOverrideAlt, fakeState, ignoreAnim, ignoreHover, hideFace, hilight, showRarity, autoFocus, overrideWidth: overrideWidth, forceIsInteractible: false, isInCombatHand: true);
        }
    }
}