using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class IoCardRender : IPatchable
{
    [OnPostfix<Card>(nameof(Card.GetAllTooltips))]
    private static void Card_GetAllTooltips_Postfix(Card __instance, State s, bool showCardTraits, ref IEnumerable<Tooltip> __result)
    {
        if (!showCardTraits)
        {
            return;
        }

        if (__instance is IAmFloppableThreeTimes { Active: false })
        {
            return;
        }

        __result = __result.Select(x =>
        {
            if (x is not TTGlossary { key: "cardtrait.floppable"})
            {
                return x;
            }

            var buttonText = PlatformIcons.GetPlatform() switch
            {
                Platform.NX => Loc.T("controller.nx.b"),
                Platform.PS => Loc.T("controller.ps.circle"),
                _ => Loc.T("controller.xbox.b"),
            };

            return new GlossaryTooltip("cardtrait.triadcardfromstarcoursedock")
            {
                Icon = Sprites.icons_triple0.Sprite,
                TitleColor = Colors.cardtrait,
                Title = Localization.Str_ship_Io_cardtrait_TripleCard_name(),
                Description = 
                    PlatformIcons.GetPlatform() == Platform.MouseKeyboard 
                    ? Localization.Str_ship_Io_cardtrait_TripleCard_description_mouse_keyboard()
                    : Localization.Str_ship_Io_cardtrait_TripleCard_description_controller(buttonText)
            };
        });
    }

    [OnTranspiler<Card>(nameof(Card.Render))]
    private static IEnumerable<CodeInstruction> Card_Render_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext([ILMatch.Ldloc().TryGetLocalIndex(out var op), ILMatch.Call("GetDataWithOverrides")]);

        cursor.GotoNext([ILMatch.Ldfld("floppable")]);
        cursor.GotoNext(MoveType.After, [ILMatch.LdcI4((int)StableSpr.icons_floppable)]);
        cursor.Emit(new CodeInstruction(OpCodes.Ldarg_0));
        cursor.Emit(new CodeInstruction(OpCodes.Ldloc, op.Value));
        cursor.EmitDelegate((Spr spr, Card c, State s) =>
        {
            if (c is not IAmFloppableThreeTimes {Active: true} flop)
            {
                return spr;
            }

            return flop.FlipIndex switch
            {
                1 => Sprites.icons_triple1.Sprite,
                2 => Sprites.icons_triple2.Sprite,
                _ => Sprites.icons_triple0.Sprite
            };
        });

        cursor.GotoNext(MoveType.After, [ILMatch.Ldfld("flipped")]);
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(new CodeInstruction(OpCodes.Ldloc, op.Value));
        cursor.EmitDelegate((bool flipped, Card c, State s) =>
        {
            return c is not IAmFloppableThreeTimes {Active: true} && flipped;
        });

        return cursor.Generate();
    }

    [OnPrefix<Card>(nameof(Card.RenderAction))]
    private static bool Card_RenderAction_Prefix(
        G g, 
        State state, 
        CardAction action, 
        bool dontDraw, 
        int shardAvailable, 
        int stunChargeAvailable, 
        int bubbleJuiceAvailable, 
        ref int __result)
    {
        if (action is not AWrapperSpawn spawn)
        {
            return true;
        }
        
		var position = g.Push(rect: new()).rect.xy;
		var initialX = (int)position.x;
        Color? color = action.disabled ? Colors.disabledText : null;

        if (spawn.isLeft)
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_left_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_left_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }
        else if (spawn.isRandom)
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_random_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_random_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }
        else 
        {
            if (!dontDraw)
            {
                Draw.Sprite(Sprites.icons_right_bay_spawn.Sprite, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(Sprites.icons_right_bay_spawn.Sprite)?.Width ?? 0;
            position.x -= 1;
        }

        g.Push(rect: new(position.x - initialX, 0));

        var icon = spawn.GetIcon(state);
        if (icon is {} i)
        {
            if (!dontDraw)
            {
                Draw.Sprite(i.path, position.x, position.y, color: color);
            }
            position.x += SpriteLoader.Get(i.path)?.Width ?? 0;
        }

		g.Pop();

		__result = (int)position.x - initialX;
		g.Pop();

        return false;
    }
}

