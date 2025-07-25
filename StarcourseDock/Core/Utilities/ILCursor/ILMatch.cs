using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Nanoray.Shrike.Harmony;

namespace Teuria.Utilities;

internal static class ILMatch
{
    public static InstructionMatcher Match(OpCode opcode) =>
        new InstructionMatcher((instr) => instr.opcode == opcode);

    public static InstructionMatcher Match(OpCode opcode, object? operand) =>
        new InstructionMatcher((instr) => instr.opcode == opcode && instr.operand == operand);

    public static InstructionMatcher Exact(string str) =>
        new InstructionMatcher((instr) => instr.ToString() == str);

    public static InstructionMatcher Contains(string contains) =>
        new InstructionMatcher((instr) => instr.ToString().Contains(contains));

    public static InstructionMatcher Contains(string contains, StringComparison comparison) =>
        new InstructionMatcher((instr) => instr.ToString().Contains(contains, comparison));

    public static InstructionMatcher LdcI4(int num) =>
        new InstructionMatcher(
            (instr) =>
            {
                switch (num)
                {
                    case 0 when instr.opcode == OpCodes.Ldc_I4_0:
                        return true;
                    case 1 when instr.opcode == OpCodes.Ldc_I4_1:
                        return true;
                    case 2 when instr.opcode == OpCodes.Ldc_I4_2:
                        return true;
                    case 3 when instr.opcode == OpCodes.Ldc_I4_3:
                        return true;
                    case 4 when instr.opcode == OpCodes.Ldc_I4_4:
                        return true;
                    case 5 when instr.opcode == OpCodes.Ldc_I4_5:
                        return true;
                    case 6 when instr.opcode == OpCodes.Ldc_I4_6:
                        return true;
                    case 7 when instr.opcode == OpCodes.Ldc_I4_7:
                        return true;
                    case 8 when instr.opcode == OpCodes.Ldc_I4_8:
                        return true;
                    case -1 when instr.opcode == OpCodes.Ldc_I4_M1:
                        return true;
                    default:
                        return (instr.opcode == OpCodes.Ldc_I4 && (int)instr.operand == num)
                            || (
                                num < byte.MaxValue
                                && instr.opcode == OpCodes.Ldc_I4_S
                                && (
                                    (instr.operand is int v && v == num)
                                    || (instr.operand is sbyte b && b == num)
                                )
                            );
                }
            }
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool StlocMatch(CodeInstruction instr) =>
        instr.opcode == OpCodes.Stloc_S
        || instr.opcode == OpCodes.Stloc
        || instr.opcode == OpCodes.Stloc_0
        || instr.opcode == OpCodes.Stloc_1
        || instr.opcode == OpCodes.Stloc_2
        || instr.opcode == OpCodes.Stloc_3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LdlocMatch(CodeInstruction instr) =>
        instr.opcode == OpCodes.Ldloc_S
        || instr.opcode == OpCodes.Ldloc
        || instr.opcode == OpCodes.Ldloc_0
        || instr.opcode == OpCodes.Ldloc_1
        || instr.opcode == OpCodes.Ldloc_2
        || instr.opcode == OpCodes.Ldloc_3;

    private static bool LdlocaMatch(CodeInstruction instr) =>
        instr.opcode == OpCodes.Ldloca_S || instr.opcode == OpCodes.Ldloca;

    public static InstructionMatcher Ldarg()
    {
        return new InstructionMatcher((instr) => instr.IsLdarg());
    }

    public static InstructionMatcher Ldarg(int n)
    {
        return new InstructionMatcher((instr) => instr.IsLdarg(n));
    }

    public static InstructionMatcher Ldarga()
    {
        return new InstructionMatcher((instr) => instr.IsLdarga());
    }

    public static InstructionMatcher Ldarga(int n)
    {
        return new InstructionMatcher((instr) => instr.IsLdarga(n));
    }

    public static InstructionMatcher Stloc()
    {
        return new InstructionMatcher((instr) => StlocMatch(instr));
    }

    public static InstructionMatcher Stloc(int index)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return StlocMatch(instr)
                    && instr.TryGetLocalIndex(out int localIndex)
                    && localIndex == index;
            }
        );
    }

    public static InstructionMatcher Ldloc()
    {
        return new InstructionMatcher((instr) => LdlocMatch(instr));
    }

    public static InstructionMatcher Ldloc(int index)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return LdlocMatch(instr)
                    && instr.TryGetLocalIndex(out int localIndex)
                    && localIndex == index;
            }
        );
    }

    public static InstructionMatcher LdlocS()
    {
        return new InstructionMatcher((instr) => instr.opcode == OpCodes.Ldloc_S);
    }

    public static InstructionMatcher LdlocS(int index)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return instr.opcode == OpCodes.Ldloc_S
                    && instr.TryGetLocalIndex(out int localIndex)
                    && localIndex == index;
            }
        );
    }

    public static InstructionMatcher Ldloca()
    {
        return new InstructionMatcher((instr) => LdlocaMatch(instr));
    }

    public static InstructionMatcher Ldloca(int index)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return LdlocaMatch(instr)
                    && instr.TryGetLocalIndex(out int localIndex)
                    && localIndex == index;
            }
        );
    }

    public static InstructionMatcher LdlocaS()
    {
        return new InstructionMatcher((instr) => instr.opcode == OpCodes.Ldloca_S);
    }

    public static InstructionMatcher LdlocaS(int index)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return instr.opcode == OpCodes.Ldloca_S
                    && instr.TryGetLocalIndex(out int localIndex)
                    && localIndex == index;
            }
        );
    }

    public static InstructionMatcher Ldstr(string text)
    {
        return new InstructionMatcher(
            (instr) =>
            {
                return instr.opcode == OpCodes.Ldstr && (instr.operand as string) == text;
            }
        );
    }

    public static InstructionMatcher Stfld(string fieldName) =>
        new InstructionMatcher(
            (instr) =>
                instr.opcode == OpCodes.Stfld && (instr.operand as FieldInfo)?.Name == fieldName
        );

    public static InstructionMatcher Stfld<T>() =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as FieldInfo;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Stfld) && operand.FieldType.Name == typeName;
            }
        );

    public static InstructionMatcher Stfld<T>(string fieldName) =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as FieldInfo;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Stfld)
                    && operand.FieldType.Name == typeName
                    && operand.Name == fieldName;
            }
        );

    // public bool MatchContainsAndExtract(string contains, out object? operand)
    // {
    //     if (MatchContains(contains))
    //     {
    //         operand = Instruction.operand;
    //         return true;
    //     }
    //     operand = null;
    //     return false;
    // }

    public static InstructionMatcher Ldfld(string fieldName) =>
        new InstructionMatcher(
            (instr) =>
                (instr.opcode == OpCodes.Ldfld) && (instr.operand as FieldInfo)?.Name == fieldName
        );

    public static InstructionMatcher Ldfld<T>() =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as FieldInfo;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Ldfld) && operand.FieldType.Name == typeName;
            }
        );

    public static InstructionMatcher Ldfld<T>(string fieldName) =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as FieldInfo;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Ldfld)
                    && operand.Name == fieldName
                    && operand.FieldType.Name == typeName;
            }
        );

    public static InstructionMatcher Call(string methodName) =>
        new InstructionMatcher(
            (instr) =>
                instr.opcode == OpCodes.Call && (instr.operand as MethodBase)?.Name == methodName
        );

    public static InstructionMatcher Call<T>(string methodName) =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as MethodBase;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Call)
                    && operand.Name == methodName
                    && operand.DeclaringType?.Name == typeName;
            }
        );

    public static InstructionMatcher Callvirt() =>
        new InstructionMatcher((instr) => instr.opcode == OpCodes.Callvirt);

    public static InstructionMatcher Callvirt(string methodName) =>
        new InstructionMatcher(
            (instr) =>
                instr.opcode == OpCodes.Callvirt
                && (instr.operand as MethodBase)?.Name == methodName
        );

    public static InstructionMatcher Callvirt<T>(string methodName) =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as MethodBase;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Callvirt)
                    && operand.Name == methodName
                    && operand.DeclaringType?.Name == typeName;
            }
        );

    public static InstructionMatcher CallOrCallvirt(string methodName) =>
        new InstructionMatcher(
            (instr) =>
                (instr.opcode == OpCodes.Callvirt || instr.opcode == OpCodes.Callvirt)
                && (instr.operand as MethodBase)?.Name == methodName
        );

    public static InstructionMatcher CallOrCallvirt<T>(string methodName) =>
        new InstructionMatcher(
            (instr) =>
            {
                string typeName = typeof(T).Name;
                var operand = instr.operand as MethodBase;
                if (operand is null)
                {
                    return false;
                }

                return (instr.opcode == OpCodes.Callvirt || instr.opcode == OpCodes.Callvirt)
                    && operand.Name == methodName
                    && operand.DeclaringType?.Name == typeName;
            }
        );

    public static InstructionMatcher Isinst() =>
        new InstructionMatcher((instr) => instr.opcode == OpCodes.Isinst);

    public static InstructionMatcher Isinst<T>() =>
        new InstructionMatcher(
            (instr) => instr.opcode == OpCodes.Isinst && (instr.operand as Type) == typeof(T)
        );

    public static InstructionMatcher Newobj(string objName) =>
        new InstructionMatcher(
            (instr) =>
                instr.opcode == OpCodes.Newobj
                && (instr.operand as ConstructorInfo)?.DeclaringType?.Name == objName
        );

    public static InstructionMatcher Newobj<T>() =>
        new InstructionMatcher(
            (instr) =>
                instr.opcode == OpCodes.Newobj
                && (instr.operand as ConstructorInfo)?.DeclaringType == typeof(T)
        );

    public static InstructionMatcher Newobj(ConstructorInfo info) =>
        new InstructionMatcher(
            (instr) => instr.opcode == OpCodes.Newobj && (instr.operand as ConstructorInfo) == info
        );
}
