using UnityEngine;

/// <summary>
/// 4×3 유닛 배치 그리드 데이터 관리.
/// 슬롯 좌표계: col 0~3 (좌→우), row 0~2 (하→상)
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int columns = 4;
    public int rows = 3;
    public float cellSize = 1.1f;

    // slots[col, row] = 배치된 UnitData (null이면 빈 칸)
    private UnitData[,] slots;

    void Awake()
    {
        Instance = this;
        slots = new UnitData[columns, rows];
    }

    /// <summary>슬롯의 월드 좌표 반환</summary>
    public Vector3 GetSlotWorldPos(int col, int row)
    {
        float startX = transform.position.x - (columns - 1) * cellSize * 0.5f;
        float startY = transform.position.y - (rows - 1) * cellSize * 0.5f;
        return new Vector3(startX + col * cellSize, startY + row * cellSize, 0f);
    }

    public bool IsEmpty(int col, int row) => slots[col, row] == null;

    /// <summary>유닛 배치. 성공 여부 반환.</summary>
    public bool PlaceUnit(int col, int row, UnitData unit)
    {
        if (col < 0 || col >= columns || row < 0 || row >= rows) return false;
        if (!IsEmpty(col, row)) return false;
        slots[col, row] = unit;
        return true;
    }

    public UnitData GetUnit(int col, int row) => slots[col, row];

    public void RemoveUnit(int col, int row)
    {
        if (col < 0 || col >= columns || row < 0 || row >= rows) return;
        slots[col, row] = null;
    }

    /// <summary>월드 좌표 → (col, row) 변환. 범위 밖이면 false.</summary>
    public bool WorldToSlot(Vector3 worldPos, out int col, out int row)
    {
        float startX = transform.position.x - (columns - 1) * cellSize * 0.5f;
        float startY = transform.position.y - (rows - 1) * cellSize * 0.5f;

        col = Mathf.RoundToInt((worldPos.x - startX) / cellSize);
        row = Mathf.RoundToInt((worldPos.y - startY) / cellSize);

        return col >= 0 && col < columns && row >= 0 && row < rows;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                bool occupied = slots != null && slots[c, r] != null;
                Gizmos.color = occupied
                    ? new Color(1f, 0.5f, 0f, 0.5f)
                    : new Color(0f, 1f, 0.4f, 0.35f);
                Gizmos.DrawWireCube(GetSlotWorldPos(c, r), Vector3.one * cellSize * 0.92f);
            }
        }
    }
#endif
}
