using UnityEngine;
using System.Collections.Generic;

namespace Game.RouletteSystem
{
    public enum WheelType { European, American }

    [System.Serializable]
    public class PocketDefinition
    {
        public int Number;
        public Color DisplayColor;
        [HideInInspector] public float StartAngle; // computed
        [HideInInspector] public float SweepAngle; // computed
    }

    [CreateAssetMenu(menuName = "RouletteSystem/RouletteWheelModel")]
    public class RouletteWheelModelSO : ScriptableObject, IRouletteWheelModel
    {
        [Header("Basic Wheel")]
        public WheelType WheelType = WheelType.European;
        public float Radius = 1f;
        public float PocketRadiusCircle = 1f;

        [Header("Spin Settings")]
        public AnimationCurve RotationEaseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public float MinSpinAngularVelocity = 720f; // degrees per second
        public float MaxSpinAngularVelocity = 1440f;
        [Header("Spin Duration (seconds)")]
        public float MinSpinDuration = 2f;
        public float MaxSpinDuration = 4f;

        [Header("Pocket Definitions (readonly order)")]
        [SerializeField] private List<PocketDefinition> pocketDefinitions = new();

        private IReadOnlyList<PocketDefinition> cached;

        public WheelType WheelTypeProp => WheelType;
        public float RadiusProp => Radius;
        public float PocketRadiusProp => PocketRadiusCircle;
        public AnimationCurve RotationEaseCurveProp => RotationEaseCurve;
        public float MinSpinAngularVelocityProp => MinSpinAngularVelocity;
        public float MaxSpinAngularVelocityProp => MaxSpinAngularVelocity;
        public IReadOnlyList<PocketDefinition> PocketDefinitions => GetOrCreatePocketDefinitions();

        private IReadOnlyList<PocketDefinition> GetOrCreatePocketDefinitions()
        {
            if (cached != null) return cached;
            // If not populated manually, generate standard sequence.
            if (pocketDefinitions == null || pocketDefinitions.Count == 0)
            {
                pocketDefinitions = GenerateDefaultSequence();
            }

            PrecomputeAngles(pocketDefinitions);
            cached = pocketDefinitions;
            return cached;
        }

        private List<PocketDefinition> GenerateDefaultSequence()
        {
            // Standard European/American sequences:
            if (WheelType == WheelType.European)
                return GenerateSequence(new int[] {
                    0,32,15,19,4,21,2,25,17,34,6,27,13,36,11,30,8,
                    23,10,5,24,16,33,1,20,14,31,9,22,18,29,7,28,
                    12,35,3,26
                });
            else // American
                return GenerateSequence(new int[] {
                    0,28,9,26,30,11,7,20,32,17,5,22,34,15,3,24,36,
                    13,1,00,27,10,25,29,12,8,19,31,18,6,21,33,16,4,
                    23,35,14,2
                });
        }

        private List<PocketDefinition> GenerateSequence(int[] numbers)
        {
            var list = new List<PocketDefinition>();
            foreach (var n in numbers)
            {
                var def = new PocketDefinition
                {
                    Number = n,
                    DisplayColor = DetermineColor(n)
                };
                list.Add(def);
            }
            return list;
        }

        private Color DetermineColor(int number)
        {
            // Simplified: 0 and 00 -> green, others alternating red/black based on standard rules.
            if (number == 0 || number == 00) return Color.green;
            // A true mapping would use the known red/black sets; placeholder:
            return (number % 2 == 0) ? Color.black : Color.red;
        }

        private void PrecomputeAngles(List<PocketDefinition> list)
        {
            int count = list.Count;
            float sweep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                var def = list[i];
                def.SweepAngle = sweep;
                def.StartAngle = i * sweep;
            }
        }

        // Interface implementations
        WheelType IRouletteWheelModel.WheelType => WheelTypeProp;
        float IRouletteWheelModel.Radius => RadiusProp;
        public float PocketCircleRadius => PocketRadiusProp;
        AnimationCurve IRouletteWheelModel.RotationEaseCurve => RotationEaseCurveProp;
        float IRouletteWheelModel.MinSpinAngularVelocity => MinSpinAngularVelocityProp;
        float IRouletteWheelModel.MaxSpinAngularVelocity => MaxSpinAngularVelocityProp;
        IReadOnlyList<PocketDefinition> IRouletteWheelModel.PocketDefinitions => PocketDefinitions;
    }

    public interface IRouletteWheelModel
    {
        WheelType WheelType { get; }
        float Radius { get; }
        float PocketCircleRadius { get; }
        AnimationCurve RotationEaseCurve { get; }
        float MinSpinAngularVelocity { get; }
        float MaxSpinAngularVelocity { get; }
        IReadOnlyList<PocketDefinition> PocketDefinitions { get; }
    }
}
