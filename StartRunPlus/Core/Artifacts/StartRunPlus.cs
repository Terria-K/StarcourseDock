using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.HotReloader;

namespace Teuria.StartRunPlus;

internal sealed class StartRunPlus : Artifact, IRegisterable
{
    public int plus = 5;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var startRunPlus1Sprite = helper.Content.Sprites.RegisterReloadableSprite(
            "StartRunPlus",
            package.PackageRoot.GetRelativeFile("assets/artifacts/StartRunPlus1.png")
        );

        helper.Content.Artifacts.RegisterArtifact(
            "StartRunPlus",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "StartRunPlus", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "StartRunPlus", "description"]).Localize,
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Sprite = startRunPlus1Sprite.Sprite,
                Meta = new()
                {
                    pools = [ArtifactPool.EventOnly],
                    owner = Deck.colorless,
                    unremovable = true
                }
            }
        );
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        int hullMax = 2 * plus * 2;

        combat.Queue(new AHullMax() { amount = hullMax });
        combat.Queue(new AHeal() { healAmount = hullMax });

        switch (plus)
        {
            case 2:
                combat.QueueImmediate(new AStatus() { statusAmount = 1, status = Status.powerdrive });
                break;
            case 3:
                combat.QueueImmediate(new AStatus() { statusAmount = 2, status = Status.powerdrive });
                break;
            case 4:
                combat.QueueImmediate(new AStatus() { statusAmount = 3, status = Status.powerdrive });
                break;
            case 5:
                combat.QueueImmediate(new AStatus() { statusAmount = 4, status = Status.powerdrive });
                break;
        }
    }

    public override void OnReceiveArtifact(State state)
    {
        int hullMax = state.ship.hpGainFromBossKills * plus * 2;
        state.ship.hullMax += hullMax;
        state.ship.hull += hullMax;
    }

    public static void Patch(IHarmony harmony)
    {
        var populateRun = typeof(State).GetNestedTypes(AccessTools.all)
            .SelectMany(t => t.GetMethods(AccessTools.all))
            .First(m => m.Name.StartsWith("<PopulateRun>")
                && m.ReturnType == typeof(Route));
        
        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(State), nameof(State.PopulateRun)),
            prefix: new HarmonyMethod(PopulateRun_Prefix)
        );

        harmony.Patch(
            populateRun,
            postfix: new HarmonyMethod(PopulateRun_Inside_Postfix)
        );

        harmony.Patch(
            AccessTools.DeclaredConstructor(typeof(IntentAttack)),
            postfix: new HarmonyMethod(IntentAttack_ctor_Postfix)
        );

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Events), nameof(Events.BootSequence)),
            postfix: new HarmonyMethod(Events_BootSequence_Postfix)
        );

        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(RunSummaryRoute), nameof(RunSummaryRoute.LeaveScreen)),
            postfix: new HarmonyMethod(RunSummaryRoute_LeaveScreen_Postfix)
        );
    }

    private static State prefixState = null!;


    private static void PopulateRun_Prefix(State __instance)
    {
        prefixState = __instance;
    }

    private static void PopulateRun_Inside_Postfix()
    {
        prefixState.SendArtifactToChar(new StartRunPlus());
    }

    private static void IntentAttack_ctor_Postfix(IntentAttack __instance)
    {
        if (__instance.multiHit > 1)
        {
            __instance.multiHit += 1;
        }
        else
        {
            __instance.damage += 1;
        }
    }

    private static void Events_BootSequence_Postfix(State s, ref List<Choice> __result)
    {
        if (ModEntry.Instance.Helper.ModData.ContainsModData(s, "srpoffering"))
        {
            return;
        }

        foreach (var choice in __result)
        {
            choice.actions.AddRange(GetActions(s));
        }
    }

    private static void RunSummaryRoute_LeaveScreen_Postfix(G g)
    {
        ModEntry.Instance.Helper.ModData.RemoveModData(g.state, "srpoffering");       
    }

    private static CardAction[] GetActions(State s)
    {
        var startRunPlus = s.GetArtifactFromColorless<StartRunPlus>();
        if (startRunPlus is null)
        {
            return [];
        }

        return startRunPlus.plus switch
        {
            1 =>
            [
                new ABlockArtifactOffering(),
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
            ],
            2 => 
            [
                new ABlockArtifactOffering(),
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },

                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
            ],
            3 => 
            [
                new ABlockArtifactOffering(),
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },

                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
            ],
            4 => 
            [               
                new ABlockArtifactOffering(),
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },

                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
            ],
            _ => 
            [
                new ABlockArtifactOffering(),
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Common] },

                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] },
                new AArtifactOfferingNoTooltip() { amount = 3, limitPools = [ArtifactPool.Boss] }
            ]
        };
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