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
        private decimal _currentBalance;
        private decimal _selectedChipValue;

        // Mevcut çip değerleri
        public static readonly decimal[] AvailableChipValues = { 1, 5, 10, 25, 50, 100 };

        public decimal CurrentBalance => _currentBalance;
        public decimal SelectedChipValue => _selectedChipValue;

        public event Action<decimal> OnBalanceChanged;
        public event Action<decimal> OnChipValueChanged;

        public ChipManager(decimal initialBalance = 1000m)
        {
            _currentBalance = initialBalance;
            _selectedChipValue = AvailableChipValues[0]; // Default olarak en küçük çip
        }

        /// <summary>
        /// Çip değeri seçer
        /// </summary>
        /// <param name="chipValue">Seçilecek çip değeri</param>
        /// <returns>Seçim başarılı mı</returns>
        public bool SelectChipValue(decimal chipValue)
        {
            bool isValidChip = Array.Exists(AvailableChipValues, value => value==chipValue);
            
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
        public bool DeductChips(decimal amount)
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
        public void AddChips(decimal amount)
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
        public void ResetBalance(decimal newBalance = 1000m)
        {
            _currentBalance = newBalance;
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        /// <summary>
        /// Belirtilen miktarda bahis yapılabilir mi kontrol eder
        /// </summary>
        /// <param name="amount">Bahis miktarı</param>
        /// <returns>Bahis yapılabilir mi</returns>
        public bool CanPlaceBet(decimal amount)
        {
            return amount > 0 && _currentBalance >= amount;
        }
    }
}

