using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Possibility : SerializedMonoBehaviour
    {
        public const float Acurracy = 10000f;

        public static bool isProcessingSuccess(float ProcessFailRate, float RateRange)
        {
            int seed = System.Environment.TickCount;
            UnityEngine.Random.InitState(seed);
            float randomPossibility = UnityEngine.Random.Range((ProcessFailRate - RateRange)* Acurracy, (ProcessFailRate + RateRange) * Acurracy);

            seed = System.Environment.TickCount;
            UnityEngine.Random.InitState(seed);
            int randNum = (int)UnityEngine.Random.Range(1, Acurracy);
            return (randNum > (randomPossibility));
        }

        public static float getRandomFaultyPossiblility()
        {
            int seed = System.Environment.TickCount;
            UnityEngine.Random.InitState(seed);
            int randomPossibility = UnityEngine.Random.Range((int)(Configration.Instance.RandomPossibleRateMin * Acurracy), (int)(Configration.Instance.RandomPossibleRateMax * Acurracy));
            return (float)randomPossibility / Acurracy;
        }
    }
}
