using System;
using Random = UnityEngine.Random;

namespace Src.Scripts.Utils
{
    public static class RandomUtils
    {
        public static void RunRandomWithSeed(Action fn, int seed = 10)
        {
            var oldState = Random.state;
            Random.InitState(seed);
            fn();
            Random.state = oldState;
        }

    }
}