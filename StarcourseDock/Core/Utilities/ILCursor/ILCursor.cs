using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using HarmonyLib;

namespace Teuria.Utilities;

internal sealed class ILCursor
{
    private readonly List<CodeInstruction> instructions;
    private readonly List<CodeInstruction> resultingInstruction;
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
        resultingInstruction = new List<CodeInstruction>(5);
    }

    public ILCursor Reset()
    {
        Index = 0;
        return this;
    }

    public ILCursor GotoPrev()
    {
        return GotoPrev(MoveType.Before, instrMatches: []);
    }

    public ILCursor GotoNext()
    {
        return GotoNext(MoveType.Before, instrMatches: []);
    }

    public ILCursor GotoPrev(params Span<InstructionMatcher> matches) =>
        GotoPrev(MoveType.Before, matches);

    public ILCursor GotoNext(params Span<InstructionMatcher> matches) =>
        GotoNext(MoveType.Before, matches);

    public ILCursor GotoPrev(MoveType moveType, params Span<InstructionMatcher> instrMatches)
    {
        GotoPrev(moveType, out bool res, instrMatches);
        if (!res)
        {
            throw new Exception("Match not found");
        }
        return this;
    }

    public ILCursor GotoPrev(
        MoveType moveType,
        out bool result,
        params Span<InstructionMatcher> instrMatches
    )
    {
        resultingInstruction.Clear();
        bool found = false;
        var len = instrMatches.Length;
        if (len == 0)
        {
            Index -= 1;
            resultingInstruction.Add(Instruction);
            result = true;
            return this;
        }
        int oldIndex = Index;
        for (; Index >= 0; Index -= 1)
        {
            var setInstructions = instructions[Index..(Index + len)];
            for (int j = 0; j < setInstructions.Count; j += 1)
            {
                var instr = setInstructions[j];
                if (!instrMatches[j].Check(instr))
                {
                    resultingInstruction.Clear();
                    found = false;
                    break;
                }

                resultingInstruction.Add(instr);
                found = true;
            }

            if (found)
            {
                if (moveType == MoveType.After)
                {
                    Index += len;
                }

                result = true;
                return this;
            }
        }

        Index = oldIndex;
        result = false;
        return this;
    }

    public ILCursor GotoNext(MoveType moveType, params Span<InstructionMatcher> instrMatches)
    {
        GotoNext(moveType, out bool res, instrMatches);
        if (!res)
        {
            throw new Exception("Match not found");
        }
        return this;
    }

    public ILCursor GotoNext(
        MoveType moveType,
        out bool result,
        params Span<InstructionMatcher> instrMatches
    )
    {
        resultingInstruction.Clear();
        bool found = false;
        var len = instrMatches.Length;
        if (len == 0)
        {
            Index += 1;
            resultingInstruction.Add(Instruction);
            result = true;
            return this;
        }
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
                if (!instrMatches[j].Check(instr))
                {
                    resultingInstruction.Clear();
                    found = false;
                    break;
                }

                resultingInstruction.Add(instr);
                found = true;
            }

            if (found)
            {
                if (moveType == MoveType.After)
                {
                    Index += len;
                }

                result = true;
                return this;
            }
        }

        Index = oldIndex;
        result = false;
        return this;
    }

    public ILCursor Encompass(Action<ILEncompasser> encompassAction)
    {
        encompassAction(new ILEncompasser(this));
        return this;
    }

    public ILCursor ExtractOperand(int index, out object? obj)
    {
        obj = resultingInstruction[index].operand;
        return this;
    }

    public ILCursor Emit(in OpCode opCodes)
    {
        instructions.Insert(Index, new CodeInstruction(opCodes));
        Index += 1;
        return this;
    }

    public ILCursor Emit(in OpCode opCodes, object? operand)
    {
        instructions.Insert(Index, new CodeInstruction(opCodes, operand));
        Index += 1;
        return this;
    }

    public ILCursor Emit(in CodeInstruction instruction)
    {
        instructions.Insert(Index, instruction);
        Index += 1;
        return this;
    }

    public ILCursor EmitDelegate<T>(T del)
        where T : Delegate
    {
        instructions.Insert(Index, CodeInstruction.CallClosure(del));
        Index += 1;
        return this;
    }

    public ILCursor Emits(ReadOnlySpan<CodeInstruction> instructions)
    {
        this.instructions.InsertRange(Index, instructions);
        Index += instructions.Length;
        return this;
    }

    public Label CreateLabel()
    {
        return il.DefineLabel();
    }

    public void MarkLabel(in Label label)
    {
        MarkLabel(label, Next);
    }

    public Label MarkLabel()
    {
        var label = CreateLabel();
        MarkLabel(label, Next);
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

    public ILCursor LogInstructions()
    {
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];
            stringBuilder.Append(i + "\t");
            stringBuilder.AppendLine(instr.ToString());
        }

        Console.WriteLine(stringBuilder.ToString());
        return this;
    }
}
