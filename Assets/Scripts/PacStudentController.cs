using UnityEditor.Build.Content;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float speed = 5f;
    public GameManager gameManager;
    public LevelGrid levelGrid;
    public AudioClip moveClip;
    public AudioClip eatClip;
    public ParticleSystem dust;
    public ParticleSystem wallHit;
    public ParticleSystem deathEffect;
    private Tweener tweener;
    private Animator animator;
    private AudioSource moveAudio;
    private Collider2D pacCollider;
 
    private KeyCode lastInput = KeyCode.None;
    private KeyCode currentInput = KeyCode.None;
    
    private int row = 1;
    private int col = 1;
    private bool isMoving = false;
 
    void Awake()
    {
        tweener = GetComponent<Tweener>();
        animator = GetComponent<Animator>();
        moveAudio = GetComponent<AudioSource>();
        moveAudio.loop = true;
        pacCollider = GetComponent<Collider2D>();
        pacCollider.enabled = false;
    }

    void Start()
    {
        Respawn();
    }
 
    void Update()
    {
        if (GameManager.gameState != GameManager.GameState.Playing)
        {
            return;
        }
        GetInput();
        if (!tweener.IsTweening())
        {
            if (TryMove(lastInput))
            {
                currentInput = lastInput;
            }
            else if (!TryMove(currentInput))
            {
                if (isMoving)
                {
                    HitWall();
                }
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
        
        if (nextCol < 0)
        {
            Teleport(27);
            nextCol = 26;
        }
        else if (nextCol > 27)
        {
            Teleport(0);
            nextCol = 1;
        }
 
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

    void Teleport(int newCol)
    {
        col = newCol;
        transform.position = levelGrid.GridToWorld(row, col);
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
        if (!isMoving || moveAudio.clip != clip)
        {
            moveAudio.clip = clip;
            moveAudio.Play();
        }
        if (!isMoving)
        {
            dust.Play();
        }
        isMoving = true;
    }
    void StopMoving()
    {
        if (isMoving)
        {
            animator.speed = 0f;
            moveAudio.Stop();
            dust.Stop();
            isMoving = false;
        }
    }
    void HitWall()
    {
        wallHit.transform.position = levelGrid.GridToWorld(row, col) + KeyToDirection(currentInput) * 0.5f;
        wallHit.Play();
        gameManager.PlayWallSound();
    }
 
    Vector3 KeyToDirection(KeyCode key)
    {
        if (key == KeyCode.W)
        {
            return Vector3.up;
        }
        if (key == KeyCode.S)
        {
            return Vector3.down;
        }
        if (key == KeyCode.A)
        {
            return Vector3.left;
        }
        return Vector3.right;
    }
 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Pellet")
        {
            levelGrid.RemovePelletAt(other.transform.position);
            Destroy(other.gameObject);
            gameManager.PelletEaten();
        }
        else if (other.tag == "PowerPellets")
        {
            levelGrid.RemovePelletAt(other.transform.position);
            Destroy(other.gameObject);
            gameManager.PowerPelletEaten();
        }
        else if (other.tag == "Cherry")
        {
            Destroy(other.gameObject);
            gameManager.CherryEaten();
        }
        else if (other.tag == "Ghost")
        {
            gameManager.GhostCollision(other.GetComponent<GhostController>());
        }
    }
    public void SetCollisions(bool on)
    {
        pacCollider.enabled = on;
    }
    public void Die()
    {
        StopMoving();
        pacCollider.enabled = false;
        animator.speed = 1f;
        animator.Play("PacStudent_Death");
        deathEffect.Play();
    }
    public void Respawn()
    {
        row = 1;
        col = 1;
        transform.position = levelGrid.GridToWorld(row, col);
        lastInput = KeyCode.None;
        currentInput = KeyCode.None;
        animator.Play("PacStudent_WalkRight");
        animator.speed = 0f;
    }
    public void Freeze()
    {
        tweener.StopTween();
        StopMoving();
        pacCollider.enabled = false;
    }
}
