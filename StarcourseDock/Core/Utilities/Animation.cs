using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Teuria.StarcourseDock;

namespace Teuria.Utilities;

internal abstract class Animation
{
    private static List<Animation> animations = [];

    public int CurrentFrame { get; private set; }

    public double Timer { get; private set; }
    public double FPS { get; set; } = 0.15f;

    public abstract int MaxFrameLength { get; }

    // must call to ModEntry
    internal static void InitPatch()
    {
        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(State), nameof(State.Update)),
            postfix: new HarmonyMethod(State_Update_Postfix)
        );
    }

    private static void State_Update_Postfix(G g)
    {
        foreach (var animation in animations)
        {
            animation.FrameUpdate(g);
        }
    }

    private void FrameUpdate(G g)
    {
        Timer += g.dt;

        if (Timer <= FPS)
        {
            return;
        }

        CurrentFrame += 1;
        Timer = 0;

        if (CurrentFrame < MaxFrameLength)
        {
            return;
        }

        CurrentFrame = 0;
    }

    public abstract Texture2D Update(G g, State? s);

    public static void AddAnimation(Animation animation)
    {
        animations.Add(animation);
    }
}
