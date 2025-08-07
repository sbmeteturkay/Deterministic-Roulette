using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RouletteGame.Models;
using RouletteGame.Interfaces;
using UnityEngine.Serialization;

namespace RouletteGame.Views
{
    /// <summary>
    /// Bahis yerleştirme UI'sını yöneten sınıf
    /// </summary>
    public class BettingUI : MonoBehaviour
    {
        [Header("Chip Selection")]
        [SerializeField] private ToggleGroup _chipGroup;
        [SerializeField] private TextMeshProUGUI _selectedChipText;
        [SerializeField] private TextMeshProUGUI _balanceText;

        [Header("Betting Controls")]
        [SerializeField] private Button _clearBetsButton;
        [SerializeField] private Button _spinButton;
        [SerializeField] private TextMeshProUGUI _totalBetText;

        private ChipManager _chipManager;
        private BettingSystem _bettingSystem;
        private List<Toggle> _chipSelectionToggles=new();

        public Action OnSpinRequested;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Awake()
        {
            _chipSelectionToggles= _chipGroup.GetComponentsInChildren<Toggle>()
                .Where(t => t.group == _chipGroup)
                .ToList();
        }

        public void Initialize(ChipManager chipManager, BettingSystem bettingSystem)
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

        public List<Toggle> GetChipSelectionToggles()
        {
            return _chipSelectionToggles;
        }
        private void SetupChipButtons()
        {

            for (int i = 0; i < _chipSelectionToggles.Count; i++)
            {
                int chipValue = _chipManager.AvailableChipValues[i].value;
                
                _chipSelectionToggles[i].onValueChanged.AddListener((x) =>
                {
                    if (x)
                        SelectChip(chipValue);
                });
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
        }

        private void ClearAllBets()
        {
            _bettingSystem.ClearAllBets();
            _bettingSystem.ClearCoveredBetNumbers();
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

        private void OnGUI()
        {
            if (_bettingSystem.ActiveBets.Count == 0)
                return;

            // 1. Aynı pozisyondaki bet'leri grupla
            var groupedBets = _bettingSystem.ActiveBets
                .GroupBy(b => b.chipPosition)
                .ToList();

            foreach (var group in groupedBets)
            {
                // Ortak ekran pozisyonu
                Vector3 screenPos = _camera.WorldToScreenPoint(group.Key);
                if (screenPos.z < 0) continue; // Kamera arkasında ise çizme

                float guiY = Screen.height - screenPos.y;

                int stackIndex = 0;
                foreach (var activeBet in group)
                {
                    var chip = _chipManager.AvailableChipValues
                        .Find(x => x.value == activeBet.BetAmount);

                    // Stack efekti (küçültme veya offset)
                    float offset = stackIndex * 3f;
                    float size = 25f; // veya stackIndex'e göre değiştirilebilir

                    Rect rect = new Rect(
                        screenPos.x - size / 2f,
                        guiY - size / 2f - offset,
                        size,
                        size
                    );

                    GUI.DrawTexture(rect, chip.icon);
                    stackIndex++;
                }
            }
        }
    }
}

