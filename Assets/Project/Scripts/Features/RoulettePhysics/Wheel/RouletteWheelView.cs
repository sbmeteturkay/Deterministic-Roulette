using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Serialization;

namespace Game.RouletteSystem
{
    public class RouletteWheelView : MonoBehaviour, IWheelView
    {
        [Header("Model")]
        [SerializeField] private RouletteWheelModelSO modelSO;
        [Header("References")]
        [SerializeField] private Transform center;
        [SerializeField, Tooltip("Görsel olarak dönecek nesne.")]
        public Transform SpinVisual;

        internal RouletteWheelModelSO Model => modelSO;
        internal Transform Center => center;
        
        public Vector3 GetPocketWorldPosition(int pocketNumber)
        {
            var pockets = Model.PocketDefinitions;
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
            
            Vector3 pocketPos = center.position + AngleToDirectionInPlane(midAngle, center.transform.forward, refDir) * (Model.PocketRadiusCircle);
            return pocketPos;
        }
        private static Vector3 AngleToDirectionInPlane(float degrees, Vector3 planeNormal, Vector3 referenceDir)
        {
            // Rotate referenceDir around planeNormal by degrees
            return Quaternion.AngleAxis(degrees, planeNormal) * referenceDir;
        }

        public Transform GetWheelCenter()
        {
            return center;
        }

        public Transform GetSpinVisual()
        {
            return SpinVisual;
        }
    }

    public interface IWheelView
    {
        Vector3 GetPocketWorldPosition(int pocketNumber);
        Transform GetWheelCenter();
        Transform GetSpinVisual();
    }
}
