using HarmonyLib;

namespace Teuria.Utilities;

internal struct InstructionMatcher(Func<CodeInstruction, bool> predicate)
{
    private Func<CodeInstruction, bool> predicate = predicate;

    public bool Check(CodeInstruction instr) => predicate(instr);
}
