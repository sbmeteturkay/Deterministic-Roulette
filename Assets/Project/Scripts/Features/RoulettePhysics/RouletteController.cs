using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public event Action<int> OnRouletteBallInPocket;
        
        bool _useRandomNumber = false;
        int targetNumber = 0;
        private void Awake()
        {
            rouletteWheelController = new RouletteWheelController(rouletteWheelView, rouletteWheelView.Model);
            rouletteBallController = new RouletteBallController(rouletteWheelController,rouletteBallView);
            rouletteBallController.OnRouletteBallInPocket += RouletteBallControllerOnOnRouletteBallInPocket;
        }

        private void OnDestroy()
        {
            rouletteBallController.OnRouletteBallInPocket -= RouletteBallControllerOnOnRouletteBallInPocket;
        }

        private void RouletteBallControllerOnOnRouletteBallInPocket(int outcome)
        {
            OnRouletteBallInPocket?.Invoke(outcome);
        }

        public void Spin()
        {
            StopAllCoroutines();
            SpinWheel();
            SpinBall();
        }
        void SpinWheel()
        {
            StartCoroutine(rouletteWheelController.StartSpin());
        }
        void SpinBall()
        {
            targetNumber = _useRandomNumber?rouletteWheelController.GetRandomPocketNumber():targetNumber;
            rouletteWheelController.SetSelectedPocket(targetNumber);
            StartCoroutine(rouletteBallController.StartSpinBall(targetNumber));
        }

        public void SetTargetOutCome(int number)
        {
            Debug.Log($"SetTargetOutCome: {number}");
            targetNumber = number;
        }

        public List<int> ReturnNumbers()
        {
            List<int> numbers = rouletteWheelView.Model.PocketDefinitions.Select(item => item.Number).ToList();
            numbers.Sort();
            return numbers;
        }
        public void UseRandomNumber(bool useRandomNumber)
        {
            _useRandomNumber = useRandomNumber;
        }

    }

}