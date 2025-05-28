using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.Utilities;

internal record struct CursorInstruction(CodeInstruction Instruction) 
{
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
}
