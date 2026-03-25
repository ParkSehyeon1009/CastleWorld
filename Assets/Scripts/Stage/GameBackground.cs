using UnityEngine;

/// <summary>
/// GameScene 배경을 구역별로 생성합니다.
/// - 상단: 적 이동 구역 (흙/전장)
/// - 중간: 방어선 (벽/경계)
/// - 하단: 플레이어 기지 (석재)
/// </summary>
[ExecuteAlways]
public class GameBackground : MonoBehaviour
{
    void Awake() => Build();

    void OnValidate()
    {
        if (!Application.isPlaying) Build();
    }

    void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        CreateZone("BG_Base",       new Color(0.12f, 0.14f, 0.18f),  0f,     0f,     5.2f, 10.5f, -30);
        CreateZone("BG_EnemyField", new Color(0.38f, 0.28f, 0.18f),  0f,  2.75f,    5.2f,  5.5f, -29);
        CreateZone("BG_BaseFloor",  new Color(0.22f, 0.22f, 0.28f),  0f, -2.75f,    5.2f,  5.5f, -29);
        CreateZone("BG_GridFloor",  new Color(0.26f, 0.26f, 0.34f),  0.02f, -1.72f, 5.0f,  2.7f, -28);
        CreateZone("BG_Wall",       new Color(0.18f, 0.16f, 0.14f),  0f,   0.08f,   5.2f,  0.55f, -28);
        CreateZone("BG_WallTop",    new Color(0.32f, 0.29f, 0.24f),  0f,   0.38f,   5.2f,  0.10f, -27);

        for (int i = 0; i < 4; i++)
        {
            float x = -1.8f + i * 1.2f;
            CreateZone($"BG_Pillar_{i}", new Color(0.14f, 0.13f, 0.11f), x, 0.08f, 0.08f, 0.55f, -26);
        }
    }

    void CreateZone(string zoneName, Color color, float x, float y, float width, float height, int sortOrder)
    {
        GameObject go = new GameObject(zoneName);
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(x, y, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSprite();
        sr.color  = color;
        sr.sortingOrder = sortOrder;

        // 스프라이트 1픽셀 = 4유닛(ppu=4) → sprite.bounds.size.x = 1
        go.transform.localScale = new Vector3(width, height, 1f);
    }

    static Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }
}
