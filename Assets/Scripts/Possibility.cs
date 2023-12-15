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

            float randomPossibility = UnityEngine.Random.Range(ProcessFailRate - RateRange, ProcessFailRate + RateRange);
            int randNum = (int)UnityEngine.Random.Range(1, Acurracy);
            return (randNum > (randomPossibility *Acurracy));
        }
    }
}
