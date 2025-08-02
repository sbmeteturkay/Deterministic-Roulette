using UnityEngine;
using System;

namespace Game.RouletteSystem
{
    public class RouletteWheelView : MonoBehaviour, IWheelView
    {
        [Header("References")]
        [SerializeField] private Transform center;
        [SerializeField] private RouletteWheelModelSO modelSO;
        [SerializeField, Tooltip("Görsel olarak dönecek nesne. Eğer boşsa kendi transform'unu kullanır.")]
        private Transform spinVisual;

        [Header("Visual")]
        [SerializeField, Tooltip("0° referansını kaydırmak için (derece). Gizmo ve cep hesaplamalarıyla uyumlu tutmak için kullanılır.")]
        public float rotationOffsetDegrees = 0f;

        private IRouletteWheelModel model => modelSO;

        private float currentRotation; // acumulative degrees (before offset)
        private float initialAngularVelocity;
        private float spinDuration;
        private float elapsed;
        private bool spinning;
        private int lastReportedPocket = -1;

        internal IRouletteWheelModel Model => model;
        internal Transform Center => center;
        public void Initialize(IRouletteWheelModel model, Transform center, Transform spinVisual = null)
        {
            if (model is RouletteWheelModelSO so)
                modelSO = so;

            currentRotation = 0f;
            spinning = false;
        }

        public void StartSpin(float initialAngularVelocity, float duration)
        {
            if (model == null) return;
            this.initialAngularVelocity = initialAngularVelocity;
            spinDuration = Mathf.Max(0.01f, duration);
            elapsed = 0f;
            spinning = true;
            OnSpinStarted?.Invoke();
        }

        public void UpdateRotation(float deltaTime)
        {
            if (!spinning || model == null) return;

            elapsed += deltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            float ease = model.RotationEaseCurve.Evaluate(t);
            float angularVelocity = initialAngularVelocity * ease;
            currentRotation += angularVelocity * deltaTime;

            ApplyVisualRotation();

            int currentPocket = GetPocketUnderIndicator();
            if (currentPocket != lastReportedPocket)
            {
                lastReportedPocket = currentPocket;
                OnIntermediatePocketChanged?.Invoke(currentPocket);
            }

            if (t >= 1f || Mathf.Abs(angularVelocity) < 0.1f)
            {
                if (spinning)
                {
                    spinning = false;
                    OnSpinStopped?.Invoke(GetPocketUnderIndicator());
                }
            }
        }

        private void ApplyVisualRotation()
        {
            float total = currentRotation + rotationOffsetDegrees;
            if (spinVisual != null)
                spinVisual.localRotation = Quaternion.Euler(0f, total, 0);
        }

        public int GetPocketUnderIndicator()
        {
            if (model == null) return -1;

            float normalized = ((currentRotation + rotationOffsetDegrees) % 360f + 360f) % 360f;
            var pockets = model.PocketDefinitions;
            int count = pockets.Count;
            float slice = 360f / count;
            int index = Mathf.FloorToInt(normalized / slice);
            index = Mathf.Clamp(index, 0, count - 1);
            return pockets[index].Number;
        }

        public event Action<int> OnIntermediatePocketChanged;
        public event Action<int> OnSpinStopped;
        public event Action OnSpinStarted;
    }

    public interface IWheelView
    {
        void Initialize(IRouletteWheelModel model, Transform center, Transform spinVisual = null);
        void StartSpin(float initialAngularVelocity, float duration);
        void UpdateRotation(float deltaTime);
        int GetPocketUnderIndicator();
        event Action<int> OnIntermediatePocketChanged;
        event Action<int> OnSpinStopped;
        event Action OnSpinStarted;
    }
}
