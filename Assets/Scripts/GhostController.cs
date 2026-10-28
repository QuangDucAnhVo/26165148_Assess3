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
    
    private int[] cornerRows = { 1, 1, 5, 5, 1, 1, 8, 8, 20, 20, 27, 27, 23, 23, 27, 27, 20, 20, 8, 8 };
    private int[] cornerCols = { 1, 12, 12, 15, 15, 26, 26, 21, 21, 26, 26, 15, 15, 12, 12, 1, 1, 6, 6, 1 };
    private int cornerIndex = 12;
    private int stepsToCorner = 0;

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
        int TowardsPacStudent()
        {
            List<int> valid = ValidDirections();
            float currentDistance = DistanceToPacStudent(row, col);
 
            List<int> closer = new List<int>();
            for (int i = 0; i < valid.Count; i++)
            {
                int dir = valid[i];
                if (DistanceToPacStudent(row + rowStep[dir], col + colStep[dir]) <= currentDistance)
                {
                    closer.Add(dir);
                }
            }
            if (closer.Count > 0)
            {
                return RandomFrom(closer);
            }
            return RandomFrom(valid);
        }

        int RandomDirection()
        {
            return RandomFrom(ValidDirections());
        }
        int ClockwiseAroundMap()
        {
            if ((row == cornerRows[cornerIndex] && col == cornerCols[cornerIndex]) || stepsToCorner > 20)
            {
                cornerIndex = (cornerIndex + 1) % cornerRows.Length;
                stepsToCorner = 0;
            }
            stepsToCorner++;

            Vector3 corner = levelGrid.GridToWorld(cornerRows[cornerIndex], cornerCols[cornerIndex]);
            List<int> valid = ValidDirections();
            int bestDir = valid[0];
            float bestDistance = 1000f;
            for (int i = 0; i < valid.Count; i++)
            {
                int dir = valid[i];
                float distance = (levelGrid.GridToWorld(row + rowStep[dir], col + colStep[dir]) - corner).magnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDir = dir;
                }
            }
            return bestDir;
        }

        float DistanceToPacStudent(int r, int c)
        {
            return (levelGrid.GridToWorld(r, c) - pacStudent.transform.position).magnitude;
        }

        int RandomFrom(List<int> options)
        {
            return options[Random.Range(0, options.Count)];
        }
        void PlayAnimation()
        {
            string facing = "Down";
            if (direction != -1)
            {
                facing = directions[direction];
            }

            if (state == GhostState.Normal)
            {
                animator.Play("Normal_" + facing);
            }
            else if (state == GhostState.Scared)
            {
                animator.Play("Scared_" + facing);
            }
            else if (state == GhostState.Recovering)
            {
                animator.Play("Recovering");
            }
            else animator.Play("Ghost_Dead");
            }

        public void SetState(GhostState newState)
        {
            state = newState;
            PlayAnimation();
        }

        public void Freeze()
        {
            tweener.StopTween();
        }

        public void GhostReset()
        {
            tweener.StopTween();
            transform.position = startPosition;
            row = startRow;
            col = startCol;
            direction = -1;
            cornerIndex = 12;
            SetState(GhostState.Normal);
        }
} 
