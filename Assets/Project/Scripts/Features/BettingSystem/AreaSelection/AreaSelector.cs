using System;
using UnityEngine;

namespace Game.Core.Area
{
    [RequireComponent(typeof(AreaView))]
    public class AreaSelector : MonoBehaviour
    {
        [SerializeField] AreaView _areaView;
        [SerializeField] private LayerMask tableMask; // masanın collider'ı burada olmalı
        public event Action<string> OnSelectArea;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 10, tableMask))
                {
                    // Masaya çarptı, hangi alansa bul
                    OnSelectArea?.Invoke(_areaView.TryGetAreaFromPosition(hit.point)?.GetID());
                }
            }
        }
    }
}