using UnityEngine;

namespace Game.RouletteSystem.Game.RouletteSystem
{
    public class RouletteWheelTester : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private RouletteWheelModelSO modelSO;
        [SerializeField] private RouletteWheelView viewComponent;

        private RouletteWheelController controller;

        private void Awake()
        {
            if (modelSO == null || viewComponent == null)
            {
                Debug.LogError("Tester: modelSO ve viewComponent inspector'dan assign edilmeli.");
                enabled = false;
                return;
            }

            // Initialize wiring
            viewComponent.Initialize(modelSO, viewComponent.Center);
            controller = new RouletteWheelController(viewComponent, modelSO);

            controller.OnSpinStarted += () => Debug.Log("[Tester] Spin started");
            controller.OnIntermediatePocketChanged += pocket => Debug.Log($"[Tester] Intermediate pocket: {pocket}");
            controller.OnSpinStopped += pocket => Debug.Log($"[Tester] Spin stopped on pocket: {pocket}");
        }

        private void Update()
        {
            // Drive view rotation
            viewComponent.UpdateRotation(Time.deltaTime);

            // Manual trigger
            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.StartSpin(null);
            }
        }
    }
}