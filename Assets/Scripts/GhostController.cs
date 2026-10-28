using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum GhostState { Normal, Scared, Recovering, Dead }
    public GhostState state = GhostState.Normal;
    private Animator animator;
    private Vector3 startPosition;
 
    void Awake()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }
 
    void Start()
    {
        SetState(GhostState.Normal);
    }

    public void SetState(GhostState newState)
    {
        state = newState;
        if (state == GhostState.Normal)
        {
            animator.Play("Normal_Down");
        }
        else if (state == GhostState.Scared)
        {
            animator.Play("Scared_Down");
        }
        else if (state == GhostState.Recovering)
        {
            animator.Play("Recovering");
        }
        else
        {
            animator.Play("Ghost_Dead");
        }
    }
    public void GhostReset()
    {
        transform.position = startPosition;
        SetState(GhostState.Normal);
    }
}
