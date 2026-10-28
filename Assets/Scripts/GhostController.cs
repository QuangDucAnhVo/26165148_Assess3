using UnityEngine;
using System.Collections.Generic;

public class GhostController : MonoBehaviour
{
    public enum GhostState
    {
        Normal,
        Scared,
        Recovering,
        Dead
    }

    public GhostState state = GhostState.Normal;
    public int ghostNumber = 1;
    public LevelGrid levelGrid;
    public PacStudentController pacStudent;
    public GameManager gameManager;
    private Animator animator;
    private Tweener tweener;
    private Vector3 startPosition;

    private int startRow;
    private int startCol;
    private int row;
    private int col;
    private int direction = -1;

    private int[] rowStep = { -1, 0, 1, 0 };
    private int[] colStep = { 0, 1, 0, -1 };
    private int[] oppositeStep = { 2, 3, 0, 1 };
    private string[] directions = { "Up", "Right", "Down", "Left" };

    void Awake()
    {
        animator = GetComponent<Animator>();
        tweener = GetComponent<Tweener>();
        startPosition = transform.position;
        startRow = (int)(-startPosition.y + 0.5f);
        startCol = (int)(startPosition.x + 0.5f);
        row = startRow;
        col = startCol;
    }

    void Start()
    {
        SetState(GhostState.Normal);
    }

    void Update()
    {
        if (GameManager.gameState != GameManager.GameState.Playing)
        {
            return;
        }

        if (!tweener.IsTweening())
        {
            Decide();
        }
    }

    void Decide()
    {
        if (state == GhostState.Dead)
        {
            if (row != startRow || col != startCol)
            {
                BackToSpawn();
                return;
            }

            gameManager.GhostRevived(this);
        }

        int dir;
        if (levelGrid.IsInSpawnArea(row, col))
        {
            dir = ExitDirection();
        }
        else if (state == GhostState.Normal)
        {
            if (ghostNumber == 1)
            {
                dir = AwayFromPacStudent();
            }
            else if (ghostNumber == 2)
            {
                dir = TowardsPacStudent();
            }
            else if (ghostNumber == 3)
            {
                dir = RandomDirection();
            }
            else dir = ClockwiseAroundMap();
        }
        else
            {
                dir = AwayFromPacStudent();
            }
            Movement(dir);
        }

        void Movement(int dir)
        {
            int nextRow = row + rowStep[dir];
            int nextCol = col + colStep[dir];
            Vector3 startPos = levelGrid.GridToWorld(row, col);
            Vector3 endPos = levelGrid.GridToWorld(nextRow, nextCol);
            tweener.AddTween(transform, startPos, endPos, 1f / CurrentSpeed());

            row = nextRow;
            col = nextCol;
            direction = dir;
            PlayAnimation();
        }

        void BackToSpawn()
        {
            float distance = (startPosition - transform.position).magnitude;
            tweener.AddTween(transform, transform.position, startPosition, distance / SlowSpeed());

            row = startRow;
            col = startCol;
            direction = -1;
        }

        float CurrentSpeed()
        {
            if (state == GhostState.Normal)
            {
                return pacStudent.speed * 0.9f;
            }

            return SlowSpeed();
        }

        float SlowSpeed()
        {
            return pacStudent.speed * 0.9f * 0.5f;
        }

        int ExitDirection()
        {
            int gapCol = 13;
            if (col >= 14)
            {
                gapCol = 14;
            }

            if (col < gapCol)
            {
                return 1;
            }

            if (col > gapCol)
            {
                return 3;
            }

            if (ghostNumber == 1 || ghostNumber == 3)
            {
                return 0;
            }

            return 2;
        }

        List<int> ValidDirections()
        {
            List<int> valid = new List<int>();
            for (int dir = 0; dir < 4; dir++)
            {
                bool isBackstep = direction != -1 && dir == oppositeStep[direction];
                if (!isBackstep && levelGrid.IsWalkable(row + rowStep[dir], col + colStep[dir]))
                {
                    valid.Add(dir);
                }
            }

            if (valid.Count == 0 && direction != -1)
            {
                valid.Add(oppositeStep[direction]);
            }

            return valid;
        }

        int AwayFromPacStudent()
        {
            List<int> valid = ValidDirections();
            float currentDistance = DistanceToPacStudent(row, col);

            List<int> away = new List<int>();
            for (int i = 0; i < valid.Count; i++)
            {
                int dir = valid[i];
                if (DistanceToPacStudent(row + rowStep[dir], col + colStep[dir]) >= currentDistance)
                {
                    away.Add(dir);
                }
            }

            if (away.Count > 0)
            {
                return RandomFrom(away);
            }

            return RandomFrom(valid);
        }

        float DistanceToPacStudent(int r, int c)
        {
            return (levelGrid.GridToWorld(r, c) - pacStudent.transform.position).magnitude;
        }

        int RandomFrom(List<int> options)
        {
            return options[Random.Range(0, options.Count)];
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

        public void Freeze()
        {
            tweener.StopTween();
        }

        public void GhostReset()
        {
            transform.position = startPosition;
            SetState(GhostState.Normal);
        }
    }
}
