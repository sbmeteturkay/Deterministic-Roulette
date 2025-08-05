using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RouletteGame.Models
{
    /// <summary>
    /// Oyuncu istatistiklerini tutan model sınıfı
    /// </summary>
    [Serializable]
    public class PlayerStatsModel
    {
        // Değerler
        [SerializeField] private int _spinsPlayed;
        [SerializeField] private float _totalWins;
        [SerializeField] private float _totalLosses;
        [SerializeField] private float _currentProfitLoss;

        // Public özellikler
        public int SpinsPlayed => _spinsPlayed;
        public float TotalWins => _totalWins;
        public float TotalLosses => _totalLosses;
        public float CurrentProfitLoss => _currentProfitLoss;

        // Olay (Event)
        public event Action OnStatsUpdated;

        // Kurucu metot

        public void SaveStats()
        {
            PlayerPrefs.SetInt("SpinsPlayed", _spinsPlayed);
            PlayerPrefs.SetFloat("TotalWins", _totalWins);
            PlayerPrefs.SetFloat("TotalLosses", _totalLosses);
            PlayerPrefs.SetFloat("CurrentProfitLoss", _currentProfitLoss);
            PlayerPrefs.Save();
        }
        public void LoadStats()
        {
            _spinsPlayed = PlayerPrefs.GetInt("SpinsPlayed", 0);
            _totalWins = PlayerPrefs.GetFloat("TotalWins", 0f);
            _totalLosses = PlayerPrefs.GetFloat("TotalLosses", 0f);
            _currentProfitLoss = PlayerPrefs.GetFloat("CurrentProfitLoss", 0f);
        
            // Veriler yüklendikten sonra UI'yi güncellemek için olay tetikle
            OnStatsUpdated?.Invoke();
        }
        /// <summary>
        /// Spin sayısını artırır
        /// </summary>
        public void IncrementSpinsPlayed()
        {
            _spinsPlayed++;
            OnStatsUpdated();
        }

        /// <summary>
        /// Kazanılan miktarı ekler
        /// </summary>
        /// <param name="amount">Kazanılan miktar</param>
        public void AddWin(float amount)
        {
            _totalWins += amount;
            _currentProfitLoss += amount;
            OnStatsUpdated?.Invoke();
        }

        /// <summary>
        /// Kaybedilen miktarı ekler
        /// </summary>
        /// <param name="amount">Kaybedilen miktar</param>
        public void AddLoss(float amount)
        {
            _totalLosses += amount;
            _currentProfitLoss -= amount;
            OnStatsUpdated?.Invoke();
        }

        /// <summary>
        /// İstatistikleri sıfırlar
        /// </summary>
        public void ResetStats()
        {
            _spinsPlayed = 0;
            _totalWins = 0f;
            _totalLosses = 0f;
            _currentProfitLoss = 0f;
            OnStatsUpdated?.Invoke();
        }
    }
}

