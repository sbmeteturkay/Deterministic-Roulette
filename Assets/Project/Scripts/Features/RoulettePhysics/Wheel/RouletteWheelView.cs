using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Serialization;

namespace Game.RouletteSystem
{
    public class RouletteWheelView : MonoBehaviour, IWheelView
    {
        [Header("References")]
        [SerializeField] private Transform center;
        [SerializeField] private RouletteWheelModelSO modelSO;
        [FormerlySerializedAs("spinVisual")] [SerializeField, Tooltip("Görsel olarak dönecek nesne. Eğer boşsa kendi transform'unu kullanır.")]
        public Transform SpinVisual;

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

        public void StartSpin(float initialAngularVelocity, float duration)
        {
            if (model == null) return;
            this.initialAngularVelocity = initialAngularVelocity;
            spinDuration = duration;
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

            if (t >= 1f && Mathf.Abs(angularVelocity) < 0.1f)
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
            if (SpinVisual != null)
                SpinVisual.localRotation = Quaternion.Euler(0f, total, 0);
        }

        public int GetPocketUnderIndicator()
        {
            if (model == null) return -1;

            float normalized = ((currentRotation + rotationOffsetDegrees) % 360f + 360f) % 360f;
            Debug.Log(normalized);
            Debug.Log(GetPocketWorldPosition(0));
            return 0;
        }

        public Vector3 GetPocketWorldPosition(int pocketNumber)
        {
            var pockets = model.PocketDefinitions;
            int count = pockets.Count;
            float sweep = 360f / count;
            var index = -1;
            Vector3 refDir = Vector3.ProjectOnPlane(center.transform.up, center.transform.forward).normalized;
            for (int i = 0; i < count; i++)
            {
                if (pockets[i].Number == pocketNumber)
                {
                    index = i;
                    break;
                }
            }
            
            float startAngle = index * sweep;
            float midAngle = startAngle + sweep * 0.5f;
            
            Vector3 pocketPos = center.position + AngleToDirectionInPlane(midAngle, center.transform.forward, refDir) * (model.PocketCircleRadius);
            return pocketPos;
        }
        private static Vector3 AngleToDirectionInPlane(float degrees, Vector3 planeNormal, Vector3 referenceDir)
        {
            // Rotate referenceDir around planeNormal by degrees
            return Quaternion.AngleAxis(degrees, planeNormal) * referenceDir;
        }
        public event Action<int> OnSpinStopped;
        public event Action OnSpinStarted;
    }

    public interface IWheelView
    {
        void StartSpin(float initialAngularVelocity, float duration);
        void UpdateRotation(float deltaTime);
        int GetPocketUnderIndicator();
        event Action<int> OnSpinStopped;
        event Action OnSpinStarted;
    }
}
