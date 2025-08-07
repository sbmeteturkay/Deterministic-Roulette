using System;
using System.Collections;
using RouletteGame.Service;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.RouletteSystem
{
    public class RouletteBallController
    {
        private RouletteWheelController rouletteWheelController;
        private RouletteBallView rouletteBallView;
        public event Action OnRouletteBallInPocket;
        public RouletteBallController(RouletteWheelController rouletteWheelController, RouletteBallView rouletteBallView)
        {
            this.rouletteWheelController = rouletteWheelController;
            this.rouletteBallView = rouletteBallView;
        }
        public IEnumerator StartSpinBall()
        {
            ServiceLocator.SoundService.PlaySfx("ball-roll");
            var rouletteBall = rouletteBallView.rouletteBall;
            
            rouletteBall.SetActive(true);
            var duration=Random.Range(4f, 16f);

            var outerRadius = rouletteWheelController.GetBowlRadius();
            var innerRadius = rouletteWheelController.GetDeflectorCircleRadius();
            
            var initialSpeed=Random.Range(rouletteBallView.minRotationSpeed, rouletteBallView.maxRotationSpeed);
            var slowCurve = rouletteBallView.RotationEaseCurve;

            var angleDeg = 0f;
            
            var elapsedTime = 0f;
            
            rouletteBall.transform.parent = rouletteBallView.transform;

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
            ServiceLocator.SoundService.StopSfx("ball-roll");
            yield return BallJumpIntoPocket();
        }
        private IEnumerator BallJumpIntoPocket()
        {
            //deflector hit
            ServiceLocator.SoundService.PlaySfx("ball-hit");
            
            var rouletteBall = rouletteBallView.rouletteBall;

            // Toplam zıplama sayısı
            int jumps = Random.Range(3, 6);
            float time = 0f;
            // Her zıplamada hedef yeniden hesaplanacak
            for (int i = 0; i < jumps; i++)
            {
                if (i >1)
                    rouletteBall.transform.parent = rouletteWheelController.GetWheelCenter();
                
                Vector3 currentPos = rouletteBall.transform.position;
                Vector3 target = rouletteWheelController.GetPocketPosition();
                Vector3 toTarget = target - currentPos;

                // Kalan zıplamalara göre bir sonraki ara nokta
                int remainingJumps = jumps - i;
                // Bu zıplamada gidilecek frac: 1 / remainingJumps
                float frac = 1f / remainingJumps;
                Vector3 nextWaypoint = currentPos + toTarget * frac;

                // Yükseklik rastgele (arc)
                float height = Random.Range(.1f, .3f);
                float durationThis = Mathf.Max(0.4f, height * 2f); // sıfır süreden kaçın
                time = 0f;

                while (time < durationThis)
                {
                    if (i == jumps - 1)
                    {
                        // //cebe oturt
                        target = rouletteWheelController.GetPocketPosition();
                        target.y -= .01f;
                        toTarget = target - currentPos;
                        nextWaypoint = currentPos + toTarget * frac;
                        ServiceLocator.SoundService.PlaySfx("ball-inpocket");
                    }
                    time += Time.deltaTime;
                    float normalized = Mathf.Clamp01(time / durationThis);
                    float arc = 4f * height * normalized * (1f - normalized);
                    Vector3 basePos = Vector3.Lerp(currentPos, nextWaypoint, normalized);
                    rouletteBall.transform.position = basePos + Vector3.up * arc;
                    yield return null;
                }
                //jump sound
                ServiceLocator.SoundService.PlaySfx("ball-hit");
            }
            yield return SettleWithAnimation(rouletteBall);
        }

        private IEnumerator SettleWithAnimation(GameObject rouletteBall)
        {
            
            float time;
            var settlePosition=rouletteBall.transform.localPosition;
            var settleTime= Random.Range(1, 3);
            var wobbleMagnitude = .01f; // Ne kadar sallanacağı (unit cinsinden)
            var wobbleSpeed = 20f;       // Ne kadar hızlı sallanacağı
            time = 0f;
            while (time < settleTime)
            {
                time += Time.deltaTime;
                
                // Sinüs bazlı yalpalama (x ve y eksenlerinde hafifçe)
                float t = time / settleTime;
                float fade = 1f - t; // zaman ilerledikçe 1'den 0'a düşer

                float wobbleX = Mathf.Sin(time * wobbleSpeed) * wobbleMagnitude * fade;
                float wobbleZ = Mathf.Cos(time * wobbleSpeed) * wobbleMagnitude * fade;

                rouletteBall.transform.localPosition = settlePosition + new Vector3(wobbleX, wobbleZ, 0);
                yield return null;
            }
            OnRouletteBallInPocket?.Invoke();
        }
    }
}
