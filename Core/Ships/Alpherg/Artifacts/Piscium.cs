using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal class Piscium : Artifact, IRegisterable
{
    public bool isRight;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Piscium", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.Piscium.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "Piscium", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "Piscium", "description"]).Localize
		});

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(AAttack_Begin_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AVolleyAttackFromAllCannons), nameof(AAttack.Begin)),
            prefix: new HarmonyMethod(AVolleyAttackFromAllCannons_Begin_Prefix)
        );
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        aAttack = null;
    }

    private static AAttack? aAttack;

    internal static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) 
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(
            instr => instr.MatchContains("AVolleyAttackFromAllCannons")
        );

        cursor.GotoPrev();

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_2);
        cursor.EmitDelegate((AAttack __instance, State s) => {
            ModEntry.Instance.Helper.ModData.SetModData(__instance, "piscium.volley", true);
            aAttack = __instance;
        });

        return cursor.Generate();
    }

    public static void AVolleyAttackFromAllCannons_Begin_Prefix(AVolleyAttackFromAllCannons __instance, State s, Combat c)
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__instance.attack, "piscium.volley", out bool data))
        {
            if (data)
            {
                var piscium = s.EnumerateAllArtifacts().Where(x => x is Piscium).Cast<Piscium>().FirstOrDefault();
                if (piscium is null)
                {
                    return;
                }

                piscium.isRight = !piscium.isRight;
                c.QueueImmediate(new ASwapScaffold() { isRight = piscium.isRight });
            }
        }
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        if (aAttack != null)
        {
            return;
        }
        isRight = !isRight;
        combat.QueueImmediate(new ASwapScaffold() { isRight = isRight });
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new ASwapScaffold() { isRight = isRight });
    }
}
