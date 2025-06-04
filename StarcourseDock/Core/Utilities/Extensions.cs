using System.Linq;
using System.Runtime.CompilerServices;
using Nickel;
using Teuria.StarcourseDock;
using ZLinq;

namespace Teuria.Utilities;

internal static class ArtifactExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetArtifact<T>(this State s)
        where T : Artifact
    {
        return s.EnumerateAllArtifacts().AsValueEnumerable().OfType<T>().FirstOrDefault();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasArtifact<T>(this State s)
        where T : Artifact
    {
        return s.EnumerateAllArtifacts().AsValueEnumerable().Any(x => x is T);
    }
}

internal static class CardExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasCardOnHand<T>(this Combat combat)
    {
        return combat.hand.AsValueEnumerable().OfType<T>().Any();
    }
}

internal static class ShipExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartExists(this Ship ship, string key)
    {
        return ship.parts.AsValueEnumerable().Where(x => x.key == key).Any();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartExists(this Ship ship, PType type)
    {
        return ship.parts.AsValueEnumerable().Where(x => x.type == type).Any();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindPartIndex(this Ship ship, string key)
    {
        return ship.FindPartIndex(x => x.key == key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindPartIndex(this Ship ship, PType type)
    {
        return ship.FindPartIndex(x => x.type == type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindPartIndex(this Ship ship, Func<Part, bool> predicate)
    {
        return ship
            .parts.AsValueEnumerable()
            .Index()
            .Where(x => predicate(x.Item))
            .Select(x => x.Index)
            .FirstOrDefault();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasPartType(this Ship ship, PType type)
    {
        return ship.parts.AsValueEnumerable().Where(x => x.type == type).Any();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Part? GetPartByKey(this Ship ship, string key)
    {
        return ship.parts.AsValueEnumerable().Where(x => x.key == key).FirstOrDefault();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<Part> RetainParts(this Ship ship, Func<Part, bool> predicate)
    {
        return ship.parts.AsValueEnumerable().Where(predicate).ToList();
    }
}

internal static class IModSpritesExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISpriteEntry RegisterAnimation(this IModSprites sprites, Animation animation)
    {
        var spr = sprites.RegisterDynamicSprite(() =>
        {
            var g = MG.inst.g;
            var state = g.state;

            return animation.Update(g, state);
        });
        Animation.AddAnimation(animation);
        return spr;
    }
}

internal static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Wrap(int value, int min, int max)
    {
        int range = max - min;
        if (range == 0)
        {
            return min;
        }

        return min + ((((value - min) % range) + range) % range);
    }
}
