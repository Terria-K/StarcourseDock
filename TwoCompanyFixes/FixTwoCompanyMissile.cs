using Nanoray.PluginManager;
using Nanoray.PluginManager.Cecil;
using Mono.Cecil;
using System.Text;
using MonoMod.Utils;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Teuria.TwoCompanyFixes;

internal sealed class FixTwoCompanyMissile : IAssemblyDefinitionEditor
{
    public byte[] AssemblyEditorDescriptor =>
        Encoding.UTF8.GetBytes($"{GetType().FullName}, {ModEntry.Instance.Package.Manifest.UniqueName} { ModEntry.Instance.Package.Manifest.Version}");

    public bool EditAssemblyDefinition(AssemblyDefinition definition, Action<AssemblyEditorResult.Message> logger)
    {
        var ccRef = definition.MainModule.AssemblyReferences.First(x => x.Name.Contains("CobaltCore") && !x.Name.Contains("Definitions"));
        var cobaltCore = definition.MainModule.AssemblyResolver.Resolve(ccRef);

        var twoCompany = definition.MainModule;

        // get the StuffBase and Missile type ref
        var stuffBase = twoCompany.ImportReference(cobaltCore.MainModule.GetType("StuffBase"));
        var missile = twoCompany.ImportReference(cobaltCore.MainModule.GetType("Missile"));

        // create a method to just finess our life

        var getMissile = CreateGetMissile(definition, cobaltCore, stuffBase, missile);


        var patchLogic = definition.MainModule.GetType("TwosCompany.PatchLogic");
        patchLogic.Methods.Add(getMissile);
        var missileHitBegin = patchLogic.FindMethod("MissileHitBegin")!;

        var ctx = new ILContext(missileHitBegin);

        ctx.Invoke((manip) =>
        {
            var cursor = new ILCursor(manip);
            var miss = cursor.CreateLocal(missile);

            cursor.Instrs[0] = Mono.Cecil.Cil.Instruction.Create(OpCodes.Ldloc, miss);
            cursor.Instrs[1].Operand = twoCompany.ImportReference(cobaltCore.MainModule.GetType("StuffBase").FindField("targetPlayer"));

            var cont = cursor.DefineLabel();

            cursor.Index = 0;

            cursor.EmitLdarg0();
            cursor.EmitLdarg2();
            cursor.EmitCall(getMissile);
            cursor.EmitStloc(miss);

            cursor.EmitLdloc(miss);
            cursor.EmitBrtrue(cont);
            cursor.EmitLdcI4(1);
            cursor.EmitRet();
            cursor.MarkLabel(cont);
        });

        var missileHitEnd = patchLogic.FindMethod("MissileHitEnd")!;

        ctx = new ILContext(missileHitEnd);

        ctx.Invoke((manip) =>
        {
            var cursor = new ILCursor(manip);
            var miss = cursor.CreateLocal(missile);

            while (cursor.TryFindNext(
                out var sors,
                instr => instr.MatchLdarg0(),
                instr => instr.MatchLdfld("AMissileHit", "targetPlayer"))
            )
            {
                sors[0].Next!.OpCode = OpCodes.Ldloc; 
                sors[0].Next!.Operand = miss;
                sors[1].Next!.Operand = twoCompany.ImportReference(cobaltCore.MainModule.GetType("StuffBase").FindField("targetPlayer"));
            }
            cursor.Index = 0;

            var cont = cursor.DefineLabel();

            cursor.EmitLdarg0();
            cursor.EmitLdarg2();
            cursor.EmitCall(getMissile);
            cursor.EmitStloc(miss);

            cursor.EmitLdloc(miss);
            cursor.EmitBrtrue(cont);
            cursor.EmitRet();
            cursor.MarkLabel(cont);
        });

        return true;
    }

    public bool WillEditAssembly(string fileBaseName)
    {
        return fileBaseName == "TwosCompany";
    }

    private MethodDefinition CreateGetMissile(AssemblyDefinition tc, AssemblyDefinition cc, TypeReference stuffBase, TypeReference missile)
    {
        // Missile GetMissile(missilehit, combat)

        // [l_stuff] :: locals_0
        // [l_stuff_casted] :: locals_1

        // ldarg.1 (combat)
        // ldfld Combat::stuff
        // ldarg.0 (missilehit)
        // ldfld AMissileHit::worldX
        // ldloca [l_stuff]
        // callvirt System.Collections.Generic.Dictionary`2<System.Int32,StuffBase>::TryGetValue
        // pop
        // ldloc [l_stuff]
        // isinst Missile
        // stloc [l_stuff_casted]
        // ldloc [l_stuff_casted]
        // ret

        var methodDefinition = new MethodDefinition(
            "<Teuria>__<GetMissile>", 
            MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig, 
            missile);
        
        var missileHitType = tc.MainModule.ImportReference(cc.MainModule.GetType("AMissileHit"));
        var combatType = tc.MainModule.ImportReference(cc.MainModule.GetType("Combat"));

        methodDefinition.Parameters.Add(new ParameterDefinition(missileHitType));
        methodDefinition.Parameters.Add(new ParameterDefinition(combatType));

        var context = new ILContext(methodDefinition);

        context.Invoke((ctx) =>
        {
            var stuffB = ctx.CreateLocal(stuffBase);
            var miss = ctx.CreateLocal(missile);

            var stuff = tc.MainModule.ImportReference(cc.MainModule.GetType("Combat").FindField("stuff"));
            var worldX = tc.MainModule.ImportReference(cc.MainModule.GetType("AMissileHit").FindField("worldX"));

            var dictType = tc.MainModule.ImportReference(stuff.FieldType);

            var tryGetValueRef = tc.MainModule.ImportReference(dictType.Resolve().FindMethod("TryGetValue"));
            tryGetValueRef.DeclaringType = dictType;

            var cursor = new ILCursor(ctx);

            cursor.EmitLdarg1();
            cursor.EmitLdfld(stuff);

            cursor.EmitLdarg0();
            cursor.EmitLdfld(worldX);

            cursor.EmitLdloca(stuffB);
            cursor.EmitCallvirt(tryGetValueRef);

            cursor.EmitPop();

            cursor.EmitLdloc(stuffB);
            cursor.EmitIsinst(missile);
            cursor.EmitStloc(miss);

            cursor.EmitLdloc(miss);
            cursor.EmitRet();
        });

        return methodDefinition;
    }
}