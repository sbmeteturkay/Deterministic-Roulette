using UnityEngine;
using System;

namespace Game.RouletteSystem
{
    public class RouletteWheelController : IWheelController
    {
        private readonly IWheelView view;
        private readonly IRouletteWheelModel model;
        private readonly System.Random rng = new();

        public RouletteWheelController(IWheelView view, IRouletteWheelModel model)
        {
            this.view = view;
            this.model = model;

            // Subscribe to view events; controller exposes them outward unchanged.
            view.OnSpinStopped += pocket => OnSpinStopped?.Invoke(pocket);
            view.OnIntermediatePocketChanged += pocket => OnIntermediatePocketChanged?.Invoke(pocket);
            view.OnSpinStarted += () => OnSpinStarted?.Invoke();
        }

        public void StartSpin(int? targetPocketNumber = null)
        {
            // targetPocketNumber per spec yok sayılıyor (rastgele)
            // 1. Rastgele tam tur sayısı (örneğin 3..7)
            int fullRotations = UnityEngine.Random.Range(3, 8); // upper bound exclusive -> 3..7

            float totalRotation = fullRotations * 360f;

            // 2. Rastgele süre, model'den al (fallback değerler)
            float minDuration = GetModelFloatField("MinSpinDuration", 2f);
            float maxDuration = GetModelFloatField("MaxSpinDuration", 4f);
            float duration = UnityEngine.Random.Range(minDuration, maxDuration);

            // 3. Easing integralini hesapla
            float easeIntegral = ComputeEaseIntegral(model.RotationEaseCurve, samples: 128);
            if (easeIntegral <= 0f) easeIntegral = 1f; // güvenlik

            // 4. Başlangıç açısal hızı çöz: totalRotation = duration * initialVelocity * easeIntegral
            float initialAngularVelocity = totalRotation / (duration * easeIntegral);

            // 5. Spin’i başlat
            view.StartSpin(initialAngularVelocity, duration);
        }

        public void StopSpin()
        {
            // Doğal easing ile durduğu için burada bir şey yapmıyoruz.
            // Gerekirse hızlı durdurma için view'a ek API gerekir.
        }

        public void ForceSetRotation(float angleDegrees)
        {
            // Eğer view bu API'yı desteklemiyorsa genişletmen gerekir.
            // Burada varsayılan olarak yapılacak bir şey yok.
        }

        public event Action OnSpinStarted;
        public event Action<int> OnSpinStopped;
        public event Action<int> OnIntermediatePocketChanged;

        // --- Yardımcılar ---

        private float ComputeEaseIntegral(AnimationCurve curve, int samples = 100)
        {
            float sum = 0f;
            float step = 1f / samples;
            for (int i = 0; i <= samples; i++)
            {
                float t = i * step;
                float v = curve.Evaluate(t);
                if (i == 0 || i == samples)
                    sum += v * 0.5f;
                else
                    sum += v;
            }
            return sum * step;
        }

        private float GetModelFloatField(string fieldName, float fallback)
        {
            // ScriptableObject özel alanlarını almak için cast etmeye çalış
            if (model is RouletteWheelModelSO so)
            {
                var field = typeof(RouletteWheelModelSO).GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null && field.GetValue(so) is float f)
                    return f;
            }
            return fallback;
        }
    }
    public interface IWheelController
    {
        void StartSpin(int? targetPocketNumber = null);
        void StopSpin();
        void ForceSetRotation(float angleDegrees);
        event Action OnSpinStarted;
        event Action<int> OnSpinStopped;
        event Action<int> OnIntermediatePocketChanged;
    }
}
