using System;
using UnityEngine;

namespace RouletteGame.Models
{
    /// <summary>
    /// Oyuncunun çip bakiyesini yöneten sınıf
    /// </summary>
    [Serializable]
    public class ChipManager
    {
        [SerializeField] private float _currentBalance;
        [SerializeField] private float _selectedChipValue;

        // Mevcut çip değerleri
        public static readonly float[] AvailableChipValues = { 1f, 5f, 10f, 25f, 50f, 100f };

        public float CurrentBalance => _currentBalance;
        public float SelectedChipValue => _selectedChipValue;

        public event Action<float> OnBalanceChanged;
        public event Action<float> OnChipValueChanged;

        public ChipManager(float initialBalance = 1000f)
        {
            _currentBalance = initialBalance;
            _selectedChipValue = AvailableChipValues[0]; // Default olarak en küçük çip
        }

        /// <summary>
        /// Çip değeri seçer
        /// </summary>
        /// <param name="chipValue">Seçilecek çip değeri</param>
        /// <returns>Seçim başarılı mı</returns>
        public bool SelectChipValue(float chipValue)
        {
            bool isValidChip = Array.Exists(AvailableChipValues, value => Mathf.Approximately(value, chipValue));
            
            if (isValidChip)
            {
                _selectedChipValue = chipValue;
                OnChipValueChanged?.Invoke(_selectedChipValue);
                return true;
            }
            
            Debug.LogError($"Geçersiz çip değeri: {chipValue}");
            return false;
        }

        /// <summary>
        /// Bakiyeden çip düşer (bahis yerleştirme)
        /// </summary>
        /// <param name="amount">Düşülecek miktar</param>
        /// <returns>İşlem başarılı mı</returns>
        public bool DeductChips(float amount)
        {
            if (amount <= 0)
            {
                Debug.LogError("Düşülecek miktar 0'dan büyük olmalı!");
                return false;
            }

            if (_currentBalance < amount)
            {
                Debug.LogWarning("Yetersiz bakiye!");
                return false;
            }

            _currentBalance -= amount;
            OnBalanceChanged?.Invoke(_currentBalance);
            return true;
        }

        /// <summary>
        /// Bakiyeye çip ekler (kazanç)
        /// </summary>
        /// <param name="amount">Eklenecek miktar</param>
        public void AddChips(float amount)
        {
            if (amount <= 0)
            {
                Debug.LogError("Eklenecek miktar 0'dan büyük olmalı!");
                return;
            }

            _currentBalance += amount;
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        /// <summary>
        /// Bakiyeyi sıfırlar
        /// </summary>
        public void ResetBalance(float newBalance = 1000f)
        {
            _currentBalance = newBalance;
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        /// <summary>
        /// Belirtilen miktarda bahis yapılabilir mi kontrol eder
        /// </summary>
        /// <param name="amount">Bahis miktarı</param>
        /// <returns>Bahis yapılabilir mi</returns>
        public bool CanPlaceBet(float amount)
        {
            return amount > 0 && _currentBalance >= amount;
        }
    }
}

