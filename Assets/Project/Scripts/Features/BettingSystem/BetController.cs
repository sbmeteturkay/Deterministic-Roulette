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
            var bet = RouletteSignalParser.ParseSignal(signal);
            if (bet.Category == "Straight")
            {
                _bettingSystem.AddBet(new StraightBet(_chipManager.SelectedChipValue,bet.CoveredNumbers,bet.Multiplier, bet.BetType));
            }
        }
    }
}