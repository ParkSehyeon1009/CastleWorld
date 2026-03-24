using UnityEngine;

/// <summary>
/// 유닛 배치 그리드 데이터 관리 + 런타임 셀 비주얼 생성.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int columns = 4;
    public int rows = 2;
    public float cellSize = 1.1f;

    [Header("셀 비주얼")]
    [Tooltip("셀 배경에 사용할 스프라이트 (null이면 흰 사각형 자동 생성)")]
    public Sprite cellSprite;
    [Tooltip("빈 슬롯 색상")]
    public Color emptyColor  = new Color(0.2f, 0.8f, 0.4f, 0.35f);
    [Tooltip("점유된 슬롯 색상")]
    public Color filledColor = new Color(1f, 0.55f, 0.1f, 0.5f);
    [Tooltip("셀 비주얼의 정렬 순서 (유닛보다 뒤에 오도록 낮게 설정)")]
    public int sortingOrder = -1;

    // slots[col, row] = 배치된 UnitData (null이면 빈 칸)
    private UnitData[,] slots;
    private SpriteRenderer[,] cellRenderers;

    void Awake()
    {
        Instance = this;
        slots = new UnitData[columns, rows];
        GenerateCellVisuals();
    }

    // ─────────────────────────────────────────────
    // 비주얼 생성
    // ─────────────────────────────────────────────

    void GenerateCellVisuals()
    {
        cellRenderers = new SpriteRenderer[columns, rows];

        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                GameObject cell = new GameObject($"Cell_{c}_{r}");
                cell.transform.SetParent(transform, false);
                cell.transform.position = GetSlotWorldPos(c, r);

                SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();
                sr.sprite = cellSprite != null ? cellSprite : CreateWhiteSquareSprite();
                sr.color  = emptyColor;
                sr.sortingOrder = sortingOrder;
                // 셀 크기를 cellSize에 맞게 스케일
                float spriteSize = sr.sprite.bounds.size.x;
                float scale = (cellSize * 0.92f) / spriteSize;
                cell.transform.localScale = new Vector3(scale, scale, 1f);

                cellRenderers[c, r] = sr;
            }
        }
    }

    static Sprite CreateWhiteSquareSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    // ─────────────────────────────────────────────
    // 슬롯 데이터 API
    // ─────────────────────────────────────────────

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
        UpdateCellColor(col, row);
        return true;
    }

    public UnitData GetUnit(int col, int row) => slots[col, row];

    public void RemoveUnit(int col, int row)
    {
        if (col < 0 || col >= columns || row < 0 || row >= rows) return;
        slots[col, row] = null;
        UpdateCellColor(col, row);
    }

    void UpdateCellColor(int col, int row)
    {
        if (cellRenderers == null) return;
        if (col < 0 || col >= columns || row < 0 || row >= rows) return;
        var sr = cellRenderers[col, row];
        if (sr == null) return;   // 씬 종료 시 자식이 먼저 파괴될 수 있음
        sr.color = slots[col, row] != null ? filledColor : emptyColor;
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
                    ? new Color(1f, 0.5f, 0f, 0.6f)
                    : new Color(0f, 1f, 0.4f, 0.4f);
                Gizmos.DrawWireCube(GetSlotWorldPos(c, r), Vector3.one * cellSize * 0.92f);
            }
        }
    }
#endif
}
