using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RouletteGame.Controllers
{
    /// <summary>
    /// Deterministik sonuç seçimi için UI kontrolcüsü
    /// </summary>
    public class DeterministicOutcomeSelector : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Toggle _deterministicModeToggle;
        [SerializeField] private TMP_Dropdown _numberSelectionDropdown;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] bool isDeterministicMode;
        
        public event Action<int> OnNumberSelectionChanged;
        public event Action<bool> OnDeterministicModeChanged;

        public void Start()
        {
            SetupUI();
            UpdateStatusDisplay();
        }

        private void SetupUI()
        {
            _deterministicModeToggle.onValueChanged.AddListener(OnDeterministicModeToggled);
            _deterministicModeToggle.isOn = isDeterministicMode;
            _numberSelectionDropdown.interactable = isDeterministicMode;
            _numberSelectionDropdown.onValueChanged.AddListener(OnNumberSelection);
        }

        private void OnNumberSelection(int arg0)
        {
            OnNumberSelectionChanged?.Invoke(arg0);
            UpdateStatusDisplay();
        }

        private void OnDestroy()
        {
            if (_deterministicModeToggle != null)
            {
                _deterministicModeToggle.onValueChanged.RemoveListener(OnDeterministicModeToggled);
            }
            _numberSelectionDropdown.onValueChanged.RemoveListener(OnNumberSelection);
        }

        private void OnDeterministicModeToggled(bool isEnabled)
        {
            isDeterministicMode=isEnabled;
            UpdateStatusDisplay();
            OnDeterministicModeChanged?.Invoke(isDeterministicMode);
        }

        private void UpdateStatusDisplay()
        {
            if (_statusText != null)
            {
                if (isDeterministicMode)
                {
                    _statusText.text = $"Deterministik Mod: AÇIK\nSeçili Sayı: {_numberSelectionDropdown.value}";
                    _statusText.color = Color.green;
                }
                else
                {
                    _statusText.text = "Deterministik Mod: KAPALI\nRastgele sonuç";
                    _statusText.color = Color.white;
                }
            }

            _numberSelectionDropdown.interactable = isDeterministicMode;
        }
    }
}

