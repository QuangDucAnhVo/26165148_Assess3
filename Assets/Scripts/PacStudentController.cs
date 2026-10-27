using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float speed = 5f;
    public LevelGrid levelGrid;
    public AudioClip moveClip;
    public AudioClip eatClip;
    public ParticleSystem dust;
    private Tweener tweener;
    private Animator animator;
    private AudioSource moveAudio;
 
    private KeyCode lastInput = KeyCode.None;
    private KeyCode currentInput = KeyCode.None;
    
    private int row = 1;
    private int col = 1;
 
    void Start()
    {
        tweener = GetComponent<Tweener>();
        animator = GetComponent<Animator>();
        moveAudio = GetComponent<AudioSource>();
        moveAudio.loop = true;
        transform.position = levelGrid.GridToWorld(row, col);
        animator.Play("PacStudent_WalkRight");
        animator.speed = 0f;
    }
 
    void Update()
    {
        GetInput();
        if (!tweener.IsTweening())
        {
            if (TryMove(lastInput))
            {
                currentInput = lastInput;
            }
            else if (!TryMove(currentInput))
            {
                StopMoving();
            }
        }
    }
 
    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastInput = KeyCode.W;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastInput = KeyCode.A;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lastInput = KeyCode.S;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastInput = KeyCode.D;
        }
    }

    bool TryMove(KeyCode key)
    {
        int nextRow = row;
        int nextCol = col;
 
        if (key == KeyCode.W)
        {
            nextRow--;
        }
        else if (key == KeyCode.S)
        {
            nextRow++;
        }
        else if (key == KeyCode.A)
        {
            nextCol--;
        }
        else if (key == KeyCode.D)
        {
            nextCol++;
        }
        else return false;      
 
        if (!levelGrid.IsWalkable(nextRow, nextCol))
        {
            return false;
        }
 
        Vector3 startPos = levelGrid.GridToWorld(row, col);
        Vector3 endPos = levelGrid.GridToWorld(nextRow, nextCol);
        tweener.AddTween(transform, startPos, endPos, 1f / speed);
        
        row = nextRow;
        col = nextCol;
        PlayMoveEffects(key, levelGrid.HasPellet(row, col));
        return true;
    }
 
    void PlayMoveEffects(KeyCode key, bool aboutToEat)
    {
        animator.speed = 1f;
        if (key == KeyCode.W)
        {
            animator.Play("PacStudent_WalkUp");
        }
        else if (key == KeyCode.S)
        {
            animator.Play("PacStudent_WalkDown");
        }
        else if (key == KeyCode.A)
        {
            animator.Play("PacStudent_WalkLeft");
        }
        else
        {
            animator.Play("PacStudent_WalkRight");
        }

        AudioClip clip = moveClip;
        if (aboutToEat)
        {
            clip = eatClip;
        }
        if (moveAudio.clip != clip || !moveAudio.isPlaying)
        {
            moveAudio.clip = clip;
            moveAudio.Play();
        }
        if (!dust.isPlaying)
        {
            dust.Play();
        }
    }
    void StopMoving()
    {
        animator.speed = 0f;
        moveAudio.Stop();
        dust.Stop();
    }
}
