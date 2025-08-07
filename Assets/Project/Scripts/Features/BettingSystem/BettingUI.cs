using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private ToggleGroup _chipGroup;
        [SerializeField] private TextMeshProUGUI _selectedChipText;
        [SerializeField] private TextMeshPro _balanceText;

        [Header("Betting Controls")]
        [SerializeField] private Button _clearBetsButton;
        [SerializeField] private Button _spinButton;
        [SerializeField] private TextMeshPro _totalBetText;
        
        [Header("Other")]
        [SerializeField] private TextMeshPro lastWinningNumbersText;

        private ChipManager _chipManager;
        private BettingSystem _bettingSystem;
        private List<Toggle> _chipSelectionToggles=new();

        public Action OnSpinRequested;
        private Camera _camera;
        
        [HideInInspector]
        public List<int> lastWinningNumbers = new();
        bool isBetValuable => _bettingSystem.TotalBetAmount<=_chipManager.CurrentBalance;

        //chips to show up on table
        Dictionary<int, int> chipsOnVault = new();
        float chipSpacingX = 25;
        float chipSpacingY = 3;

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
            UpdateWinningNumbers();
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

        public void UpdateWinningNumbers()
        {
            var lastWinningNumbersTxt = "";
            for (var index = 0; index < lastWinningNumbers.Count; index++)
            {
                var lastWinningNumber = lastWinningNumbers[index];
                lastWinningNumbersTxt += (index!=0?", ":"")+lastWinningNumber.ToString();
            }

            lastWinningNumbersText.text= lastWinningNumbersTxt;
        }

        private void OnBetAdded(IBet bet)
        {
            UpdateTotalBetDisplay();
            // Görsel bet indicator eklenebilir
            CreateBetIndicator(bet);
            SetSpinButtonEnabled(isBetValuable);
        }

        private void OnBetRemoved(IBet bet)
        {
            UpdateTotalBetDisplay();
            // Görsel bet indicator kaldırılabilir
            RemoveBetIndicator(bet);
            SetSpinButtonEnabled(isBetValuable);
        }

        private void OnAllBetsCleared()
        {
            UpdateTotalBetDisplay();
            ClearAllBetIndicators();
            SetSpinButtonEnabled(isBetValuable);
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
            DrawChipsOnBetTable();
            DrawChipsOnVault();
        }

        private void DrawChipsOnVault()
        {
            var balance = _chipManager.CurrentBalance-_bettingSystem.TotalBetAmount;

            for (var index = _chipManager.predefinedChipValues.Count-1; index>=0; index--)
            {
                var denom = _chipManager.predefinedChipValues[index];
                chipsOnVault[denom] = balance / denom;
                balance %= denom;
            }

            var groupChips = chipsOnVault
                .Where(kvp => kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Key)
                .Select(kvp => kvp.Key)
                .ToList();

            Vector3 screenPos = _camera.WorldToScreenPoint(_balanceText.transform.position);
            screenPos.y = Screen.height - (screenPos.y+ 30);
            for (int x = 0; x < groupChips.Count; x++)
            {
                int denom = groupChips[x];
                int count = chipsOnVault[denom];

                var chip = _chipManager.AvailableChipValues
                    .Find(_x => _x.value == groupChips[x]);

                for (int y = 0; y < count; y++)
                {
                    var posX = screenPos.x- x * chipSpacingX;
                    var posY = screenPos.y - y * chipSpacingY;

                    GUI.DrawTexture(new Rect(posX, posY, 25, 25), chip.icon);
                }
            }
        }

        private void DrawChipsOnBetTable()
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

