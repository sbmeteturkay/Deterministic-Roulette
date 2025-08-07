using System;
using System.Collections.Generic;
using RouletteGame.Service;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteGame.Models
{
    public struct Chip
    {
        public int value;
        public Texture icon;
        
        public Chip(Texture icon, int value)
        {
            this.icon = icon;
            this.value = value;
        }
    }
    /// <summary>
    /// Oyuncunun çip bakiyesini yöneten sınıf
    /// </summary>
    public class ChipManager
    {
        private int _currentBalance;
        private int _selectedChipValue=1;

        // Mevcut çip değerleri
        public List<Chip> AvailableChipValues = new();

        public int CurrentBalance => _currentBalance;
        public int SelectedChipValue => _selectedChipValue;

        public event Action<int> OnBalanceChanged;
        public event Action<int> OnChipValueChanged;
        public List<int> predefinedChipValues = new() { 1, 5, 10, 25, 50, 100 };

        public ChipManager(List<Toggle> chipSelectionToggles)
        {
            if (!PlayerPrefs.HasKey("ChipBalance"))
            {
                PlayerPrefs.SetInt("ChipBalance", 100);
            }
            _currentBalance = PlayerPrefs.GetInt("ChipBalance");
            
            var list = chipSelectionToggles;
            for (var i = 0; i < list.Count; i++)
            {
                AvailableChipValues.Add(new Chip(list[i].targetGraphic.mainTexture,
                    predefinedChipValues[i]));
            }
        }

        /// <summary>
        /// Çip değeri seçer
        /// </summary>
        /// <param name="chipValue">Seçilecek çip değeri</param>
        /// <returns>Seçim başarılı mı</returns>
        public bool SelectChipValue(int chipValue)
        {
            bool isValidChip = AvailableChipValues.Exists(x => x.value == chipValue);
            
            if (isValidChip)
            {
                _selectedChipValue = chipValue;
                ServiceLocator.SoundService.PlaySfx("button-click");
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
        public bool DeductChips(int amount)
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
        public void AddChips(int amount)
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
        public void ResetBalance(int newBalance = 1000)
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

