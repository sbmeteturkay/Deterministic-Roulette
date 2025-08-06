using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Area;
using RouletteGame.Interfaces;
using UnityEngine;

namespace RouletteGame.Models
{
    /// <summary>
    /// Bahis sistemini yöneten sınıf
    /// </summary>
    public class BettingSystem : IBettingSystem
    {
        private List<IBet> _activeBets = new();

        public List<IBet> ActiveBets => _activeBets.ToList(); // Defensive copy
        public int TotalBetAmount => _activeBets.Sum(bet => bet.BetAmount);

        public event Action<IBet> OnBetAdded;
        public event Action<IBet> OnBetRemoved;
        public event Action OnAllBetsCleared;

        /// <summary>
        /// Bahis ekler
        /// </summary>
        /// <param name="bet">Eklenecek bahis</param>
        /// <returns>Bahis başarıyla eklendi mi</returns>
        public bool AddBet(IBet bet)
        {
            if (bet == null)
            {
                UnityEngine.Debug.LogError("Null bahis eklenemez!");
                return false;
            }

            if (bet.BetAmount <= 0)
            {
                UnityEngine.Debug.LogError("Bahis miktarı 0'dan büyük olmalı!");
                return false;
            }

            _activeBets.Add(bet);
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

    }
}

