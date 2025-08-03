using UnityEngine;
using RouletteGame.Models;
using RouletteGame.Views;
using RouletteGame.Controllers;
using RouletteGame.Interfaces;

namespace RouletteGame.Managers
{
    /// <summary>
    /// Oyunun genel akışını ve bileşenler arası entegrasyonu yöneten ana sınıf
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Models")]
        [SerializeField] private PlayerStatsModel _playerStatsModel;
        [SerializeField] private RouletteWheelModel _rouletteWheelModel;
        [SerializeField] private BettingSystem _bettingSystem;
        [SerializeField] private ChipManager _chipManager;
        
        [Header("Views")]
        [SerializeField] private PlayerStatsView _playerStatsView;
        [SerializeField] private RouletteWheelView _rouletteWheelView;
        [SerializeField] private BettingUI _bettingUI;

        [Header("Controllers")]
        [SerializeField] private RouletteWheelController _rouletteWheelController;
        [SerializeField] private DeterministicOutcomeSelector _deterministicOutcomeSelector;
        [SerializeField] private BetController _betController;

        // Diğer yardımcı sınıflar
        private PayoutCalculator _payoutCalculator;
        private IRouletteType _currentRouletteType; // Avrupa veya Amerikan ruleti

        void Awake()
        {
            // Bağımlılık Enjeksiyonu (Dependency Injection) ve Başlatma
            // Varsayılan olarak Avrupa Ruleti kullanıyoruz
            _currentRouletteType = new EuropeanRoulette(); 

            _playerStatsModel = new PlayerStatsModel();
            _rouletteWheelModel = new RouletteWheelModel(_currentRouletteType);
            _bettingSystem = new BettingSystem();
            _chipManager = new ChipManager();
            _payoutCalculator = new PayoutCalculator();

            // View'ları ve Controller'ları başlat
            _playerStatsView.Initialize(_playerStatsModel);
            _bettingUI.Initialize(_chipManager, _bettingSystem);
            _rouletteWheelController.Initialize(_rouletteWheelModel, _rouletteWheelView);
            _deterministicOutcomeSelector.Initialize(_rouletteWheelModel, _currentRouletteType);
            _betController.Initialize(_chipManager, _bettingSystem);

            // Event abonelikleri
            _rouletteWheelModel.OnSpinCompleted += OnSpinCompleted;
            _bettingUI.OnSpinRequested += OnSpinRequested;
        }

        void OnDestroy()
        {
            if (_rouletteWheelModel != null)
            {
                _rouletteWheelModel.OnSpinCompleted -= OnSpinCompleted;
            }
            if (_bettingUI != null)
            {
                _bettingUI.OnSpinRequested -= OnSpinRequested;
            }
        }

        private void OnSpinRequested()
        {
            if (_bettingSystem.TotalBetAmount > 0 && _chipManager.CanPlaceBet(_bettingSystem.TotalBetAmount))
            {
                _chipManager.DeductChips(_bettingSystem.TotalBetAmount);
                _playerStatsModel.IncrementSpinsPlayed();
                _rouletteWheelController.SpinWheel();
                _bettingUI.SetSpinButtonEnabled(false);
            }
            else
            {
                Debug.LogWarning("Bahis yapmadan veya yetersiz bakiye ile spin yapılamaz!");
            }
        }

        private void OnSpinCompleted(int winningNumber)
        {
            Debug.Log($"GameManager: Spin tamamlandı, kazanan sayı: {winningNumber}");

            float totalPayout = _payoutCalculator.CalculateTotalPayout(_bettingSystem.ActiveBets, winningNumber);
            float totalLoss = _payoutCalculator.CalculateTotalLoss(_bettingSystem.ActiveBets, winningNumber);
            float netProfitLoss = totalPayout - totalLoss;

            if (netProfitLoss > 0)
            {
                _chipManager.AddChips(totalPayout); // Sadece kazanılan miktarı ekle
                _playerStatsModel.AddWin(netProfitLoss); // Net karı istatistiklere ekle
                Debug.Log($"Kazandınız! Net kazanç: {netProfitLoss:F2}");
            }
            else if (netProfitLoss < 0)
            {
                _playerStatsModel.AddLoss(Mathf.Abs(netProfitLoss)); // Net kaybı istatistiklere ekle
                Debug.Log($"Kaybettiniz! Net kayıp: {Mathf.Abs(netProfitLoss):F2}");
            }
            else
            {
                Debug.Log("Berabere!");
            }

            _bettingSystem.ClearAllBets(); // Bahisleri temizle
            _bettingUI.SetSpinButtonEnabled(true);
        }

        /// <summary>
        /// Rulet tipini değiştirir (Avrupa/Amerikan)
        /// </summary>
        /// <param name="newType">Yeni rulet tipi</param>
        public void SetRouletteType(IRouletteType newType)
        {
            _currentRouletteType = newType;
            _rouletteWheelModel = new RouletteWheelModel(_currentRouletteType); // Modeli yeni tip ile yeniden başlat
            _rouletteWheelView.Initialize(_rouletteWheelModel); // View'ı yeni model ile güncelle
            _deterministicOutcomeSelector.Initialize(_rouletteWheelModel, _currentRouletteType); // Kontrolcüyü güncelle
            Debug.Log($"Rulet tipi {newType.TypeName} olarak değiştirildi.");
        }
    }
}

