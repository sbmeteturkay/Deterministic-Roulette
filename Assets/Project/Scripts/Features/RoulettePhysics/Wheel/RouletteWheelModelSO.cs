using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

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
    public class RouletteWheelModelSO : ScriptableObject
    {
        [Header("Roulette Wheel")]
        public WheelType WheelType = WheelType.European;
        public float BowlRadius = 2f;
        public float DeflectorsRadiusCircle = 1f;
        public float WheelRadius = 1f;
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
        
        public IReadOnlyList<PocketDefinition> PocketDefinitions => GetOrCreatePocketDefinitions();

        private IReadOnlyList<PocketDefinition> GetOrCreatePocketDefinitions()
        {
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
                    DisplayColor = AssignColor(n)
                };
                list.Add(def);
            }
            return list;
        }

        public Color AssignColor(int number)
        {

            // Sıfırlar yeşil
            if (number == 0)
            {
                return Color.green;
            }

            // Kırmızı mı?
            if (redNumbers.Contains(number))
            {
                return Color.red;
            }
            else
            {
                // Numara geçerli bir sayı mı?: default black
                return Color.black;
            }
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
        private readonly HashSet<int> redNumbers = new HashSet<int>
        {
            1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
        };
    }
}
