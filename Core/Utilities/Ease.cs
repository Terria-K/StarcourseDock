using Microsoft.Xna.Framework;

namespace Teuria.Utilities;

public static class Ease 
{
    public delegate double Easer(double t);

    public static readonly Easer Linear = (double t) => t;

    public static readonly Easer SineIn = (double t) => -(double)Math.Cos(MathHelper.PiOver2 * t) + 1;
    public static readonly Easer SineOut = (double t) => (double)Math.Sin(MathHelper.PiOver2 * t);
    public static readonly Easer SineInOut = (double t) => -(double)Math.Cos(MathHelper.Pi * t) / 2f + .5f;

    public static readonly Easer QuadIn = (double t) => t * t;
    public static readonly Easer QuadOut = Invert(QuadIn);
    public static readonly Easer QuadInOut = Follow(QuadIn, QuadOut);

    public static readonly Easer CubeIn = (double t) => t * t * t;
    public static readonly Easer CubeOut = Invert(CubeIn);
    public static readonly Easer CubeInOut = Follow(CubeIn, CubeOut);

    public static readonly Easer QuintIn = (double t) => t * t * t * t * t;
    public static readonly Easer QuintOut = Invert(QuintIn);
    public static readonly Easer QuintInOut = Follow(QuintIn, QuintOut);

    public static readonly Easer ExpoIn = (double t) => (double)Math.Pow(2, 10 * (t - 1));
    public static readonly Easer ExpoOut = Invert(ExpoIn);
    public static readonly Easer ExpoInOut = Follow(ExpoIn, ExpoOut);

    public static readonly Easer BackIn = (double t) => t * t * (2.70158f * t - 1.70158f);
    public static readonly Easer BackOut = Invert(BackIn);
    public static readonly Easer BackInOut = Follow(BackIn, BackOut);

    public static readonly Easer BigBackIn = (double t) => { return t * t * (4f * t - 3f); };
    public static readonly Easer BigBackOut = Invert(BigBackIn);
    public static readonly Easer BigBackInOut = Follow(BigBackIn, BigBackOut);

    public static readonly Easer ElasticIn = (double x) => {
        double x2 = x * x;
        return x2 * x2 * (double)Math.Sin(x * MathHelper.Pi * 4.5f);
    };

    public static readonly Easer ElasticOut = (double x) => {
        double x2 = (x - 1f) * (x - 1f);
        return 1f - x2 * x2 * (double)Math.Cos(x * MathHelper.Pi * 4.5f);
    };

    public static readonly Easer ElasticInOut = (double x) => {
        double x2;
        if (x < 0.45) {
            x2 = x * x;
            return 8f * x2 * x2 * (double)Math.Sin(x * 28.27433466f);
        } else if (x < 0.55) {
            return 0.5f + 0.75f * (double)Math.Sin(x * 12.56637096f);
        } else {
            x2 = (x - 1f) * (x - 1f);
            return 1f - 8f * x2 * x2 * (double)Math.Sin(x * 28.27433466f);
        }
    };

    private const double B1 = 1f / 2.75f;
    private const double B2 = 2f / 2.75f;
    private const double B3 = 1.5f / 2.75f;
    private const double B4 = 2.5f / 2.75f;
    private const double B5 = 2.25f / 2.75f;
    private const double B6 = 2.625f / 2.75f;

    public static readonly Easer BounceIn = (double t) =>
    {
        t = 1 - t;
        return t switch 
        {
            < B1 => 1 - 7.5625f * t * t,
            < B2 => 1 - (7.5625f * (t - B3) * (t - B3) + .75f),
            < B4 => 1 - (7.5625f * (t - B5) * (t - B5) + .9375f),
            _ => 1 - (7.5625f * (t - B6) * (t - B6) + .984375f)
        };
    };

    public static readonly Easer BounceOut = (double t) =>
    {
        return t switch 
        {
            < B1 => 7.5625f * t * t,
            < B2 => 7.5625f * (t - B3) * (t - B3) + .75f,
            < B4 => 7.5625f * (t - B5) * (t - B5) + .9375f,
            _ => 7.5625f * (t - B6) * (t - B6) + .984375f
        };
    };

    public static readonly Easer BounceInOut = (double t) =>
    {
        if (t < .5f)
        {
            t = 1 - t * 2;
            return t switch 
            {
                < B1 => (1 - 7.5625f * t * t) / 2,
                < B2 => (1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2,
                < B4 => (1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2,
                _ => (1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2
            };
        }
        t = t * 2 - 1;
        return t switch 
        {
            < B1 => (7.5625f * t * t) / 2 + .5f,
            < B2 => (7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f,
            < B4 => (7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f,
            _ => (7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f
        };
    };

    public static Easer Invert(Easer easer)
    {
        return (double t) => { return 1 - easer(1 - t); };
    }

    public static Easer Follow(Easer first, Easer second)
    {
        return (double t) => { 
            if (t <= 0.5f) 
            {
                return first(t * 2) / 2;
            }
            return second(t * 2 - 1) / 2 + 0.5f; 
        };
    }

    public static double UpDown(double eased)
    {
        if (eased <= .5f)
            return eased * 2;
        return 1 - (eased - .5f) * 2;

    }
}