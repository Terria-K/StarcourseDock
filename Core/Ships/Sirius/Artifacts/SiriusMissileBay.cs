using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class SiriusMissileBay : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("SiriusMissileBay", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.EventOnly],
                unremovable = true,
            },
            Sprite = Sprites.SiriusMissileBay.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusMissileBay", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusMissileBay", "description"]).Localize
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            new HarmonyMethod(AAttack_Begin_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(ASpawn), nameof(ASpawn.Begin)),
            new HarmonyMethod(ASpawn_Begin_Prefix)
        );

        ModEntry.Instance.Harmony.PatchVirtual(
            AccessTools.DeclaredMethod(typeof(StuffBase), nameof(StuffBase.GetTooltips)),
            postfix: new HarmonyMethod(StuffBase_GetTooltips_Postfix)
        );

        ModEntry.Instance.Harmony.PatchVirtual(
            AccessTools.DeclaredMethod(typeof(StuffBase), nameof(StuffBase.Render)),
            postfix: new HarmonyMethod(StuffBase_Render_Postfix)
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (!combat.HasCardOnHand<ToggleMissileBay>())
        {
            combat.Queue(new AAddCard
            {
                card = new ToggleMissileBay(),
                destination = CardDestination.Hand
            });
        }
    }

    private static void ASpawn_Begin_Prefix(ASpawn __instance, State s, Combat c)
    {
        if (s.ship.Get(BayPowerDownStatus.BayPowerDownEntry.Status) > 0)
        {
            ModEntry.Instance.Helper.ModData.SetModData(__instance.thing, "powerdown", true);
            s.ship.Add(BayPowerDownStatus.BayPowerDownEntry.Status, -1);
        }
    }

    private static void StuffBase_Render_Postfix(StuffBase __instance, G g, Vec v)
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__instance, "powerdown", out bool data))
        {
            if (data)
            {
                var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
                Vec offset = v + __instance.GetOffset(g);
                Draw.Sprite(Sprites.power_down.Sprite, offset.x + 7, offset.y + 16, color: color);
            }
        }
    }

    private static void StuffBase_GetTooltips_Postfix(StuffBase __instance, ref List<Tooltip> __result)
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__instance, "powerdown", out bool data))
        {
            if (data)
            {
                __result.Add(PowerDownTooltip());
            }
        }
    }

    private static void AAttack_Begin_Prefix(AAttack __instance, State s, Combat c)
    {
        if (__instance.fromDroneX != null)
        {
            if (!c.stuff.TryGetValue(__instance.fromDroneX.Value, out StuffBase? midrow))
            {
                return;
            }

            if (ModEntry.Instance.Helper.ModData.TryGetModData(midrow, "powerdown", out bool data))
            {
                if (data)
                {
                    __instance.damage -= 1;
                }
            }
        }
    }

    private static Tooltip PowerDownTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::powerDown")
        {
            Title = ModEntry.Instance.Localizations.Localize(["ship", "Sirius", "icon", "powerDown", "name"]),
            TitleColor = Colors.midrow,
            Description = ModEntry.Instance.Localizations.Localize(["ship", "Sirius", "icon", "powerDown", "description"]),
            Icon = Sprites.power_down.Sprite
        };
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new ToggleMissileBay() },
            PowerDownTooltip()
        ];
    }
}
