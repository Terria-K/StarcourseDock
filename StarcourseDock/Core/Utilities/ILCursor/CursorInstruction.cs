using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.Utilities;

internal record struct CursorInstruction(CodeInstruction instruction)
{
    public CodeInstruction Instruction = instruction;

    public bool Match(OpCode opcode)
    {
        return Instruction.opcode == opcode;
    }

    public bool Match(OpCode opcode, object? operand)
    {
        return Instruction.opcode == opcode && Instruction.operand == operand;
    }

    public bool Match(string instructionStr)
    {
        return Instruction.ToString() == instructionStr;
    }

    public bool MatchContains(string contains)
    {
        return Instruction.ToString().Contains(contains);
    }

    public bool MatchExtract(OpCode opcode, out object? operand)
    {
        if (Instruction.opcode == opcode)
        {
            operand = Instruction.operand;
            return true;
        }
        operand = null;
        return false;
    }

    public bool MatchContainsAndExtract(string contains, out object? operand)
    {
        if (MatchContains(contains))
        {
            operand = Instruction.operand;
            return true;
        }
        operand = null;
        return false;
    }

    public bool MatchStfld(string fieldName)
    {
        return (Instruction.opcode == OpCodes.Stfld)
            && (Instruction.operand as FieldInfo)?.Name == fieldName;
    }

    public bool MatchStfld<T>()
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as FieldInfo;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Stfld) && operand.FieldType.Name == typeName;
    }

    public bool MatchStfld<T>(string fieldName)
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as FieldInfo;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Stfld)
            && operand.Name == fieldName
            && operand.FieldType.Name == typeName;
    }

    public bool MatchLdfld(string fieldName)
    {
        return (Instruction.opcode == OpCodes.Ldfld)
            && (Instruction.operand as FieldInfo)?.Name == fieldName;
    }

    public bool MatchLdfld<T>()
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as FieldInfo;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Ldfld) && operand.FieldType.Name == typeName;
    }

    public bool MatchLdfld<T>(string fieldName)
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as FieldInfo;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Ldfld)
            && operand.Name == fieldName
            && operand.FieldType.Name == typeName;
    }

    public bool MatchCall(string methodName)
    {
        return (Instruction.opcode == OpCodes.Call)
            && (Instruction.operand as MethodBase)?.Name == methodName;
    }

    public bool MatchCall<T>(string methodName)
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as MethodBase;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Call)
            && operand.Name == methodName
            && operand.DeclaringType?.Name == typeName;
    }

    public bool MatchCallvirt(string methodName)
    {
        return (Instruction.opcode == OpCodes.Callvirt)
            && (Instruction.operand as MethodBase)?.Name == methodName;
    }

    public bool MatchCallvirt<T>(string methodName)
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as MethodBase;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Callvirt)
            && operand.Name == methodName
            && operand.DeclaringType?.Name == typeName;
    }

    public bool MatchCallOrCallvirt(string methodName)
    {
        return (Instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt)
            && (Instruction.operand as MethodBase)?.Name == methodName;
    }

    public bool MatchCallOrCallvirt<T>(string methodName)
    {
        string typeName = typeof(T).Name;
        var operand = Instruction.operand as MethodBase;
        if (operand is null)
        {
            return false;
        }

        return (Instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt)
            && operand.Name == methodName
            && operand.DeclaringType?.Name == typeName;
    }

    public bool MatchNewobj(string objName)
    {
        return (Instruction.opcode == OpCodes.Newobj)
            && (Instruction.operand as ConstructorInfo)?.DeclaringType?.Name == objName;
    }

    public bool MatchNewobj<T>()
    {
        return (Instruction.opcode == OpCodes.Newobj)
            && (Instruction.operand as ConstructorInfo)?.DeclaringType == typeof(T);
    }

    public bool MatchNewobj(ConstructorInfo info)
    {
        return (Instruction.opcode == OpCodes.Newobj)
            && (Instruction.operand as ConstructorInfo) == info;
    }
}
