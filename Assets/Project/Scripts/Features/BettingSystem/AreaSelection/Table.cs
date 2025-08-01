using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Area
{
    [CreateAssetMenu(fileName = "Table", menuName = "ScriptableObjects/Table", order = 1)]
    public class Table : ScriptableObject
    {
        public List<Area> areas = new();
    }
}