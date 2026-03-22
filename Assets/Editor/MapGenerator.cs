using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class MapGenerator : Editor
{
    [MenuItem("Tools/Generate Pixel Art Map")]
    public static void GenerateMap()
    {
        // Remove existing Grid if present
        GameObject existing = GameObject.Find("Grid");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }

        // ── Grid ──────────────────────────────────────────────
        GameObject gridGO = new GameObject("Grid");
        Grid grid = gridGO.AddComponent<Grid>();
        grid.cellSize = new Vector3(1f, 1f, 0f);

        // ── Tilemap layers ────────────────────────────────────
        Tilemap groundTilemap = CreateTilemapLayer(gridGO, "Ground",      0);
        Tilemap pathTilemap   = CreateTilemapLayer(gridGO, "Stone Path",  1);
        Tilemap wallTilemap   = CreateTilemapLayer(gridGO, "Wall",        2);

        // ── Load tiles ────────────────────────────────────────
        // Grass variants (0-3 for center fill variety)
        TileBase[] grassTiles = new TileBase[]
        {
            Load("TP Grass/TX Tileset Grass 0"),
            Load("TP Grass/TX Tileset Grass 1"),
            Load("TP Grass/TX Tileset Grass 2"),
            Load("TP Grass/TX Tileset Grass 3"),
        };

        // Flower decoration tiles
        TileBase[] flowerTiles = new TileBase[]
        {
            Load("TP Grass/TX Tileset Grass Flower 0"),
            Load("TP Grass/TX Tileset Grass Flower 1"),
            Load("TP Grass/TX Tileset Grass Flower 2"),
            Load("TP Grass/TX Tileset Grass Flower 3"),
        };

        // Stone ground (inner fill, edges)
        TileBase stoneCenter = Load("TP Stone Ground/TX Tileset Stone Ground_0");
        TileBase stoneEdgeH  = Load("TP Stone Ground/TX Tileset Stone Ground_1");
        TileBase stoneEdgeV  = Load("TP Stone Ground/TX Tileset Stone Ground_4");

        // Wall tiles
        TileBase wallTop        = Load("TP Wall/TX Tileset Wall_0");
        TileBase wallTopLeft    = Load("TP Wall/TX Tileset Wall_3");
        TileBase wallTopRight   = Load("TP Wall/TX Tileset Wall_4");
        TileBase wallLeft       = Load("TP Wall/TX Tileset Wall_8");
        TileBase wallRight      = Load("TP Wall/TX Tileset Wall_11");
        TileBase wallBottom     = Load("TP Wall/TX Tileset Wall_18");
        TileBase wallBotLeft    = Load("TP Wall/TX Tileset Wall_21");
        TileBase wallBotRight   = Load("TP Wall/TX Tileset Wall_22");

        // ── Map layout ────────────────────────────────────────
        // 4x larger than original 20x15 → 40x30
        const int W = 40;
        const int H = 30;

        System.Random rng = new System.Random(42);

        // 1. Fill ground with grass (slight variation)
        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                int variant = rng.Next(0, 10) < 2 ? rng.Next(1, 4) : 0; // 20% variant
                groundTilemap.SetTile(new Vector3Int(x, y, 0), grassTiles[variant]);
            }
        }

        // 2. Scatter flower decoration (inside walls, away from path)
        int pathMidY = H / 2;       // horizontal path row
        int pathMidX = W / 2;       // vertical path col
        for (int x = 2; x < W - 2; x++)
        {
            for (int y = 2; y < H - 2; y++)
            {
                bool onPath = (y == pathMidY || x == pathMidX);
                if (!onPath && rng.Next(0, 20) == 0)
                {
                    TileBase flower = flowerTiles[rng.Next(0, flowerTiles.Length)];
                    pathTilemap.SetTile(new Vector3Int(x, y, 0), flower);
                }
            }
        }

        // 3. Stone path – horizontal (row 7) and vertical (col 10)
        for (int x = 1; x < W - 1; x++)
        {
            pathTilemap.SetTile(new Vector3Int(x, pathMidY, 0), stoneCenter);
        }
        for (int y = 1; y < H - 1; y++)
        {
            pathTilemap.SetTile(new Vector3Int(pathMidX, y, 0), stoneCenter);
        }

        // 4. Wall border
        for (int x = 0; x < W; x++)
        {
            // Top row
            wallTilemap.SetTile(new Vector3Int(x, H - 1, 0), wallTop);
            // Bottom row
            wallTilemap.SetTile(new Vector3Int(x, 0, 0), wallBottom);
        }
        for (int y = 0; y < H; y++)
        {
            // Left column
            wallTilemap.SetTile(new Vector3Int(0, y, 0), wallLeft);
            // Right column
            wallTilemap.SetTile(new Vector3Int(W - 1, y, 0), wallRight);
        }
        // Corners
        wallTilemap.SetTile(new Vector3Int(0,     H - 1, 0), wallTopLeft);
        wallTilemap.SetTile(new Vector3Int(W - 1, H - 1, 0), wallTopRight);
        wallTilemap.SetTile(new Vector3Int(0,     0,     0), wallBotLeft);
        wallTilemap.SetTile(new Vector3Int(W - 1, 0,     0), wallBotRight);

        // ── Reposition camera to center of map ───────────────
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(W / 2f, H / 2f, -10f);
            cam.orthographic = true;
            cam.orthographicSize = H / 2f;
        }

        // Save scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[MapGenerator] Map generated successfully! (40x30 tiles)");
    }

    // ── Helpers ───────────────────────────────────────────────
    static Tilemap CreateTilemapLayer(GameObject parent, string name, int sortOrder)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Tilemap tm   = go.AddComponent<Tilemap>();
        TilemapRenderer tr = go.AddComponent<TilemapRenderer>();
        tr.sortingOrder = sortOrder;
        return tm;
    }

    static TileBase Load(string relativePath)
    {
        string basePath = "Assets/Cainos/Pixel Art Top Down - Basic/Tile Palette/";
        string fullPath = basePath + relativePath + ".asset";
        TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(fullPath);
        if (tile == null)
            Debug.LogWarning($"[MapGenerator] Tile not found: {fullPath}");
        return tile;
    }
}
