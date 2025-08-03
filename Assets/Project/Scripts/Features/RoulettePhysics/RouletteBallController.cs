using System.Collections;
using UnityEngine;

public class RouletteBallController
{
    private RouletteWheelView rouletteWheelView;

    public RouletteBallController(RouletteWheelView rouletteWheelView)
    {
        this.rouletteWheelView = rouletteWheelView;
    }
    public IEnumerator BallFreeFall()
    {
            
        var duration=Random.Range(4f, 8f);
        var outerRadius = rouletteWheelView.Model.BowlRadius;
        var innerRadius = rouletteWheelView.Model.DeflectorsRadiusCircle;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Radius: dıştan içe (linear)
            float currentRadius = Mathf.Lerp(outerRadius, innerRadius, t);

            // Açısal konum (radyan)
            float angleDeg = 60 * elapsedTime;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * currentRadius;
            ball.transform.position = rouletteWheelView.Center.position + offset;
            yield return null;
        }
           
    }
}