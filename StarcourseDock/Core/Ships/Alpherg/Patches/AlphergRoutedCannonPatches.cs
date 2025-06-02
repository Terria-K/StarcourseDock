using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class AlphergRoutedCannonPatches : IPatchable
{
    [OnPrefix<AAttack>(nameof(AAttack.GetTooltips))]
    internal static void AAttack_GetTooltips_Prefix(State s)
    {
        var routedCannon = s.GetArtifact<RoutedCannon>();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        int partX = s.ship.x;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                if (
                    s.route is Combat combat
                    && combat.stuff.TryGetValue(partX, out StuffBase? value)
                )
                {
                    value.hilight = 2;
                }
                p.hilight = true;
            }
            partX++;
        }
    }

    [OnPostfix<AVolleyAttackFromAllCannons>(nameof(AVolleyAttackFromAllCannons.Begin))]
    internal static void AVolleyAttackFromAllCannons_Begin_Postfix(
        AVolleyAttackFromAllCannons __instance,
        State s,
        Combat c
    )
    {
        var routedCannon = s.GetArtifact<RoutedCannon>();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        List<AAttack> listOfAttacks = new List<AAttack>();
        int i = 0;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                __instance.attack.fromX = new int?(i);
                listOfAttacks.Add(Mutil.DeepCopy(__instance.attack));
            }
            i++;
        }

        c.QueueImmediate(listOfAttacks);
    }

    [OnTranspiler<AAttack>(nameof(AAttack.Begin))]
    internal static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt("GetPartTypeCount"));

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_2);
        cursor.EmitDelegate(
            (int x, AAttack aAttack, State s) =>
            {
                if (aAttack.targetPlayer)
                {
                    return x;
                }
                var routedCannon = s.GetArtifact<RoutedCannon>();
                if (routedCannon is null || routedCannon.disabled)
                {
                    return x;
                }

                int cannon = s.ship.GetPartTypeCount(PType.cannon, false);
                int empty = s.ship.GetPartTypeCount(PType.empty, false);

                return cannon + empty;
            }
        );

        return cursor.Generate();
    }
}
