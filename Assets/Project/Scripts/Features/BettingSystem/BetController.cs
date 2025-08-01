using System;
using System.Collections.Generic;
using Game.Core.Area;
using RouletteGame.Interfaces;
using UnityEngine;

namespace RouletteGame.Models
{
    [Serializable]
    class BetModel
    {
        private string ID;
        float multiplier;
    }
    public class BetController : MonoBehaviour
    {
        ChipManager _chipManager;
        IBettingSystem _bettingSystem;
        [SerializeField] AreaSelector areaSelector;
        [SerializeField] BetModel[] bets;
        public void Initialize(ChipManager chipManager, IBettingSystem bettingSystem)
        {
            _chipManager = chipManager;
            _bettingSystem = bettingSystem;
            Debug.Log("init");
        }
        private void OnEnable()
        {
            areaSelector.OnSelectArea += AreaSelectorOnSelectArea;
        }
        private void OnDisable()
        {
            areaSelector.OnSelectArea -= AreaSelectorOnSelectArea;
        }
        private void AreaSelectorOnSelectArea(string signal)
        {
            List<int> coveredNumbers = new List<int>();
            if (signal[0] == '1')
            {
                _bettingSystem.AddBet(new StraightBet(_chipManager.SelectedChipValue,coveredNumbers,2,"Straight Bet"));
            }
        }
    }
}