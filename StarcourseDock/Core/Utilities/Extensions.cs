using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nickel;
using Teuria.StarcourseDock;

namespace Teuria.Utilities;

internal static class ArtifactExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetArtifact<T>(this State s)
        where T : Artifact
    {
        var spanArtifacts = CollectionsMarshal.AsSpan(s.EnumerateAllArtifacts());
        for (int i = 0; i < spanArtifacts.Length; i++)
        {
            var artifact = spanArtifacts[i];
            if (artifact is T art)
            {
                return art;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasArtifact<T>(this State s)
        where T : Artifact
    {
        var spanArtifacts = CollectionsMarshal.AsSpan(s.EnumerateAllArtifacts());
        for (int i = 0; i < spanArtifacts.Length; i++)
        {
            if (spanArtifacts[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetArtifactFromColorless<T>(this State s)
        where T : Artifact
    {
        var spanArtifacts = CollectionsMarshal.AsSpan(s.artifacts);
        for (int i = 0; i < spanArtifacts.Length; i++)
        {
            var artifact = spanArtifacts[i];
            if (artifact is T art)
            {
                return art;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasArtifactFromColorless<T>(this State s)
        where T : Artifact
    {
        var spanArtifacts = CollectionsMarshal.AsSpan(s.artifacts);
        for (int i = 0; i < spanArtifacts.Length; i++)
        {
            if (spanArtifacts[i] is T)
            {
                return true;
            }
        }

        return false;
    }
}

internal static class CardExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasCardOnHand<T>(this Combat combat)
    {
        var spanHand = CollectionsMarshal.AsSpan(combat.hand);
        for (int i = 0; i < spanHand.Length; i++)
        {
            if (spanHand[i] is T)
            {
                return true;
            }
        }
        return false;
    }
}

internal static class ShipExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartExists(this Ship ship, string key)
    {
        var spanParts = CollectionsMarshal.AsSpan(ship.parts);
        for (int i = 0; i < spanParts.Length; i++)
        {
            if (spanParts[i].key == key)
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartExists(this Ship ship, PType type)
    {
        var spanParts = CollectionsMarshal.AsSpan(ship.parts);
        for (int i = 0; i < spanParts.Length; i++)
        {
            if (spanParts[i].type == type)
            {
                return true;
            }
        }
        return false;
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
        var spanParts = CollectionsMarshal.AsSpan(ship.parts);
        for (int i = 0; i < spanParts.Length; i++)
        {
            if (predicate(spanParts[i]))
            {
                return i;
            }
        }
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Part? GetPartByKey(this Ship ship, string key)
    {
        var spanParts = CollectionsMarshal.AsSpan(ship.parts);
        for (int i = 0; i < spanParts.Length; i++)
        {
            var part = spanParts[i];
            if (part.key == key)
            {
                return part;
            }
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<Part> RetainParts(this Ship ship, Func<Part, bool> predicate)
    {
        var retainedParts = new List<Part>();
        var spanParts = CollectionsMarshal.AsSpan(ship.parts);
        for (int i = 0; i < spanParts.Length; i++)
        {
            var part = spanParts[i];
            if (predicate(part))
            {
                retainedParts.Add(part);
            }
        }
        return retainedParts;
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
