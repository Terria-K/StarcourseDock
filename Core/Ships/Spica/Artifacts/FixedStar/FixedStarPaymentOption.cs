using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class FixedStarPaymentOption : IEvadePaymentOption
{
    public bool CanPayForEvade(IEvadePaymentOption.ICanPayForEvadeArgs args)
    {
        if (!args.State.EnumerateAllArtifacts().Any(x => x is FixedStar))
        {
            return false;
        }
        return args.State.ship.Get(Status.evade) > 0;
    }

    public IReadOnlyList<CardAction> ProvideEvadePaymentActions(IEvadePaymentOption.IProvideEvadePaymentActionsArgs args)
    {
        if (!args.State.EnumerateAllArtifacts().Any(x => x is FixedStar))
        {
            return [];
        }
        args.State.ship.Add(Status.evade, -1);
        return [];
    }
}