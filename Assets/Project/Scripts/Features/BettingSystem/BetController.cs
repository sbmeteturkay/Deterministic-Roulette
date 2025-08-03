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
            var bet = SignalBetFactory.CreateBetFromSignal(signal, _chipManager.SelectedChipValue); // 2. sütun
            if (bet != null)
            {
                _bettingSystem.AddBet(bet);
            }
        }
    }
}