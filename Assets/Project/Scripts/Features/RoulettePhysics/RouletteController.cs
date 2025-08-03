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
        
        bool _generateRandomNumber = false;
        int targetNumber = 0;
        private void Awake()
        {
            rouletteWheelController = new RouletteWheelController(rouletteWheelView, rouletteWheelView.Model);
            rouletteBallController = new RouletteBallController(rouletteWheelController,rouletteBallView);
        }

        public void Spin()
        {
            StopAllCoroutines();
            SpinBall();
            SpinWheel();
        }
        void SpinWheel()
        {
            StartCoroutine(rouletteWheelController.StartSpin());
        }
        void SpinBall()
        {
            StartCoroutine(rouletteBallController.StartSpinBall(_generateRandomNumber?rouletteWheelController.GetRandomPocketNumber():targetNumber));
        }

        public void SetTargetOutCome(int number)
        {
            targetNumber = number;
        }

        public void SetRandomNumber(bool generateRandomNumber)
        {
            _generateRandomNumber = generateRandomNumber;
        }

    }

}