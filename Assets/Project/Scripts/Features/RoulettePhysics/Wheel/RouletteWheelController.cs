using UnityEngine;
using System;
using System.Collections;
using Random = System.Random;

namespace Game.RouletteSystem
{
    public class RouletteWheelController : IWheelController
    {
        private readonly IWheelView view;
        private readonly RouletteWheelModelSO model;

        public event Action OnSpinStarted;
        public event Action OnSpinStopped;
        
        private int selectedPocked;
        public RouletteWheelController(IWheelView view, RouletteWheelModelSO model)
        {
            this.view = view;
            this.model = model;
        }

        public IEnumerator StartSpin()
        {
            // 2. Rastgele süre, model'den al (fallback değerler)
            float minDuration = model.MinSpinDuration;
            float maxDuration = model.MaxSpinDuration;
            float minSpinAngularVelocity = model.MinSpinAngularVelocity;
            float maxSpinAngularVelocity = model.MaxSpinAngularVelocity;
            float duration = UnityEngine.Random.Range(minDuration, maxDuration);

            float initialAngularVelocity = UnityEngine.Random.Range(minSpinAngularVelocity, maxSpinAngularVelocity);

            yield return UpdateRotation(duration,initialAngularVelocity);
        }
        
        public void StopSpin()
        {
            // Doğal easing ile durduğu için burada bir şey yapmıyoruz.
            // Gerekirse hızlı durdurma için view'a ek API gerekir.
        }
        
        public int GetRandomPocketNumber()
        {
            selectedPocked = model.PocketDefinitions[UnityEngine.Random.Range(0,model.PocketDefinitions.Count)].Number;
            Debug.Log("pocket selected: "+selectedPocked);
            return selectedPocked;
        }

        public Vector3 GetPocketPosition()
        {
            return view.GetPocketWorldPosition(model.PocketDefinitions[selectedPocked].Number);
        }
        public float GetBowlRadius()
        {
            return model.BowlRadius;
        }

        public float GetDeflectorCircleRadius()
        {
            return model.DeflectorsRadiusCircle;
        }

        public Transform GetWheelCenter()
        {
            return view.GetWheelCenter();
        }
        private IEnumerator UpdateRotation(float duration, float initialAngularVelocity)
        {
            float elapsed = 0f;
            float currentRotation = 0f;
            
            while (true)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float ease = model.RotationEaseCurve.Evaluate(t);
                float angularVelocity = initialAngularVelocity * ease;

                currentRotation += angularVelocity * Time.deltaTime;
                view.GetSpinVisual().localRotation = Quaternion.Euler(0f, currentRotation, 0f);

                if (t >= 1f)
                    break;

                yield return null;
            }

            OnSpinStopped?.Invoke();
        }

        
    }
    public interface IWheelController
    {
        IEnumerator StartSpin();
        void StopSpin();
        event Action OnSpinStarted;
        event Action OnSpinStopped;
    }
}
