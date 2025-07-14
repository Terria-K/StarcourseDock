using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;
using FSPRO;
using System.Runtime.InteropServices;

namespace Teuria.StarcourseDock;

internal sealed class PolarityWings : Artifact, IRegisterable
{
    private WeakReference<State>? state;
    public bool isOrange;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "PolarityWings",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_PolarityWings.Sprite,
                Name = Localization.ship_Albireo_artifact_PolarityWings_name(),
                Description = Localization.ship_Albireo_artifact_PolarityWings_description()
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            Polarity.GetTooltip()
        ];
    }

    public override Spr GetSprite()
    {
        if (state is not null && state.TryGetTarget(out State? s) && s is not null)
        {
            if (Polarity.IsOrangePolarity(s))
            {
                isOrange = true;
                return Sprites.artifacts_PolarityWings_Orange.Sprite;
            }
            isOrange = false;
            return Sprites.artifacts_PolarityWings.Sprite;
        }

        if (isOrange)
        {
            return Sprites.artifacts_PolarityWings_Orange.Sprite;
        }

        return Sprites.artifacts_PolarityWings.Sprite;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        this.state ??= new WeakReference<State>(state);
        PutWingsBackToNormal(state);
    }

    public override void OnCombatEnd(State state)
    {
        PutWingsBackToNormal(state);
    }

    private void PutWingsBackToNormal(State state)
    {
        var parts = CollectionsMarshal.AsSpan(state.ship.parts);

        bool changedPart = false;

        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part.skin == AlbireoShip.AlbireoEmptyOrange.UniqueName)
            {
                part.type = PType.wing;
                part.skin = AlbireoShip.AlbireoWingsOrange.UniqueName;
                changedPart = true;
                continue;
            }
            if (part.skin == AlbireoShip.AlbireoEmptyBlue.UniqueName)
            {
                part.type = PType.wing;
                part.skin = AlbireoShip.AlbireoWingsBlue.UniqueName;
                changedPart = true;
            }
        }

        if (changedPart)
        {
            Audio.Play(Event.TogglePart);
        }
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        Pulse();
        if (combat.energy > 0)
        {
            var parts = CollectionsMarshal.AsSpan(state.ship.parts);

            bool changedPart = false;

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part.skin == AlbireoShip.AlbireoWingsOrange.UniqueName)
                {
                    part.type = PType.empty;
                    part.skin = AlbireoShip.AlbireoEmptyOrange.UniqueName;
                    changedPart = true;
                    continue;
                }
                if (part.skin == AlbireoShip.AlbireoWingsBlue.UniqueName)
                {
                    part.type = PType.empty;
                    part.skin = AlbireoShip.AlbireoEmptyBlue.UniqueName;
                    changedPart = true;
                }
            }

            if (changedPart)
            {
                Audio.Play(Event.TogglePart);
            }

            return;
        }

        bool isOrange = Polarity.IsOrangePolarity(state);

        string targetWing;
        string skin;

        if (isOrange)
        {
            targetWing = "orange_wing";
            skin = AlbireoShip.AlbireoEmptyOrange.UniqueName;
        }
        else
        {
            targetWing = "blue_wing";
            skin = AlbireoShip.AlbireoEmptyBlue.UniqueName;
        }

        var spart = state.ship.GetPart(targetWing);
        spart.type = PType.empty;
        spart.skin = skin;

        Audio.Play(Event.TogglePart);
    }
}