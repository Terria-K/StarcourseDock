using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class SiriusShip : IRegisterable
{
	internal static IShipEntry SiriusEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var bay = helper.Content.Ships.RegisterPart("SiriusMissileBay", new()
        {
            Sprite = Sprites.sirius_missilebay.Sprite,
            DisabledSprite = Sprites.sirius_missilebay_inactive.Sprite
        });


        SiriusEntry = helper.Content.Ships.RegisterShip("Sirius", new()
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "description"]).Localize,
            UnderChassisSprite = Sprites.sirius_chassis.Sprite,
            Ship = new()
            {
                ship = new()
                {
                    x = 1,
                    hull = 9,
                    hullMax = 9,
                    shieldMaxBase = 5,
                    parts = [
                        new Part()
                        {
                            type = PType.missiles,
                            skin = bay.UniqueName,
                            damageModifier = PDamMod.armor,
                            damageModifierOverrideWhileActive = PDamMod.none,
                            key = "firstBay"
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("SiriusCockpit", new()
                            {
                                Sprite = Sprites.sirius_cockpit.Sprite
                            }).UniqueName,
                            key = "cockpit"
                        },
                        new Part()
                        {
                            type = PType.comms,
                            skin = helper.Content.Ships.RegisterPart("SiriusComms", new()
                            {
                                Sprite = Sprites.sirius_comms.Sprite
                            }).UniqueName,
                            key = "closeToScaffold",
                            damageModifier = PDamMod.weak
                        },
                        new Part()
                        {
                            type = PType.missiles,
                            skin = bay.UniqueName,
                            key = "weak",
                            damageModifier = PDamMod.armor,
                            damageModifierOverrideWhileActive = PDamMod.none,
                            active = false
                        }
                    ]
                },
                artifacts = [new ShieldPrep(), new SiriusMissileBay()],
                cards = [
                    new SiriusBusiness(),
                    new DodgeColorless(),
                    new BasicShieldColorless(),
                    new DroneshiftColorless(),
                    new CannonColorless()
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts)),
            postfix: new HarmonyMethod(ArtifactReward_GetBlockedArtifacts_Postfix)
        );
    }

    internal static void ArtifactReward_GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s) 
    {
        if (s.ship.key != SiriusEntry.UniqueName)
        {
            __result.Add(typeof(SiriusSubwoofer));
            __result.Add(typeof(SiriusMissileBayV2));
            __result.Add(typeof(SiriusInquisitor));
        }
    }
}