using System;
using UnityEngine;

namespace Game.RouletteSystem
{
    public class RouletteController : MonoBehaviour
    {
        [Header("Views")] 
        [SerializeField] private RouletteWheelView rouletteWheelView;
        [SerializeField] private RouletteBallView rouletteBallView;

        private RouletteWheelController rouletteWheelController;
        private RouletteBallController rouletteBallController;

        private void Awake()
        {
            rouletteWheelController = new RouletteWheelController(rouletteWheelView, rouletteWheelView.Model);
            rouletteBallController = new RouletteBallController(rouletteWheelController,rouletteBallView);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                SpinBall();
                SpinWheel();
            }
        }

        void SpinWheel()
        {
            StartCoroutine(rouletteWheelController.StartSpin());
        }
        void SpinBall()
        {
            StartCoroutine(rouletteBallController.StartSpinBall(rouletteWheelController.GetRandomPocketNumber()));
        }

    }

}