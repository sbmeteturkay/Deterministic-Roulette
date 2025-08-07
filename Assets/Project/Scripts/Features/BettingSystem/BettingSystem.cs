using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Area;
using RouletteGame.Interfaces;
using RouletteGame.Service;
using UnityEngine;

namespace RouletteGame.Models
{
    /// <summary>
    /// Bahis sistemini yöneten sınıf
    /// </summary>
    public class BettingSystem : IBettingSystem, IDisposable
    {
        AreaSelector _areaSelector;
        ChipManager _chipManager;

        private List<IBet> _activeBets = new();

        public List<IBet> ActiveBets => _activeBets.ToList(); // Defensive copy
        public int TotalBetAmount => _activeBets.Sum(bet => bet.BetAmount);

        public event Action<IBet> OnBetAdded;
        public event Action<IBet> OnBetRemoved;
        public event Action OnAllBetsCleared;

        private bool canBet = true;
        

        public bool CanBet
        {
            get => canBet;
            set=>canBet = value;
        }
        public void Initialize(ChipManager chipManager, AreaSelector areaSelector)
        {
            _areaSelector = areaSelector;
            _chipManager = chipManager;
            areaSelector.OnSelectArea += AreaSelectorOnSelectArea;
        }
        /// <summary>
        /// Bahis ekler
        /// </summary>
        /// <param name="bet">Eklenecek bahis</param>
        /// <returns>Bahis başarıyla eklendi mi</returns>
        public bool AddBet(IBet bet)
        {
            if (bet == null)
            {
                Debug.LogError("Null bahis eklenemez!");
                return false;
            }

            if (bet.BetAmount <= 0)
            {
                Debug.LogError("Bahis miktarı 0'dan büyük olmalı!");
                return false;
            }
            //unselect recent winning number after any bet
            HoverWinningBetNumber(-1,false);
            //_areaSelector.SetCoveredAreaNumbers(new List<int>());
            _activeBets.Add(bet);
            ServiceLocator.SoundService.PlaySfx("chip-put");
            OnBetAdded?.Invoke(bet);
            return true;
        }

        /// <summary>
        /// Bahis kaldırır
        /// </summary>
        /// <param name="bet">Kaldırılacak bahis</param>
        /// <returns>Bahis başarıyla kaldırıldı mı</returns>
        public bool RemoveBet(IBet bet)
        {
            if (bet == null)
            {
                UnityEngine.Debug.LogError("Null bahis kaldırılamaz!");
                return false;
            }

            bool removed = _activeBets.Remove(bet);
            if (removed)
            {
                OnBetRemoved?.Invoke(bet);
            }
            return removed;
        }

        /// <summary>
        /// Tüm bahisleri temizler
        /// </summary>
        public void ClearAllBets()
        {
            _activeBets.Clear();
            OnAllBetsCleared?.Invoke();
        }
        private void AreaSelectorOnSelectArea(Area area)
        {
            if(area==null||!canBet)
                return;
            var bet = SignalBetFactory.CreateBetFromSignal(area.GetID(), _chipManager.SelectedChipValue);
            if (bet != null)
            {
                bet.chipPosition = _areaSelector.GetWorldPositionFromArea(area);
                if (bet.CoveredNumbers.Count > 1)
                    _areaSelector.SetCoveredAreaNumbers(new List<int>(bet.CoveredNumbers));
                else
                {
                    ClearCoveredBetNumbers();
                }
                AddBet(bet);
            }
        }
        public void HoverWinningBetNumber(int betNumber,bool win)
        {
            _areaSelector.SetHoveredArea(betNumber,win);
        }

        public void ClearCoveredBetNumbers()
        {
            _areaSelector.SetCoveredAreaNumbers(new List<int>());
        }
        public void Dispose()
        {
            _areaSelector.OnSelectArea-= AreaSelectorOnSelectArea;
        }
    }
}

