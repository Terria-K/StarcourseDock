using System.Linq;
using Nickel;
using Teuria.StarcourseDock;
using ZLinq;

namespace Teuria.Utilities;

internal static class ArtifactExtensions
{
    public static T? GetArtifact<T>(this State s)
    where T : Artifact
    {
        return s.EnumerateAllArtifacts().AsValueEnumerable().OfType<T>().FirstOrDefault();
    }

    public static bool HasArtifact<T>(this State s)
    where T : Artifact
    {
        return s.EnumerateAllArtifacts().AsValueEnumerable().Any(x => x is T);
    }
}

internal static class CardExtensions
{
    public static bool HasCardOnHand<T>(this Combat combat)
    {
        return combat.hand
            .AsValueEnumerable()
            .OfType<T>()
            .Any();
    }
}

internal static class ShipExtensions
{
    public static bool IsPartExists(this Ship ship, string key)
    {
        return ship.parts
            .AsValueEnumerable()
            .Where(x => x.key == key)
            .Any();
    }

    public static bool IsPartExists(this Ship ship, PType type)
    {
        return ship.parts
            .AsValueEnumerable()
            .Where(x => x.type == type)
            .Any();
    }

    public static int FindPartIndex(this Ship ship, string key)
    {
        return ship.FindPartIndex(x => x.key == key);
    }

    public static int FindPartIndex(this Ship ship, PType type)
    {
        return ship.FindPartIndex(x => x.type == type);
    }

    public static int FindPartIndex(this Ship ship, Func<Part, bool> predicate)
    {
        return ship.parts.AsValueEnumerable()
            .Index()
            .Where(x => predicate(x.Item))
            .Select(x => x.Index)
            .FirstOrDefault();
    }

    public static bool HasPartType(this Ship ship, PType type)
    {
        return ship.parts.AsValueEnumerable()
            .Where(x => x.type == type)
            .Any();
    }

    public static Part? GetPartByKey(this Ship ship, string key)
    {
        return ship.parts.AsValueEnumerable()
            .Where(x => x.key == key)
            .FirstOrDefault();
    }

    public static List<Part> RetainParts(this Ship ship, Func<Part, bool> predicate)
    {
        return ship.parts.AsValueEnumerable().Where(predicate).ToList();
    }
}

internal static class IModSpritesExtensions
{
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