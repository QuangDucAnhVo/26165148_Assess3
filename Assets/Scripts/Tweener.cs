using UnityEngine;

public class Tweener : MonoBehaviour
{
    private Tween activeTween;

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

    public bool AddTween(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (IsTweening())
        {
            return false;
        }

        activeTween = new Tween(target, startPos, endPos, Time.time, duration);
        return true;
    }

    public bool IsTweening()
    {
        return activeTween != null && Time.time - activeTween.StartTime < activeTween.Duration;
    }
}
