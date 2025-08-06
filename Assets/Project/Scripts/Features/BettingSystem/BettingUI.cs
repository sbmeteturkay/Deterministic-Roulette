using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RouletteGame.Models;
using RouletteGame.Interfaces;

namespace RouletteGame.Views
{
    /// <summary>
    /// Bahis yerleştirme UI'sını yöneten sınıf
    /// </summary>
    public class BettingUI : MonoBehaviour
    {
        [Header("Chip Selection")]
        [SerializeField] private Button[] _chipButtons;
        [SerializeField] private TextMeshProUGUI _selectedChipText;
        [SerializeField] private TextMeshProUGUI _balanceText;

        [Header("Betting Controls")]
        [SerializeField] private Button _clearBetsButton;
        [SerializeField] private Button _spinButton;
        [SerializeField] private TextMeshProUGUI _totalBetText;

        //[Header("Betting Table")]
        //[SerializeField] private Transform _bettingTableParent;
        //[SerializeField] private GameObject _betIndicatorPrefab;

        private ChipManager _chipManager;
        private IBettingSystem _bettingSystem;
        //private List<GameObject> _betIndicators = new List<GameObject>();

        public System.Action OnSpinRequested;
        public System.Action<decimal> OnChipValueSelected;

        public void Initialize(ChipManager chipManager, IBettingSystem bettingSystem)
        {
            _chipManager = chipManager;
            _bettingSystem = bettingSystem;

            // Event subscriptions
            _chipManager.OnBalanceChanged += UpdateBalanceDisplay;
            _chipManager.OnChipValueChanged += UpdateSelectedChipDisplay;
            _bettingSystem.OnBetAdded += OnBetAdded;
            _bettingSystem.OnBetRemoved += OnBetRemoved;
            _bettingSystem.OnAllBetsCleared += OnAllBetsCleared;

            SetupChipButtons();
            SetupControlButtons();
            
            // İlk güncelleme
            UpdateBalanceDisplay(_chipManager.CurrentBalance);
            UpdateSelectedChipDisplay(_chipManager.SelectedChipValue);
            UpdateTotalBetDisplay();
        }

        private void OnDestroy()
        {
            if (_chipManager != null)
            {
                _chipManager.OnBalanceChanged -= UpdateBalanceDisplay;
                _chipManager.OnChipValueChanged -= UpdateSelectedChipDisplay;
            }

            if (_bettingSystem != null)
            {
                _bettingSystem.OnBetAdded -= OnBetAdded;
                _bettingSystem.OnBetRemoved -= OnBetRemoved;
                _bettingSystem.OnAllBetsCleared -= OnAllBetsCleared;
            }
        }

        private void SetupChipButtons()
        {
            for (int i = 0; i < _chipButtons.Length && i < ChipManager.AvailableChipValues.Length; i++)
            {
                int chipValue = ChipManager.AvailableChipValues[i];
                int index = i; // Closure için local copy

                _chipButtons[i].onClick.AddListener(() => SelectChip(chipValue));
                
                // Button text'ini güncelle
                var buttonText = _chipButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = chipValue.ToString("F0");
                }
            }
        }

        private void SetupControlButtons()
        {
            if (_clearBetsButton != null)
            {
                _clearBetsButton.onClick.AddListener(ClearAllBets);
            }

            if (_spinButton != null)
            {
                _spinButton.onClick.AddListener(()=>
                {
                    RequestSpin();               
                });
            }
        }

        private void SelectChip(int chipValue)
        {
            _chipManager.SelectChipValue(chipValue);
            OnChipValueSelected?.Invoke(chipValue);
        }

        private void ClearAllBets()
        {
            _bettingSystem.ClearAllBets();
        }

        private void RequestSpin()
        {
            OnSpinRequested?.Invoke();
        }

        private void UpdateBalanceDisplay(int balance)
        {
            if (_balanceText != null)
            {
                _balanceText.text = $"Bakiye: {balance:F2}";
            }
        }

        private void UpdateSelectedChipDisplay(int chipValue)
        {
            if (_selectedChipText != null)
            {
                _selectedChipText.text = $"Seçili Çip: {chipValue:F0}";
            }
        }

        private void UpdateTotalBetDisplay()
        {
            if (_totalBetText != null)
            {
                _totalBetText.text = $"Toplam Bahis: {_bettingSystem.TotalBetAmount:F2}";
            }
        }

        private void OnBetAdded(IBet bet)
        {
            UpdateTotalBetDisplay();
            // Görsel bet indicator eklenebilir
            CreateBetIndicator(bet);
        }

        private void OnBetRemoved(IBet bet)
        {
            UpdateTotalBetDisplay();
            // Görsel bet indicator kaldırılabilir
            RemoveBetIndicator(bet);
        }

        private void OnAllBetsCleared()
        {
            UpdateTotalBetDisplay();
            ClearAllBetIndicators();
        }

        private void CreateBetIndicator(IBet bet)
        {
            // TODO: Bahis masasında görsel indicator oluştur
            Debug.Log($"Bahis eklendi: {bet.BetType} - {bet.BetAmount}");
        }

        private void RemoveBetIndicator(IBet bet)
        {
            // TODO: Bahis masasından görsel indicator kaldır
            Debug.Log($"Bahis kaldırıldı: {bet.BetType} - {bet.BetAmount}");
        }

        private void ClearAllBetIndicators()
        {
            // TODO: Tüm görsel indicatorları temizle
            // foreach (var indicator in _betIndicators)
            // {
            //     if (indicator != null)
            //         Destroy(indicator);
            // }
            //_betIndicators.Clear();
            Debug.Log("Tüm bahisler temizlendi");
        }

        public void SetSpinButtonEnabled(bool enabled)
        {
            if (_spinButton != null)
            {
                _spinButton.interactable = enabled;
            }
        }
    }
}

