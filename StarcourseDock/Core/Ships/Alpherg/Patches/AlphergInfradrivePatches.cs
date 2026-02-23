using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlphergInfradrivePatches
{
	[HarmonyPatch(typeof(Card), nameof(Card.GetActualDamage))]
	[HarmonyTranspiler]
    internal static IEnumerable<CodeInstruction> Card_GetActualDamage_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        // Shamelessly steals some code
		try
		{
			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.Find([
					ILMatches.Ldloc<int>(originalMethod).CreateLdlocaInstruction(out var ldlocaDamage),
					ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldlocShip),
					ILMatches.LdcI4((int)Status.powerdrive),
					ILMatches.Call("Get"),
					ILMatches.Instruction(OpCodes.Add),
					ILMatches.Stloc<int>(originalMethod),
				])
				.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
					ldlocShip,
					ldlocaDamage,
					new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(Card_GetActualDamage_Transpiler_ModifyDamage))),
				])
				.AllElements();
		}
		catch (Exception ex)
		{
			ModEntry.Instance.Logger!.LogError("Could not patch method {DeclaringType}::{Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod.DeclaringType, originalMethod, ModEntry.Instance.Package.Manifest.UniqueName, ex);
			return instructions;
		}
    }

    private static void Card_GetActualDamage_Transpiler_ModifyDamage(Ship ship, ref int damage)
    {
        damage -= ship.Get(InfradriveStatus.Infradrive.Status);
    }
}
