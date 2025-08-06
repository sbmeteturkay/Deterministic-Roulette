using UnityEngine;
using System.Collections;
using System.Linq;

namespace Game.RouletteSystem
{
    public class RouletteWheelController : IWheelController
    {
        private readonly IWheelView view;
        private readonly RouletteWheelModelSO model;

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
            float duration = Random.Range(minDuration, maxDuration);

            float initialAngularVelocity = Random.Range(minSpinAngularVelocity, maxSpinAngularVelocity);

            yield return UpdateRotation(duration,initialAngularVelocity);
        }
        
        public void StopSpin()
        {
            // Doğal easing ile durduğu için burada bir şey yapmıyoruz.
            // Gerekirse hızlı durdurma için view'a ek API gerekir.
        }
        
        public int GetRandomPocketNumber()
        {
            selectedPocked = model.PocketDefinitions[Random.Range(0,model.PocketDefinitions.Count)].Number;
            Debug.Log("pocket selected: "+selectedPocked);
            return selectedPocked;
        }

        public void SetSelectedPocket(int pocketNumber)
        {
            selectedPocked = pocketNumber;
        }

        public Vector3 GetPocketPosition()
        {
            return view.GetPocketWorldPosition(selectedPocked);
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
            float currentRotation = view.GetSpinVisual().localEulerAngles.y;
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
        }

        
    }
    public interface IWheelController
    {
        IEnumerator StartSpin();
        void StopSpin();
    }
}
