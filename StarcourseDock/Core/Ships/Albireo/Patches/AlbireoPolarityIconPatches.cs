using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlbireoPolarityIconPatches
{
    [HarmonyPatch(typeof(Ship), nameof(Ship.RenderStatusRow)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Ship_RenderStatusRow_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext([
            ILMatch.Ldarg(3),
            ILMatch.LdlocS(),
            ILMatch.Callvirt("get_Item"),
            ILMatch.Stloc().TryGetLocalIndex(out var kv)
        ]);

        cursor.GotoNext(MoveType.After, [ILMatch.Ldfld("color")]);
        cursor.Emit(OpCodes.Ldarg_1);
        cursor.Emit(OpCodes.Ldloc, kv.Value);
        cursor.EmitDelegate((Color color, G g, KeyValuePair<Status, int> kv) =>
        {
            if (kv.Key != Polarity.PolarityStatus.Status)
            {
                return color;
            }

            var deck = g.state.GetArtifactFromColorless<DoubleDeck>();
            if (deck is null)
            {
                return color;
            }

            var status = g.state.ship.Get(Polarity.PolarityStatus.Status);

            if (status < deck.HalfMax)
            {
                return new Color("2f94ff");
            }

            if (status > deck.HalfMax)
            {
                return new Color("ff7c19");
            }

            return Colors.white;
        });

        cursor.GotoNext(MoveType.After, [ILMatch.Ldfld("icon")]);
        cursor.Emit(OpCodes.Ldarg_1);
        cursor.Emit(OpCodes.Ldloc, kv.Value);
        cursor.EmitDelegate((Spr spr, G g, KeyValuePair<Status, int> kv) =>
        {
            if (kv.Key != Polarity.PolarityStatus.Status)
            {
                return spr;
            }


            var deck = g.state.GetArtifactFromColorless<DoubleDeck>();
            if (deck is null)
            {
                return spr;
            }

            var status = g.state.ship.Get(Polarity.PolarityStatus.Status);

            if (status < deck.HalfMax)
            {
                return Sprites.icons_status_polarity_blue.Sprite;
            }

            if (status > deck.HalfMax)
            {
                return Sprites.icons_status_polarity_orange.Sprite;
            }

            return Sprites.icons_status_polarity.Sprite;
        });

        return cursor.Generate();
    }
}
