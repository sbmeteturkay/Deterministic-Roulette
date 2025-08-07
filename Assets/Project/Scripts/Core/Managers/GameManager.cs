using Game.Core;
using Game.Core.Area;
using Game.RouletteSystem;
using RouletteGame.Controllers;
using UnityEngine;
using RouletteGame.Models;
using RouletteGame.Service;
using RouletteGame.Views;

namespace RouletteGame.Managers
{
    /// <summary>
    /// Oyunun genel akışını ve bileşenler arası entegrasyonu yöneten ana sınıf
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private BettingSystem _bettingSystem;
        private SoundManager _soundManager;
        private ChipManager _chipManager;
        private PayoutCalculator _payoutCalculator;

        [Header("Models")]
        private PlayerStatsModel _playerStatsModel;
        
        [Header("Views")]
        [SerializeField] private PlayerStatsView _playerStatsView;
        [SerializeField] private BettingUI _bettingUI;

        [Header("Controllers")]
        [SerializeField] private RouletteController rouletteController;
        
        [Header("Other")]
        [SerializeField] private DeterministicOutcomeSelector _deterministicOutcomeSelector;
        [SerializeField] private AreaSelector _areaSelector;
        [SerializeField] private Animator gameCameraAnimator;
        
        [Header("Sound")]
        [SerializeField]SoundLibrary _soundLibrary;
        [SerializeField]AudioSource sfxSource;
        [SerializeField]AudioSource musicSource;
        
        private bool useRandomNumber=false;

        void Awake()
        {
            // Bağımlılık Enjeksiyonu (Dependency Injection) ve Başlatma
            // Varsayılan olarak Avrupa Ruleti kullanıyoruz
            _playerStatsModel = new PlayerStatsModel();
            _bettingSystem = new BettingSystem();
            _payoutCalculator = new PayoutCalculator();
            _soundManager = new SoundManager(sfxSource, musicSource,_soundLibrary);
            _chipManager = new ChipManager(_bettingUI.GetChipSelectionToggles());

            
            ServiceLocator.Initialize(_soundManager);
            
            // View'ları ve Controller'ları başlat
            _playerStatsView.Initialize(_playerStatsModel);
            _bettingSystem.Initialize(_chipManager,_areaSelector);
            _bettingUI.Initialize(_chipManager, _bettingSystem);

            _deterministicOutcomeSelector.SetNumbers(rouletteController.ReturnNumbers());

            // Event abonelikleri
            _bettingUI.OnSpinRequested += OnSpinRequested;
            
            _deterministicOutcomeSelector.OnDeterministicModeChanged += DeterministicOutcomeSelectorOnOnDeterministicModeChanged;
            _deterministicOutcomeSelector.OnNumberSelectionChanged += DeterministicOutcomeSelectorOnOnNumberSelectionChanged;
            
            rouletteController.OnRouletteBallInPocketEvent += RouletteControllerOnOnRouletteBallInPocket;
        }

 
        void OnDestroy()
        {
            _bettingUI.OnSpinRequested -= OnSpinRequested;
            _deterministicOutcomeSelector.OnDeterministicModeChanged -= DeterministicOutcomeSelectorOnOnDeterministicModeChanged;
            _deterministicOutcomeSelector.OnNumberSelectionChanged -= DeterministicOutcomeSelectorOnOnNumberSelectionChanged;
            
            rouletteController.OnRouletteBallInPocketEvent -= RouletteControllerOnOnRouletteBallInPocket;

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
            _bettingSystem.HoverWinningBetNumber(outcomeWinningNumber);
        }

        private void OnSpinRequested()
        {
            if (_bettingSystem.TotalBetAmount > 0 && _chipManager.CanPlaceBet(_bettingSystem.TotalBetAmount))
            {
                _chipManager.DeductChips(_bettingSystem.TotalBetAmount);
                _playerStatsModel.IncrementSpinsPlayed();
                _bettingUI.SetSpinButtonEnabled(false);
                _bettingSystem.CanBet = false;
                rouletteController.Spin();
                ServiceLocator.SoundService.PlaySfx("spin");
                gameCameraAnimator.Play("Wheel");
            }
            else
            {
                Debug.LogWarning("Bahis yapmadan veya yetersiz bakiye ile spin yapılamaz!");
            }
        }

        private void OnSpinCompleted(int winningNumber)
        {
            Debug.Log($"GameManager: Spin tamamlandı, kazanan sayı: {winningNumber}");

            int totalPayout = _payoutCalculator.CalculateTotalPayout(_bettingSystem.ActiveBets, winningNumber);
            int totalLoss = _payoutCalculator.CalculateTotalLoss(_bettingSystem.ActiveBets, winningNumber);
            int netProfitLoss = totalPayout - totalLoss;

            if (netProfitLoss > 0)
            {
                _chipManager.AddChips(totalPayout); // Sadece kazanılan miktarı ekle
                _playerStatsModel.AddWin(netProfitLoss); // Net karı istatistiklere ekle
                ServiceLocator.SoundService.PlaySfx("win");
                Debug.Log($"Kazandınız! Net kazanç: {netProfitLoss:F2}");
            }
            else if (netProfitLoss < 0)
            {
                ServiceLocator.SoundService.PlaySfx("lose");
                _playerStatsModel.AddLoss(Mathf.Abs(netProfitLoss)); // Net kaybı istatistiklere ekle
                Debug.Log($"Kaybettiniz! Net kayıp: {Mathf.Abs(netProfitLoss):F2}");
            }
            else
            {
                Debug.Log("Berabere!");
            }

            _bettingSystem.ClearAllBets(); // Bahisleri temizle
            _bettingUI.SetSpinButtonEnabled(true);
            _bettingUI.lastWinningNumbers.Add(winningNumber);
            _bettingUI.UpdateWinningNumbers();
            _bettingSystem.CanBet = true;
            gameCameraAnimator.Play("Top");
        }

        private void OnApplicationQuit()
        {
            _playerStatsModel.SaveStats();
            PlayerPrefs.SetInt("ChipBalance",_chipManager.CurrentBalance);
        }
        private void OnApplicationPause(bool isPaused)
        {
            // Uygulama duraklatıldığında (örneğin, telefonun ana ekranına dönüldüğünde)
            if (isPaused)
            {
                _playerStatsModel.SaveStats();
            }
        }

    }
}

