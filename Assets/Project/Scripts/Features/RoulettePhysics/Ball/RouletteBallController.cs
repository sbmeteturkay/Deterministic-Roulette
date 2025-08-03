using System.Collections;
using UnityEngine;

namespace Game.RouletteSystem
{
    public class RouletteBallController
    {
        private RouletteWheelController rouletteWheelController;
        private RouletteBallView rouletteBallView;
        public RouletteBallController(RouletteWheelController rouletteWheelController, RouletteBallView rouletteBallView)
        {
            this.rouletteWheelController = rouletteWheelController;
            this.rouletteBallView = rouletteBallView;
        }
        public IEnumerator StartSpinBall(int randomPocketNumber)
        {
            var rouletteBall = rouletteBallView.rouletteBall;
            
            var duration=Random.Range(4f, 16f);

            var outerRadius = rouletteWheelController.GetBowlRadius();
            var innerRadius = rouletteWheelController.GetDeflectorCircleRadius();
            
            var initialSpeed=Random.Range(rouletteBallView.minRotationSpeed, rouletteBallView.maxRotationSpeed);
            var slowCurve = rouletteBallView.RotationEaseCurve;

            var angleDeg = 0f;
            
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                // Radius: dıştan içe (linear)
                float currentRadius = Mathf.Lerp(outerRadius, innerRadius, t);

                // Açısal konum (radyan)
                angleDeg += (initialSpeed* slowCurve.Evaluate(t)) * Time.deltaTime ;
                float angleRad = angleDeg * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(angleRad), .1f, Mathf.Sin(angleRad)) * currentRadius;
                rouletteBall.transform.position = rouletteWheelController.GetWheelCenter().position + offset;
                yield return null;
            }

            yield return BallJumpIntoPocket();
        }
        private IEnumerator BallJumpIntoPocket()
        {
            var rouletteBall = rouletteBallView.rouletteBall;

            // Toplam zıplama sayısı
            int jumps = Random.Range(1, 5);

            // Her zıplamada hedef yeniden hesaplanacak
            for (int i = 0; i < jumps; i++)
            {
                Vector3 currentPos = rouletteBall.transform.position;
                Vector3 target = rouletteWheelController.GetPocketPosition();
                Vector3 toTarget = target - currentPos;

                // Kalan zıplamalara göre bir sonraki ara nokta
                int remainingJumps = jumps - i;
                // Bu zıplamada gidilecek frac: 1 / remainingJumps
                float frac = 1f / remainingJumps;
                Vector3 nextWaypoint = currentPos + toTarget * frac;

                // Yükseklik rastgele (arc)
                float height = Random.Range(0f, .2f);
                float durationThis = Mathf.Max(0.01f, height * 2f); // sıfır süreden kaçın
                float time = 0f;

                while (time < durationThis)
                {
                    time += Time.deltaTime;
                    float normalized = Mathf.Clamp01(time / durationThis);
                    float arc = 4f * height * normalized * (1f - normalized);
                    Vector3 basePos = Vector3.Lerp(currentPos, nextWaypoint, normalized);
                    rouletteBall.transform.position = basePos + Vector3.up * arc;
                    yield return null;
                }

                // Kesinleştir
                rouletteBall.transform.parent = rouletteWheelController.GetWheelCenter();
                rouletteBall.transform.position = nextWaypoint;
            }

            // Son olarak (hedef çok hareket ettiyse) güncel hedefe zıplama yapmadan yerleştir
            Vector3 finalTarget = rouletteWheelController.GetPocketPosition();
            rouletteBall.transform.position = finalTarget;
        }

    }
}
