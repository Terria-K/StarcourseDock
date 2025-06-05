using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class DoubleStarPaymentOption : IEvadePaymentOption
{
    public bool CanPayForEvade(IEvadePaymentOption.ICanPayForEvadeArgs args)
    {
        if (!args.State.HasArtifactFromColorless<DoubleStar>())
        {
            return false;
        }
        return args.State.ship.Get(Status.evade) > 0;
    }

    public IReadOnlyList<CardAction> ProvideEvadePaymentActions(
        IEvadePaymentOption.IProvideEvadePaymentActionsArgs args
    )
    {
        if (!args.State.HasArtifactFromColorless<DoubleStar>())
        {
            return [];
        }
        args.State.ship.Add(Status.evade, -1);
        return [];
    }
}
