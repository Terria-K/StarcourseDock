using System;
using System.Collections.Generic;

namespace Shockah.Kokoro;

public partial interface IKokoroApi
{
    IV2 V2 { get; }

    public partial interface IV2
    {
        public interface IKokoroV2ApiHook;
    }
}
