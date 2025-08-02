using UnityEngine;

namespace Game.RouletteSystem
{
    public class RouletteController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private RouletteWheelView rouletteWheelView;
        
        
        private RouletteWheelController controller;

        private void Awake()
        {
            controller = new RouletteWheelController(rouletteWheelView, rouletteWheelView.Model);
        }
    }

}
