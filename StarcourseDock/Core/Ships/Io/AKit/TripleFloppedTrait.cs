using Microsoft.Xna.Framework;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class TripleFloppedTrait : IKokoroApi.IV2.ICardRenderingApi.IHook
{
    public Matrix ModifyNonTextCardRenderMatrix(IKokoroApi.IV2.ICardRenderingApi.IHook.IModifyNonTextCardRenderMatrixArgs args) => Matrix.Identity;
    
    public Matrix ModifyCardActionRenderMatrix(IKokoroApi.IV2.ICardRenderingApi.IHook.IModifyCardActionRenderMatrixArgs args)
    {
		var g = args.G;
		var actions = args.Actions;

		if (args.Card is IAmFloppableThreeTimesAndFlippable)
		{
			int spacing = 9 * g.mg.PIX_SCALE;
			int index = actions.ToList().IndexOf(args.Action);
			return Matrix.CreateTranslation(0, (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0);
		}

		return Matrix.Identity;
    }
}