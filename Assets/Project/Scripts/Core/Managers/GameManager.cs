using Game.RouletteSystem;
using RouletteGame.Controllers;
using UnityEngine;
using RouletteGame.Models;
using RouletteGame.Views;

namespace RouletteGame.Managers
{
    /// <summary>
    /// Oyunun genel akışını ve bileşenler arası entegrasyonu yöneten ana sınıf
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Models")]
        [SerializeField] private PlayerStatsModel _playerStatsModel;
        [SerializeField] private BettingSystem _bettingSystem;
        [SerializeField] private ChipManager _chipManager;
        
        [Header("Views")]
        [SerializeField] private PlayerStatsView _playerStatsView;
        [SerializeField] private BettingUI _bettingUI;

        [Header("Controllers")]
        [SerializeField] private DeterministicOutcomeSelector _deterministicOutcomeSelector;
        [SerializeField] private BetController _betController;
        [SerializeField] private RouletteController rouletteController;

        // Diğer yardımcı sınıflar
        private PayoutCalculator _payoutCalculator;

        private bool useRandomNumber=false;

        void Awake()
        {
            // Bağımlılık Enjeksiyonu (Dependency Injection) ve Başlatma
            // Varsayılan olarak Avrupa Ruleti kullanıyoruz

            _playerStatsModel = new PlayerStatsModel();
            _bettingSystem = new BettingSystem();
            _chipManager = new ChipManager();
            _payoutCalculator = new PayoutCalculator();

            // View'ları ve Controller'ları başlat
            _playerStatsView.Initialize(_playerStatsModel);
            _bettingUI.Initialize(_chipManager, _bettingSystem);
            _betController.Initialize(_chipManager, _bettingSystem);


            _deterministicOutcomeSelector.SetNumbers(rouletteController.ReturnNumbers());

            // Event abonelikleri
            _bettingUI.OnSpinRequested += OnSpinRequested;
            
            _deterministicOutcomeSelector.OnDeterministicModeChanged += DeterministicOutcomeSelectorOnOnDeterministicModeChanged;
            _deterministicOutcomeSelector.OnNumberSelectionChanged += DeterministicOutcomeSelectorOnOnNumberSelectionChanged;
            
            rouletteController.OnRouletteBallInPocket += RouletteControllerOnOnRouletteBallInPocket;
        }

 
        void OnDestroy()
        {
            _bettingUI.OnSpinRequested -= OnSpinRequested;
            _deterministicOutcomeSelector.OnDeterministicModeChanged -= DeterministicOutcomeSelectorOnOnDeterministicModeChanged;
            _deterministicOutcomeSelector.OnNumberSelectionChanged -= DeterministicOutcomeSelectorOnOnNumberSelectionChanged;
            
            rouletteController.OnRouletteBallInPocket -= RouletteControllerOnOnRouletteBallInPocket;

        }
        private void DeterministicOutcomeSelectorOnOnNumberSelectionChanged(int targetNumber)
        {
            rouletteController.SetTargetOutCome(targetNumber);
        }

        private void DeterministicOutcomeSelectorOnOnDeterministicModeChanged(bool isDeterministic)
        {
            useRandomNumber = !isDeterministic;
            rouletteController.UseRandomNumber(useRandomNumber);
        }
        private void RouletteControllerOnOnRouletteBallInPocket(int outcomeWinningNumber)
        {
            OnSpinCompleted(outcomeWinningNumber);
        }

        private void OnSpinRequested()
        {
            if (_bettingSystem.TotalBetAmount > 0 && _chipManager.CanPlaceBet(_bettingSystem.TotalBetAmount))
            {
                _chipManager.DeductChips(_bettingSystem.TotalBetAmount);
                _playerStatsModel.IncrementSpinsPlayed();
                _bettingUI.SetSpinButtonEnabled(false);
                rouletteController.Spin();
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
    }
}

