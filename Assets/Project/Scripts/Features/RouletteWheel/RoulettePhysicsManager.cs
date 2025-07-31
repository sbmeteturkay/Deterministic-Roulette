using UnityEngine;
using System;
using RouletteGame.Interfaces;

namespace RouletteGame.Physics
{
    /// <summary>
    /// Rulet çarkı ve top arasındaki fiziksel etkileşimleri koordine eden sınıf
    /// </summary>
    public class RoulettePhysicsManager : MonoBehaviour
    {
        [Header("Physics Components")]
        [SerializeField] private RouletteWheelPhysics _wheelPhysics;
        [SerializeField] private RouletteBallPhysics _ballPhysics;
        
        [Header("Timing Settings")]
        [SerializeField] private float _ballStartDelay = 1f; // Çark başladıktan sonra topun başlama gecikmesi
        [SerializeField] private float _ballStopDelay = 2f; // Çark durduktan sonra topun durma gecikmesi
        
        private bool _isSpinInProgress = false;
        private int _targetNumber = -1;
        
        // Events
        public event Action<int> OnSpinCompleted;
        public event Action OnSpinStarted;
        
        private void Start()
        {
            if (_wheelPhysics == null)
                _wheelPhysics = GetComponentInChildren<RouletteWheelPhysics>();
            
            if (_ballPhysics == null)
                _ballPhysics = GetComponentInChildren<RouletteBallPhysics>();
            
            // Event subscriptions
            if (_wheelPhysics != null)
            {
                _wheelPhysics.OnWheelStopped += OnWheelStopped;
                _wheelPhysics.OnSpinStarted += OnWheelSpinStarted;
            }
            
            if (_ballPhysics != null)
            {
                _ballPhysics.OnBallStopped += OnBallStopped;
            }
        }
        
        private void OnDestroy()
        {
            if (_wheelPhysics != null)
            {
                _wheelPhysics.OnWheelStopped -= OnWheelStopped;
                _wheelPhysics.OnSpinStarted -= OnWheelSpinStarted;
            }
            
            if (_ballPhysics != null)
            {
                _ballPhysics.OnBallStopped -= OnBallStopped;
            }
        }
        
        /// <summary>
        /// Rulet tipini ayarlar
        /// </summary>
        /// <param name="rouletteType">Rulet tipi</param>
        public void SetRouletteType(IRouletteType rouletteType)
        {
            if (_wheelPhysics != null)
            {
                _wheelPhysics.SetRouletteType(rouletteType);
            }
        }
        
        /// <summary>
        /// Spin işlemini başlatır
        /// </summary>
        /// <param name="targetNumber">Hedef sayı (deterministik mod için, -1 ise rastgele)</param>
        public void StartSpin(int targetNumber = -1)
        {
            if (_isSpinInProgress) return;
            
            _isSpinInProgress = true;
            _targetNumber = targetNumber;
            
            OnSpinStarted?.Invoke();
            
            // Çarkı başlat
            if (_wheelPhysics != null)
            {
                _wheelPhysics.StartSpin(targetNumber);
            }
            
            // Topu gecikmeyle başlat
            Invoke(nameof(StartBallSpin), _ballStartDelay);
        }
        
        private void StartBallSpin()
        {
            if (_ballPhysics != null)
            {
                _ballPhysics.StartBallSpin();
            }
        }
        
        private void OnWheelSpinStarted()
        {
            Debug.Log("Çark dönmeye başladı!");
        }
        
        private void OnWheelStopped(int winningNumber)
        {
            Debug.Log($"Çark durdu! Kazanan sayı: {winningNumber}");
            // Çark durduğunda top hala hareket ediyor olabilir
        }
        
        private void OnBallStopped(int winningNumber)
        {
            Debug.Log($"Top durdu! Final kazanan sayı: {winningNumber}");
            
            // Spin işlemi tamamlandı
            _isSpinInProgress = false;
            
            // Deterministik modda hedef sayı varsa onu kullan, yoksa topun belirlediği sayıyı kullan
            int finalWinningNumber = _targetNumber >= 0 ? _targetNumber : winningNumber;
            
            OnSpinCompleted?.Invoke(finalWinningNumber);
        }
        
        /// <summary>
        /// Spin işlemini zorla durdurur
        /// </summary>
        public void ForceStopSpin()
        {
            if (_wheelPhysics != null)
            {
                _wheelPhysics.ForceStop();
            }
            
            if (_ballPhysics != null)
            {
                _ballPhysics.ResetBall();
            }
            
            _isSpinInProgress = false;
            CancelInvoke(); // Bekleyen invoke'ları iptal et
        }
        
        /// <summary>
        /// Tüm fizik bileşenlerini sıfırlar
        /// </summary>
        public void ResetPhysics()
        {
            ForceStopSpin();
            
            if (_wheelPhysics != null)
            {
                _wheelPhysics.ResetWheel();
            }
            
            if (_ballPhysics != null)
            {
                _ballPhysics.ResetBall();
            }
        }
        
        /// <summary>
        /// Şu anda spin işlemi devam ediyor mu
        /// </summary>
        public bool IsSpinInProgress => _isSpinInProgress;
        
        /// <summary>
        /// Çarkın şu anda dönüp dönmediğini kontrol eder
        /// </summary>
        public bool IsWheelSpinning => _wheelPhysics != null && _wheelPhysics.IsSpinning;
    }
}

