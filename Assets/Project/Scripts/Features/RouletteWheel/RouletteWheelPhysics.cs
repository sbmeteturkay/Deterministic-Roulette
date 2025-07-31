using UnityEngine;
using System;
using System.Collections;
using RouletteGame.Interfaces;

namespace RouletteGame.Physics
{
    /// <summary>
    /// Rulet çarkının fiziksel dönüşünü ve duruşunu yöneten sınıf
    /// </summary>
    public class RouletteWheelPhysics : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private Rigidbody _wheelRigidbody;
        [SerializeField] private float _initialSpinTorque = 100f; // Başlangıç torku
        [SerializeField] private float _decelerationRate = 0.98f; // Yavaşlama oranı (her frame)
        [SerializeField] private float _minAngularVelocityToStop = 0.1f; // Durması için minimum açısal hız
        
        [Header("Wheel Configuration")]
        [SerializeField] private Transform _wheelTransform;
        [SerializeField] private int _numberOfSlots = 37; // Avrupa ruleti için 37 (0-36)
        
        private float _targetAngle = 0f;
        private bool _isSpinning = false;
        private IRouletteType _rouletteType;
        
        // Events
        public event Action<int> OnWheelStopped;
        public event Action OnSpinStarted;
        
        private void Start()
        {
            if (_wheelTransform == null)
                _wheelTransform = transform;

            if (_wheelRigidbody == null)
            {
                _wheelRigidbody = GetComponent<Rigidbody>();
                if (_wheelRigidbody == null)
                {
                    Debug.LogError("RouletteWheelPhysics: Rigidbody bulunamadı!");
                    enabled = false;
                    return;
                }
            }
            _wheelRigidbody.isKinematic = true; // Başlangıçta kinematik olsun
        }
        
        /// <summary>
        /// Rulet tipini ayarlar
        /// </summary>
        /// <param name="rouletteType">Rulet tipi</param>
        public void SetRouletteType(IRouletteType rouletteType)
        {
            _rouletteType = rouletteType;
            _numberOfSlots = rouletteType.AvailableNumbers.Count;
        }
        
        /// <summary>
        /// Çarkı döndürmeye başlar
        /// </summary>
        /// <param name="targetNumber">Hedef sayı (deterministik mod için)</param>
        public void StartSpin(int targetNumber = -1)
        {
            if (_isSpinning) return;
            
            _isSpinning = true;
            OnSpinStarted?.Invoke();
            
            _wheelRigidbody.isKinematic = false; // Fizik motoru kontrol etsin
            _wheelRigidbody.angularVelocity = Vector3.zero;
            _wheelRigidbody.AddTorque(Vector3.up * _initialSpinTorque, ForceMode.Impulse);

            // Hedef açıyı hesapla
            if (targetNumber >= 0)
            {
                _targetAngle = CalculateTargetAngle(targetNumber);
            }
            else
            {
                // Rastgele hedef açı
                _targetAngle = UnityEngine.Random.Range(0f, 360f);
            }
            
            StartCoroutine(SpinCoroutine());
        }
        
        /// <summary>
        /// Belirtilen sayı için hedef açıyı hesaplar
        /// </summary>
        /// <param name="targetNumber">Hedef sayı</param>
        /// <returns>Hedef açı (derece)</returns>
        private float CalculateTargetAngle(int targetNumber)
        {
            if (_rouletteType == null) return 0f;
            
            int index = _rouletteType.AvailableNumbers.IndexOf(targetNumber);
            if (index == -1) return 0f;
            
            // Her slot için açı
            float anglePerSlot = 360f / _numberOfSlots;
            
            // Hedef açıyı hesapla (saat yönünün tersine)
            float baseAngle = index * anglePerSlot;
            
            // Birkaç tam tur ekle (daha gerçekçi görünmesi için)
            int extraRotations = UnityEngine.Random.Range(3, 8);
            float totalAngle = baseAngle + (extraRotations * 360f);
            
            return totalAngle;
        }
        
        /// <summary>
        /// Çark dönüş coroutine'i
        /// </summary>
        private IEnumerator SpinCoroutine()
        {
            // Yavaşlama aşaması
            while (_wheelRigidbody.angularVelocity.magnitude > _minAngularVelocityToStop)
            {
                _wheelRigidbody.angularVelocity *= _decelerationRate; // Açısal hızı azalt
                yield return null;
            }
            
            // Tamamen durdur ve hedef açıya ayarla
            _wheelRigidbody.angularVelocity = Vector3.zero;
            _wheelRigidbody.isKinematic = true; // Fizik motoru kontrol etmesin
            
            // Son pozisyonu ayarla
            Vector3 currentEulerAngles = _wheelTransform.eulerAngles;
            currentEulerAngles.y = _targetAngle;
            _wheelTransform.eulerAngles = currentEulerAngles;
            
            _isSpinning = false;
            
            // Kazanan sayıyı belirle
            int winningNumber = GetWinningNumber();
            OnWheelStopped?.Invoke(winningNumber);
        }
        
        /// <summary>
        /// Mevcut çark pozisyonuna göre kazanan sayıyı belirler
        /// </summary>
        /// <returns>Kazanan sayı</returns>
        private int GetWinningNumber()
        {
            if (_rouletteType == null) return 0;
            
            float currentAngle = _wheelTransform.eulerAngles.y;
            float anglePerSlot = 360f / _numberOfSlots;
            
            // Hangi slotta olduğunu hesapla
            int slotIndex = Mathf.FloorToInt(currentAngle / anglePerSlot);
            slotIndex = slotIndex % _numberOfSlots;
            
            // Güvenlik kontrolü
            if (slotIndex >= 0 && slotIndex < _rouletteType.AvailableNumbers.Count)
            {
                return _rouletteType.AvailableNumbers[slotIndex];
            }
            
            return 0; // Varsayılan olarak 0 döndür
        }
        
        /// <summary>
        /// Çarkın şu anda dönüp dönmediğini kontrol eder
        /// </summary>
        public bool IsSpinning => _isSpinning;
        
        /// <summary>
        /// Mevcut açısal hızı döndürür
        /// </summary>
        public float CurrentAngularVelocity => _wheelRigidbody.angularVelocity.magnitude;
        
        /// <summary>
        /// Çarkı zorla durdurur
        /// </summary>
        public void ForceStop()
        {
            if (_isSpinning)
            {
                StopAllCoroutines();
                _isSpinning = false;
                _wheelRigidbody.angularVelocity = Vector3.zero;
                _wheelRigidbody.isKinematic = true;
                
                int winningNumber = GetWinningNumber();
                OnWheelStopped?.Invoke(winningNumber);
            }
        }
        
        /// <summary>
        /// Çarkı sıfırlar
        /// </summary>
        public void ResetWheel()
        {
            ForceStop();
            _wheelTransform.rotation = Quaternion.identity;
        }
    }
}

