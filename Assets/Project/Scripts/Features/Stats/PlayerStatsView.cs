using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RouletteGame.Models;

namespace RouletteGame.Views
{
    /// <summary>
    /// Oyuncu istatistiklerinin UI'da gösterimini yöneten sınıf
    /// </summary>
    public class PlayerStatsView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshPro _spinsPlayedText;
        [SerializeField] private TextMeshPro _totalWinsText;
        [SerializeField] private TextMeshPro _totalLossesText;
        [SerializeField] private TextMeshPro _profitLossText;
        [SerializeField] private Button _resetStatsButton;

        private PlayerStatsModel _model;

        public void Initialize(PlayerStatsModel model)
        {
            _model = model;
            _model.OnStatsUpdated += UpdateUI;
            
            if (_resetStatsButton != null)
            {
                _resetStatsButton.onClick.AddListener(ResetStats);
            }
            //veriyi playerprefsten çek
            model.LoadStats();
            // İlk güncelleme
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnStatsUpdated -= UpdateUI;
            }

            if (_resetStatsButton != null)
            {
                _resetStatsButton.onClick.RemoveListener(ResetStats);
            }
        }

        private void UpdateUI()
        {
            if (_model == null) return;

            if (_spinsPlayedText != null)
                _spinsPlayedText.text = $"Oynanan Spin: {_model.SpinsPlayed}";

            if (_totalWinsText != null)
                _totalWinsText.text = $"Toplam Kazanç: {_model.TotalWins:F2}";

            if (_totalLossesText != null)
                _totalLossesText.text = $"Toplam Kayıp: {_model.TotalLosses:F2}";

            if (_profitLossText != null)
            {
                float profitLoss = _model.CurrentProfitLoss;
                string color = profitLoss >= 0 ? "green" : "red";
                string sign = profitLoss >= 0 ? "+" : "";
                _profitLossText.text = $"<color={color}>Net: {sign}{profitLoss:F2}</color>";
            }
        }

        private void ResetStats()
        {
            if (_model != null)
            {
                _model.ResetStats();
            }
        }

        // Manuel güncelleme için public metot
        public void RefreshDisplay()
        {
            UpdateUI();
        }
    }
}

