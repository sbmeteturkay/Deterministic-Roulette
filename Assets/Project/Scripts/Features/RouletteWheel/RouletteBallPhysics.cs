using UnityEngine;
using System;
using System.Collections;

namespace RouletteGame.Physics
{
    /// <summary>
    /// Rulet topunun fiziksel hareketini yöneten sınıf
    /// </summary>
    public class RouletteBallPhysics : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private Rigidbody _ballRigidbody;
        [SerializeField] private float _initialBallSpeed = 5f; // Topun başlangıç hızı
        [SerializeField] private float _ballSpinForce = 10f; // Topa uygulanan dönüş kuvveti
        [SerializeField] private float _ballFriction = 0.5f; // Topun sürtünme katsayısı
        [SerializeField] private float _ballBounceForce = 2f; // Topun zıplama kuvveti
        [SerializeField] private float _minBallSpeedToStop = 0.1f; // Topun durması için minimum hız

        [Header("References")]
        [SerializeField] private Transform _wheelCenter;
        [SerializeField] private Transform _ballStartPoint;

        private bool _isBallMoving = false;
        private int _winningNumber = -1;

        public event Action<int> OnBallStopped;

        private void Start()
        {
            if (_ballRigidbody == null)
            {
                _ballRigidbody = GetComponent<Rigidbody>();
            }
            if (_ballRigidbody == null)
            {
                Debug.LogError("RouletteBallPhysics: Rigidbody bulunamadı!");
                enabled = false;
            }
            
            ResetBall();
        }

        /// <summary>
        /// Topu döndürmeye başlar
        /// </summary>
        public void StartBallSpin()
        {
            if (_isBallMoving) return;

            _isBallMoving = true;
            _ballRigidbody.isKinematic = false;
            _ballRigidbody.velocity = Vector3.zero;
            _ballRigidbody.angularVelocity = Vector3.zero;

            // Topu başlangıç noktasına yerleştir
            if (_ballStartPoint != null)
            {
                _ballRigidbody.position = _ballStartPoint.position;
                _ballRigidbody.rotation = _ballStartPoint.rotation;
            }

            // Topa başlangıç kuvveti uygula
            Vector3 initialDirection = (_wheelCenter.position - _ballRigidbody.position).normalized;
            _ballRigidbody.AddForce(initialDirection * _initialBallSpeed, ForceMode.VelocityChange);
            _ballRigidbody.AddTorque(Vector3.up * _ballSpinForce, ForceMode.Impulse);

            StartCoroutine(MonitorBallSpeed());
        }

        /// <summary>
        /// Topun hızını izler ve durduğunda olayı tetikler
        /// </summary>
        private IEnumerator MonitorBallSpeed()
        {
            while (_isBallMoving)
            {
                // Topun hızını yavaşça azalt
                _ballRigidbody.velocity *= (1f - _ballFriction * Time.deltaTime);
                _ballRigidbody.angularVelocity *= (1f - _ballFriction * Time.deltaTime);

                if (_ballRigidbody.velocity.magnitude < _minBallSpeedToStop && _ballRigidbody.angularVelocity.magnitude < _minBallSpeedToStop)
                {
                    _isBallMoving = false;
                    _ballRigidbody.isKinematic = true; // Topu durdur
                    _ballRigidbody.velocity = Vector3.zero;
                    _ballRigidbody.angularVelocity = Vector3.zero;

                    // Kazanan sayıyı belirle (burada basit bir placeholder)
                    _winningNumber = DetermineWinningNumberFromPosition();
                    OnBallStopped?.Invoke(_winningNumber);
                    Debug.Log($"Top durdu! Kazanan sayı: {_winningNumber}");
                }
                yield return null;
            }
        }

        /// <summary>
        /// Topun pozisyonuna göre kazanan sayıyı belirler (basit bir örnek)
        /// Gerçek uygulamada rulet çarkının slotları ile etkileşim gerekecek.
        /// </summary>
        /// <returns>Kazanan sayı</returns>
        private int DetermineWinningNumberFromPosition()
        {
            // TODO: Topun hangi slota düştüğünü gerçekçi bir şekilde belirle
            // Bu kısım, rulet çarkının fiziksel yapısına ve slotların konumlarına bağlı olacaktır.
            // Şimdilik rastgele bir sayı döndürüyoruz.
            return UnityEngine.Random.Range(0, 37);
        }

        /// <summary>
        /// Topu sıfırlar ve başlangıç noktasına geri getirir
        /// </summary>
        public void ResetBall()
        {
            _isBallMoving = false;
            _ballRigidbody.isKinematic = true;
            _ballRigidbody.velocity = Vector3.zero;
            _ballRigidbody.angularVelocity = Vector3.zero;
            if (_ballStartPoint != null)
            {
                _ballRigidbody.position = _ballStartPoint.position;
                _ballRigidbody.rotation = _ballStartPoint.rotation;
            }
            _winningNumber = -1;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Topun çark veya slotlarla çarpışması durumunda zıplama efekti
            if (_isBallMoving && collision.gameObject.CompareTag("RouletteObstacle")) // Engeller için tag kullanın
            {
                Vector3 normal = collision.contacts[0].normal;
                _ballRigidbody.AddForce(normal * _ballBounceForce, ForceMode.Impulse);
            }
        }
    }
}

