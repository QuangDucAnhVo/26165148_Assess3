using UnityEngine;

public class Tweener : MonoBehaviour
{
    private Tween activeTween;
    // Update is called once per frame
    void Update()
    {
        if (activeTween != null)
        {
            float timeFraction = (Time.time - activeTween.StartTime) / activeTween.Duration;

            if (timeFraction < 1.0f)
            {
                activeTween.Target.position = Vector3.Lerp(activeTween.StartPos, activeTween.EndPos, timeFraction);
            }
            else
            {
                activeTween.Target.position = activeTween.EndPos;
                activeTween = null;
            }
        }
        
    }
}
