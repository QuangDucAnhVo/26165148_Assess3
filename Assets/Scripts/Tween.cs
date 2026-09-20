using UnityEngine;

public class Tween
{
    public Transform Target;
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public float StartTime;
    public float Duration;

    public Tween(Transform target, Vector3 startPos, Vector3 endPos, float startTime, float duration)
    {
        Target = target;
        StartPosition = startPos;
        EndPosition = endPos;
        StartTime = startTime;
        Duration = duration;
    }
}
