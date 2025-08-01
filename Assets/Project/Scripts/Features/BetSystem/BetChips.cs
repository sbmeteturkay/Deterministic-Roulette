using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BetSystem
{
    [Serializable]
    public class BetChips
    {
        [SerializeField]private Button betButtonUI;
        public GameObject chipPrefab;
        public int betAmount;
    }
}