using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    private int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    
    private int[,] map;
 
    void Awake()
    {
        int quarterRows = levelMap.GetLength(0); 
        int quarterCols = levelMap.GetLength(1);   
        int rows = quarterRows * 2 - 1;          
        int cols = quarterCols * 2;            
 
        map = new int[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int quarterRow = r;
                if (r >= quarterRows)
                {
                    quarterRow = rows - 1 - r;    
                }
                int quarterCol = c;
                if (c >= quarterCols)
                {
                    quarterCol = cols - 1 - c;     
                }
                map[r, c] = levelMap[quarterRow, quarterCol];
            }
        }
    }

    public int GetTile(int row, int col)
    {
        if (row < 0 || row >= map.GetLength(0) || col < 0 || col >= map.GetLength(1))
        {
            return -1;
        }
        return map[row, col];
    }
    
    public bool IsWalkable(int row, int col)
    {
        int tile = GetTile(row, col);
        return tile == 0 || tile == 5 || tile == 6;
    }
 
    public bool HasPellet(int row, int col)
    {
        int tile = GetTile(row, col);
        return tile == 5 || tile == 6;
    }

    public Vector3 GridToWorld(int row, int col)
    {
        return new Vector3(col, -row, 0f);
    }
}
