using System;
using UnityEngine;

namespace Game.Core.Area
{
    [Serializable]
    public class Area
    {
        [SerializeField] private Vector2 cellSize = Vector2.one;
        [SerializeField] private Vector2 position;
        [SerializeField] private string ID;
        public Vector2 GetPosition() => position;
        public Vector2 GetSize() => cellSize;
        public string GetID() => ID;
    }
}