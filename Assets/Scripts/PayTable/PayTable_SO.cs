using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay SO/ Playtables")]
    public class PayTable_SO : ScriptableObject
    {
        [field: SerializeField] public List<int> AllowedAmount { get; private set; }
        [SerializeField] public List<SerializeDict> _paytableCore;
        [field: SerializeField] public AccumulatedJackpotModePoint AccumulatedJackpotModePoint { get; private set; }
        [field: SerializeField] public float RewardPercentForEachMatchingfaceInRubicsCube = 17.75f;

        public Dictionary<string, PaytableCore> PayTableCore { get; private set; }

        private void OnValidate()
        {
            if (PayTableCore == null)
            {
                PayTableCore = new Dictionary<string, PaytableCore>();
            }

            foreach (SerializeDict dict in _paytableCore)
            {
                if (!PayTableCore.ContainsKey(dict.Character))
                    PayTableCore.Add(dict.Character, dict.Paytable);
            }
        }
    }

    [Serializable]
    public class SerializeDict
    {
        public string Character;
        public PaytableCore Paytable;
    }

    [Serializable]
    public class PaytableCore
    {
        public int RewardPercentage;
        public int Timer;
    }

    [Serializable]
    public class AccumulatedJackpotModePoint
    {
        public int Points;
        public List<float> AccumulatedAmount;
    }

    [Serializable]
    public class PaytableData
    {
    }
}
