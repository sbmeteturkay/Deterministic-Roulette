using UnityEditor;
using UnityEngine;

public class AreaView : MonoBehaviour
{
    [SerializeField]Table table;


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