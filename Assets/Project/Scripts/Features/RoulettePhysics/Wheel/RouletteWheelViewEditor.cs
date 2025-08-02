#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Game.RouletteSystem
{
    [CustomEditor(typeof(RouletteWheelView))]
    public class RouletteWheelViewEditor : Editor
    {
        private RouletteWheelView view;

        private void OnEnable()
        {
            view = (RouletteWheelView)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void DrawWheelGizmos(RouletteWheelView view, GizmoType gizmoType)
        {
            var model = view.Model;
            var center = view.Center;
            if (model == null || center == null) return;

            Vector3 pos = center.position;
            float radius = model.Radius;
            float pocketRadius = model.PocketCircleRadius;

            // Determine plane normal: assume wheel lies such that its local forward is the axis (adjust if your wheel's axis differs)
            // If your wheel rotates around its local Z, use view.transform.forward as normal; if around X/Y adjust accordingly.
            Vector3 planeNormal = center.transform.forward; // çarkın "axis"ı burası; gerektiğinde değiştirilebilir

            // Choose a reference direction in-plane: take transform.up projected onto plane
            Vector3 refDir = Vector3.ProjectOnPlane(center.transform.up, planeNormal).normalized;
            if (refDir.sqrMagnitude < 0.001f)
                refDir = Vector3.ProjectOnPlane(center.transform.right, planeNormal).normalized;
            if (refDir.sqrMagnitude < 0.001f)
                refDir = Vector3.right; // fallback

            // Draw outer circle in that plane
            Handles.color = Color.white;
            Handles.DrawWireDisc(pos, planeNormal, radius);
            
            Handles.color = Color.white;
            Handles.DrawWireDisc(pos, planeNormal, pocketRadius);

            var pockets = model.PocketDefinitions;
            int count = pockets.Count;
            float sweep = 360f / count;

            // Get offset if exists
            float offset = 0f;
            var rotationOffsetField = typeof(RouletteWheelView).GetField("rotationOffsetDegrees", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (rotationOffsetField != null)
                offset = (float)rotationOffsetField.GetValue(center);

            for (int i = 0; i < count; i++)
            {
                var pocket = pockets[i];
                float startAngle = i * sweep + offset;
                float midAngle = startAngle + sweep * 0.5f;

                // Separator line
                Vector3 dirStart = AngleToDirectionInPlane(startAngle, planeNormal, refDir);
                Handles.color = Color.gray;
                Handles.DrawLine(pos, pos + dirStart * radius);

                // Label
                Vector3 labelPos = pos + AngleToDirectionInPlane(midAngle, planeNormal, refDir) * (radius * 0.9f);
                GUIStyle style = new GUIStyle();
                style.fontSize = 16;
                style.normal.textColor = pocket.DisplayColor;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(labelPos, pocket.Number.ToString(), style);
                
                //pocket indicator
                
                Vector3 pocketPos = pos + AngleToDirectionInPlane(midAngle, planeNormal, refDir) * (pocketRadius);
                Handles.SphereHandleCap(0,pocketPos, Quaternion.identity, .01f, EventType.Repaint);
            }

            // Indicator: top in wheel's local reference (angle zero with offset)
            Vector3 indicatorDir = AngleToDirectionInPlane(0 + offset, planeNormal, refDir);
            Handles.color = Color.yellow;
            Handles.DrawLine(pos + indicatorDir * (radius + 0.1f), pos + indicatorDir * (radius + 0.3f));
        }

        private static Vector3 AngleToDirectionInPlane(float degrees, Vector3 planeNormal, Vector3 referenceDir)
        {
            // Rotate referenceDir around planeNormal by degrees
            return Quaternion.AngleAxis(degrees, planeNormal) * referenceDir;
        }

        private static Vector3 AngleToDirection(float degrees)
                {
                    float rad = degrees * Mathf.Deg2Rad;
                    return new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);
                }
            }
        }
#endif
