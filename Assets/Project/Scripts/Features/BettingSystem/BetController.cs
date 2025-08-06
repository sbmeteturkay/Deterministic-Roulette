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
            areaSelector.OnSelectArea += AreaSelectorOnSelectArea;
            _bettingSystem.OnBetAdded += BettingSystemOnOnBetAdded;
        }

        private void OnDisable()
        {
            areaSelector.OnSelectArea -= AreaSelectorOnSelectArea;
            _bettingSystem.OnBetAdded -= BettingSystemOnOnBetAdded;

        }
        private void BettingSystemOnOnBetAdded(IBet obj)
        {
            //unselect recent winning number after bet
            HoverWinningBetNumber(-1);
        }

        public void HoverWinningBetNumber(int betNumber)
        {
            areaSelector.SetHoveredArea(betNumber);
        }
        private void AreaSelectorOnSelectArea(string signal)
        {
            var bet = SignalBetFactory.CreateBetFromSignal(signal, _chipManager.SelectedChipValue); // 2. sütun
            if (bet != null)
            {
                areaSelector.SetCoveredAreaNumbers(new List<int>(bet.CoveredNumbers));
                _bettingSystem.AddBet(bet);
            }
        }
    }
}