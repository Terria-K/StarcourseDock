using System;
using System.Collections.Generic;

namespace Shockah.Kokoro;

public partial interface IKokoroApi
{
    IV2 V2 { get; }

    public partial interface IV2
    {
        public interface IKokoroV2ApiHook;

        /// <summary>
        /// A Kokoro wrapper for a custom <see cref="CardAction"/>.
        /// </summary>
        /// <typeparam name="T">The more concrete type of the card action being wrapped.</typeparam>
        public interface ICardAction<out T>
            where T : CardAction
        {
            /// <summary>
            /// Returns the actual usable card action.
            /// </summary>
            T AsCardAction { get; }
        }

        /// <summary>
        /// A Kokoro wrapper for a custom <see cref="Route"/>.
        /// </summary>
        /// <typeparam name="T">The more concrete type of the route being wrapped.</typeparam>
        public interface IRoute<out T>
            where T : Route
        {
            /// <summary>
            /// Returns the actual usable route.
            /// </summary>
            T AsRoute { get; }
        }

        /// <summary>
        /// Allows choosing the priority for an auto-implemented hook (for example, on <see cref="Artifact"/> types).
        /// </summary>
        public interface IHookPriority
        {
            /// <summary>
            /// The priority for the hook. Higher priority hooks are called before lower priority ones. Defaults to <c>0</c>.
            /// </summary>
            double HookPriority { get; }
        }
    }
}
