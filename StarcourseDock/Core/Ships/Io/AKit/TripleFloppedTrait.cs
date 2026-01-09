using Microsoft.Xna.Framework;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class TripleFloppedTrait : IKokoroApi.IV2.ICardRenderingApi.IHook
{
    public Matrix ModifyNonTextCardRenderMatrix(IKokoroApi.IV2.ICardRenderingApi.IHook.IModifyNonTextCardRenderMatrixArgs args) => Matrix.Identity;
    
    public Matrix ModifyCardActionRenderMatrix(IKokoroApi.IV2.ICardRenderingApi.IHook.IModifyCardActionRenderMatrixArgs args)
    {
		var g = args.G;

		if (args.Card is not IAmFloppableThreeTimes { Active: true })
		{
			return Matrix.Identity;
		}

		var actions = args.Actions;

		var spacing = 12 * g.mg.PIX_SCALE;
		var newXOffset = 12 * g.mg.PIX_SCALE;
		var newYOffset = 10 * g.mg.PIX_SCALE;
		var index = actions.ToList().IndexOf(args.Action);
		return index switch
		{
			0 => Matrix.CreateTranslation(-newXOffset, -newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
			1 => Matrix.CreateTranslation(newXOffset, -newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
			_ => Matrix.Identity
		};
    }
}