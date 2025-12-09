using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nickel;

namespace Teuria.Utilities;

public readonly struct AddScaffoldConfiguration
{
    public required IPartEntry Part { get; init; }
}

internal static class ShipEntryExtensions
{
    extension(IShipEntry entry)
    {
        public void RegisterAddScaffold(AddScaffoldConfiguration configuration)
        {
            AddScaffoldManager.Add(entry.UniqueName, configuration.Part);
        }
    }
}

internal static class AddScaffoldManager
{
    public static Dictionary<string, IPartEntry> Scaffolds = [];

    public static void Add(string shipName, IPartEntry spriteEntry)
    {
        Scaffolds.Add(shipName, spriteEntry);
    }
}

internal static class ArtifactExtensions
{
    extension<T>(State s)
        where T : Artifact
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? GetArtifact()
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
        public bool HasArtifact()
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
        public T? GetArtifactFromColorless()
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
        public bool HasArtifactFromColorless()
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
}

internal static class CardExtensions
{
    extension (Combat combat)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCardOnHand<T>()
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
}

internal static class ShipExtensions
{
    extension (List<Part> parts)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Part? GetPartByKey(string key)
        {
            var spanParts = CollectionsMarshal.AsSpan(parts);
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
    }

    extension (Ship ship)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPartExists(string key)
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
        public bool IsPartExists(PType type)
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
        public int FindPartIndex(string key)
        {
            return ship.FindPartIndex(x => x.key == key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindPartIndex(PType type)
        {
            return ship.FindPartIndex(x => x.type == type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindPartIndex(Func<Part, bool> predicate)
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
        public Part? GetPartByKey(string key)
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
        public List<Part> RetainParts(Func<Part, bool> predicate)
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
