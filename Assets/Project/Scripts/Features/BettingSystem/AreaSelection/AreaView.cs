using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Area
{
    public class AreaView : MonoBehaviour
    {
        [SerializeField]Table table;
        private Camera cameraMain;
        private List<Area> _coveredNumberAreas=new();
        private Area selectedArea;

        private void Start()
        {
            cameraMain = Camera.main;
        }

        public Area TryGetAreaFromPosition(Vector3 position)
        {
            foreach (var area in table.areas)
            {
                if (ContainsPoint(area, position))
                {
                    return area;
                }
            }
            return null;
        }

        private bool ContainsPoint(Area area, Vector3 point)
        {
            var halfW = area.GetSize().x * 0.5f;
            var halfD = area.GetSize().y * 0.5f;

            var areaWorldPoint = AreaWorldPoint(area);
        
            return point.x >= -halfW+areaWorldPoint.x && point.x <= halfW+areaWorldPoint.x && point.z >= -halfD+areaWorldPoint.z && point.z <= halfD+areaWorldPoint.z;
        }

        private Vector3 AreaWorldPoint(Area area)
        {
            return new Vector3(area.GetPosition().x, 0, area.GetPosition().y) + transform.position;
        }

        public void SetHoveredArea(int area)
        {
            selectedArea = table.areas.Find(x=>x.GetID()==area.ToString());
        }
        public void SetCoveredAreasListFromNumbers(List<int> coveredNumbers)
        {
            _coveredNumberAreas.Clear();
            foreach (var coveredNumber in coveredNumbers)
            {
                var area = table.areas.Find(x => x.GetID() == coveredNumber.ToString());
                if(area!=null)
                    _coveredNumberAreas.Add(area);
            }
        }
        private void OnGUI()
        {
            if (_coveredNumberAreas.Count==0) return;

            HoverSelectedAreas();
            CircleDrawSelectedArea();
            
            GUI.color = Color.white; // Sıfırla

        }

        private void CircleDrawSelectedArea()
        {
            if (selectedArea != null)
            {
                Vector3 worldCenter = AreaWorldPoint(selectedArea); // Vector3 (X, Y, Z)
                Vector2 size = selectedArea.GetSize();              // Vector2 (X, Y)
                
                Vector3 screenCenter = cameraMain.WorldToScreenPoint(worldCenter);

                // GUI Y ekseni ters
                screenCenter.y = Screen.height - screenCenter.y;

                // World boyutunu pixel boyutuna çevirmek için iki kenar köşe hesapla
                Vector3 worldCorner = worldCenter + new Vector3(size.x / 2f, 0, size.y / 2f);
                Vector3 screenCorner = cameraMain.WorldToScreenPoint(worldCorner);
                screenCorner.y = Screen.height - screenCorner.y;

                Vector3 pixelSize = new Vector3(Mathf.Abs(screenCorner.x - screenCenter.x) * 2f,
                    Mathf.Abs(screenCorner.y - screenCenter.y) * 2f);

                Rect rect = new Rect(screenCenter - pixelSize / 2f, pixelSize);

                // Renk seçimi
                GUI.color = _coveredNumberAreas.Contains(selectedArea)? new Color(0f, 1f, 0f, 0.8f): new Color(1f, .5f, .2f, 0.8f);
                
                float thickness = 2f; // çerçeve kalınlığı

                // Üst çizgi
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
                // Alt çizgi
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
                // Sol çizgi
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
                // Sağ çizgi
                GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);

                // ID yazısı (orta üstte)
                GUI.color = Color.black;
                GUI.Label(new Rect(rect.x, rect.y - 16, rect.width, 20), selectedArea.GetID(), GUI.skin.label);
            }
        }

        private void HoverSelectedAreas()
        {
            foreach (var area in _coveredNumberAreas)
            {
                Vector3 worldCenter = AreaWorldPoint(area); // Vector3 (X, Y, Z)
                Vector2 size = area.GetSize();              // Vector2 (X, Y)

                // 3D dünya pozisyonunu ekran pozisyonuna çevir
                Vector3 screenCenter = cameraMain.WorldToScreenPoint(worldCenter);

                // Ekran dışında kalanları atla
                if (screenCenter.z < 0) continue;

                // GUI Y ekseni ters
                screenCenter.y = Screen.height - screenCenter.y;

                // World boyutunu pixel boyutuna çevirmek için iki kenar köşe hesapla
                Vector3 worldCorner = worldCenter + new Vector3(size.x / 2f, 0, size.y / 2f);
                Vector3 screenCorner = cameraMain.WorldToScreenPoint(worldCorner);
                screenCorner.y = Screen.height - screenCorner.y;

                Vector3 pixelSize = new Vector3(Mathf.Abs(screenCorner.x - screenCenter.x) * 2f,
                    Mathf.Abs(screenCorner.y - screenCenter.y) * 2f);

                Rect rect = new Rect(screenCenter - pixelSize / 2f, pixelSize);

                // Hover kontrolü (mouse screen space'te)
                Vector2 mouse = Event.current.mousePosition;
                bool isHovered = rect.Contains(mouse);

                // Renk seçimi
                GUI.color = isHovered ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 1f, 0f, 0.15f);

                GUI.DrawTexture(rect, Texture2D.whiteTexture);

                // ID yazısı (orta üstte)
                GUI.color = Color.black;
                GUI.Label(new Rect(rect.x, rect.y - 16, rect.width, 20), area.GetID(), GUI.skin.label);
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (table == null) return;
            foreach (var area in table.areas)
            {
                Gizmos.DrawCube(AreaWorldPoint(area), new Vector3(area.GetSize().x,0,area.GetSize().y));
                Handles.Label(AreaWorldPoint(area), area.GetID());
            }
        }
#endif

    }
}
