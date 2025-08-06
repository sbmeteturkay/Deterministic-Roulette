using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Area
{
    [RequireComponent(typeof(AreaView))]
    public class AreaSelector : MonoBehaviour
    {
        [SerializeField] AreaView _areaView;
        [SerializeField] private LayerMask tableMask; // masanın collider'ı burada olmalı
        private Camera camera1;
        public event Action<string> OnSelectArea;

        private void Start()
        {
            camera1 = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = camera1.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 10, tableMask))
                {
                    // Masaya çarptı, hangi alansa bul
                    OnSelectArea?.Invoke(_areaView.TryGetAreaFromPosition(hit.point)?.GetID());
                }
            }
        }

        public void SetCoveredAreaNumbers(List<int> coveredAreaNumbers)
        {
            _areaView.SetCoveredAreasListFromNumbers(coveredAreaNumbers);
        }

        public void SetHoveredArea(int hoveredAreaNumber)
        {
            _areaView.SetHoveredArea(hoveredAreaNumber);
        }
    }
}