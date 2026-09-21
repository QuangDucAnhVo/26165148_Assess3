using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float speed;
    public AudioClip moveClip;
    public AudioSource moveAudio;
    public Tweener tweener;
    private Animator animator;
    public Vector3[] corners;
    private int currentCorner = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tweener = GetComponent<Tweener>();
        animator = GetComponent<Animator>();
        
        transform.position = corners[0];
        
        moveAudio.clip = moveClip;
        moveAudio.loop = true;
        moveAudio.Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (!tweener.IsTweening())
        {
            int nextCorner = (currentCorner + 1) % corners.Length;
            Vector3 start = corners[currentCorner];
            Vector3 end = corners[nextCorner];

            float duration = Vector3.Distance(start, end) / speed;
            tweener.AddTween(transform, start, end, duration);

            WalkAnimation(end - start);
            currentCorner = nextCorner;
        }
    }

    void WalkAnimation(Vector3 direction)
    {
        if (direction.x > 0)
        {
            animator.Play("PacStudent_WalkRight");
        }
        else if (direction.x < 0)
        {
            animator.Play("PacStudent_WalkLeft");
        }
        else if (direction.y > 0)
        {
            animator.Play("PacStudent_WalkUp");
        }
        else
        {
            animator.Play("PacStudent_WalkDown");
        }
    }
}
