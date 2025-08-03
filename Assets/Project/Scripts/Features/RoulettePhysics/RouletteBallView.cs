using UnityEngine;

namespace Game.RouletteSystem
{
    public class RouletteBallView :MonoBehaviour
    {
        public GameObject rouletteBall;
        public AnimationCurve RotationEaseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    }
}