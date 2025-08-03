using UnityEngine;

namespace Game.RouletteSystem
{
    public class RouletteBallView :MonoBehaviour
    {
        public GameObject rouletteBall;
        public AnimationCurve RotationEaseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public float minRotationSpeed = 720;
        public float maxRotationSpeed = 2260;
    }
}