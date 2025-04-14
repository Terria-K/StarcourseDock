using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;

namespace Teuria.Utilities;

internal sealed class ILCursor
{
    private readonly List<CodeInstruction> instructions;
    private readonly ILGenerator il;

    public IList<CodeInstruction> Instructions => instructions;
    public ILGenerator Context => il;

    public int Index { get; set; }

    public CodeInstruction Instruction => instructions[Index];
    public CodeInstruction Prev => instructions[Index - 1];
    public CodeInstruction Next => instructions[Index + 1];

    public ILCursor(ILGenerator ctx, IEnumerable<CodeInstruction> instructions)
    {
        this.instructions = [.. instructions];
        il = ctx;
    }

    public void GotoPrev(MoveType moveType, params Func<CursorInstruction, bool>[] matches)
    {
        if (!TryGotoPrev(moveType, matches))
        {
            throw new Exception("Matches cannot be found!");
        }
    }

    public void GotoNext(MoveType moveType, params Func<CursorInstruction, bool>[] matches)
    {
        if (!TryGotoNext(moveType, matches))
        {
            throw new Exception("Matches cannot be found!");
        }
    }

    public void GotoPrev(params Func<CursorInstruction, bool>[] matches)
    {
        GotoPrev(MoveType.Before, matches);
    }

    public void GotoNext(params Func<CursorInstruction, bool>[] matches)
    {
        GotoNext(MoveType.Before, matches);
    }

    public bool TryGotoPrev(params Func<CursorInstruction, bool>[] matches)
    {
        return TryGotoPrev(MoveType.Before, matches);
    }

    public bool TryGotoNext(params Func<CursorInstruction, bool>[] matches)
    {
        return TryGotoNext(MoveType.Before, matches);
    }

    public bool TryGotoPrev(MoveType moveType, params Func<CursorInstruction, bool>[] matches)
    {
        bool found = false;
        var len = matches.Length;
        int oldIndex = Index;
        for (; Index >= 0; Index -= 1)
        {
            var setInstructions = instructions[Index..(Index + len)];
            for (int j = 0; j < setInstructions.Count; j += 1)
            {
                var instr = setInstructions[j];
                if (!matches[j](new CursorInstruction(instr)))
                {
                    found = false;
                    break;
                }
                found = true;
            }

            if (found)
            {
                if (moveType == MoveType.After)
                {
                    Index += len;
                }
                return true;
            }
        }

        Index = oldIndex;
        return false;
    }

    public bool TryGotoNext(MoveType moveType, params Func<CursorInstruction, bool>[] matches)
    {
        bool found = false;
        var len = matches.Length;
        int oldIndex = Index;
        for (; Index < instructions.Count; Index += 1)
        {
            if (Index + len >= instructions.Count)
            {
                break;
            }
            var setInstructions = instructions[Index..(Index + len)];
            for (int j = 0; j < setInstructions.Count; j += 1)
            {
                var instr = setInstructions[j];
                if (!matches[j](new CursorInstruction(instr)))
                {
                    found = false;
                    break;
                }
                found = true;
            }

            if (found)
            {
                if (moveType == MoveType.After)
                {
                    Index += len;
                }
                return true;
            }
        }

        Index = oldIndex;
        return false;
    }

    public void Emit(in OpCode opCodes)
    {
        instructions.Insert(Index, new CodeInstruction(opCodes));
        Index += 1;
    }

    public void Emit(in OpCode opCodes, object? operand)
    {
        instructions.Insert(Index, new CodeInstruction(opCodes, operand));
        Index += 1;
    }

    public void EmitDelegate<T>(T del)
    where T : Delegate
    {
        instructions.Insert(Index, CodeInstruction.CallClosure(del));
        Index += 1;
    }

    public Label CreateLabel()
    {
        return il.DefineLabel();
    }

    public void MarkLabel(in Label label)
    {
        MarkLabel(label, Instruction);
    }

    public Label MarkLabel()
    {
        var label = CreateLabel();
        MarkLabel(label, Instruction);
        return label;
    }

    public Label MarkLabel(CodeInstruction instruction)
    {
        var label = CreateLabel();
        MarkLabel(label, instruction);
        return label;
    }

    public Label MarkLabel(in Label label, CodeInstruction instruction)
    {
        instruction.labels.Add(label);
        return label;
    }

    public IEnumerable<CodeInstruction> Generate()
    {
        return instructions;
    }

    public void LogInstructions()
    {
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];
            stringBuilder.Append(i + "\t");
            stringBuilder.AppendLine(instr.ToString());
        }

        Console.WriteLine(stringBuilder.ToString());
    }
}

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
}


internal enum MoveType
{
    Before,
    After
}