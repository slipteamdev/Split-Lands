using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public enum Team { Team1, Team2, None }
    

    private static Map Instance;

    [SerializeField] private Image mask;
    [SerializeField] private Sprite startMask;
    [SerializeField] private Sprite startMaskEdge;
    [Space]
    [SerializeField] private MeshFilter edgesMeshFilter;
    [SerializeField] private MeshCollider edgesMeshCollider;

    private Texture2D terrainMask;
    private byte[,] terrainData;

    public static int[] pixelCount;
    public static float normalicedProgress => (float)pixelCount[0] / (pixelCount[0] + pixelCount[1]); // Devuelve el porcentaje de terreno controlado por el equipo 1, normalizado

    public int MapWidth => terrainMask.width;
    public int MapHeight => terrainMask.height;

    private const float TEAM1_MASK_VALUE = 0f;
    private const float TEAM2_MASK_VALUE = 1f;


    private List<Vector3> edgesVertices = new List<Vector3>(32000);
    private List<int> edgesTriangles = new List<int>(32000);
    private Mesh edgesMesh;
    private void Awake()
    {
        Instance = this;
        pixelCount = new int[2] { 0, 0 };

        InitMask();
    }

    private void InitMask()
    {
        terrainMask = Instantiate(startMask.texture);
        terrainData = new byte[terrainMask.width, terrainMask.height];
        edgesMesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        mask.sprite = Sprite.Create(terrainMask, startMask.rect, new Vector2(0.5f, 0.5f), 100);

        Color[] edge_pixels = startMaskEdge.texture.GetPixels();
        Color[] mask_pixels = terrainMask.GetPixels();
        int width = terrainMask.width;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < terrainMask.height; j++)
            {
                int index = i + j * width;
                Color color = edge_pixels[index];

                if (color.r < 0.5f && color.b > 0.5f)
                {
                    mask_pixels[index] = new Color(1f, 1f, 1f, TEAM1_MASK_VALUE);
                    terrainData[i, j] = 1;
                    pixelCount[0]++;
                }
                else if (color.r > 0.5f && color.b < 0.5f)
                {
                    mask_pixels[index] = new Color(1f, 1f, 1f, TEAM2_MASK_VALUE);
                    terrainData[i, j] = 2;
                    pixelCount[1]++;
                }
                else
                {
                    mask_pixels[index] = new Color(1f, 1f, 1f, TEAM2_MASK_VALUE);
                    terrainData[i, j] = 0;
                }
            }
        }

        terrainMask.SetPixels(mask_pixels);
        terrainMask.Apply();

        UpdateEdgesMesh();
    }

    public static void Draw(Vector2 position, Sprite destruction, Team team) => Draw(((int)position.x), ((int)position.y), destruction, team, Vector2.right);
    public static void Draw(Vector2 position, Sprite destruction, Team team, Vector2 direction) => Draw(((int)position.x), ((int)position.y), destruction, team, direction);
    public static void Draw(int x, int y, Sprite destruction, Team team) => Draw(x, y, destruction, team, Vector2.right);

    public static void Draw(int x, int y, Sprite destruction, Team team, Vector2 direction)
    {
        int width = destruction.texture.width;
        int height = destruction.texture.height;
        int half_width = width / 2;
        int half_height = height / 2;

        direction = direction.normalized;

        float viewport_x = ((float)x / Screen.width) + 0.5f;
        float viewport_y = ((float)y / Screen.height) + 0.5f;

        x = Mathf.FloorToInt(viewport_x * Instance.MapWidth);
        y = Mathf.FloorToInt(viewport_y * Instance.MapHeight);

        Color[] brush_pixels = destruction.texture.GetPixels();
        bool has_updated = false;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x_local = i - half_width;
                float y_local = j - half_height;

                int x_pos = x + Mathf.FloorToInt(x_local * direction.x - y_local * direction.y);
                int y_pos = y + Mathf.FloorToInt(x_local * direction.y + y_local * direction.x);

                if (x_pos < 0 || x_pos >= Instance.MapWidth || y_pos < 0 || y_pos >= Instance.MapHeight) continue;

                Color color = brush_pixels[i + j * width];
                if (color.a < 0.5f) continue;

                byte team_index = (byte)(team + 1);

                if (Instance.terrainData[x_pos, y_pos] != team_index)
                {
                    has_updated = true;

                    if (color == Color.white)
                    {
                        if (Instance.terrainData[x_pos, y_pos] != 0)
                            pixelCount[Instance.terrainData[x_pos, y_pos] - 1] -= 1;

                        Instance.terrainData[x_pos, y_pos] = 0;

                        Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1, 1, 1, TEAM2_MASK_VALUE));
                    }
                    else
                    {
                        if (Instance.terrainData[x_pos, y_pos] != 0)
                            pixelCount[Instance.terrainData[x_pos, y_pos] - 1] -= 1;

                        pixelCount[team_index - 1]++;
                        Instance.terrainData[x_pos, y_pos] = team_index;

                        Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1, 1, 1, team == Team.Team1 ? TEAM1_MASK_VALUE : TEAM2_MASK_VALUE));
                    }
                }
            }
        }

        if (!has_updated) return;

        FixNonsenseEdges(x - half_width, y - half_height, destruction, team);

        Instance.terrainMask.Apply();
        Instance.UpdateEdgesMesh();
    }

    private static void FixNonsenseEdges(int x, int y, Sprite destruction, Team team)
    {
        int width = destruction.texture.width;
        int height = destruction.texture.height;

        for (int a = 0; a < width; a++)
        {
            for (int b = 0; b < height; b++)
            {
                int x_pos = x + a;
                int y_pos = y + b;

                if (x_pos < 0 || x_pos >= Instance.MapWidth || y_pos < 0 || y_pos >= Instance.MapHeight)
                    continue;

                byte current_team = Instance.terrainData[x_pos, y_pos];
                if (current_team == 0) continue;

                bool has_same_team_neighbor = false;
                bool has_enemy_team_neighbor = false;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        int neighbor_x = x_pos + i;
                        int neighbor_y = y_pos + j;

                        if (neighbor_x >= 0 && neighbor_x < Instance.MapWidth && neighbor_y >= 0 && neighbor_y < Instance.MapHeight)
                        {
                            byte neighbor_team = Instance.terrainData[neighbor_x, neighbor_y];

                            if (neighbor_team == current_team) has_same_team_neighbor = true;
                            else if (neighbor_team != 0) has_enemy_team_neighbor = true;
                        }
                    }
                }

                if (!has_same_team_neighbor)
                {
                    pixelCount[current_team - 1]--;

                    if (has_enemy_team_neighbor)
                    {
                        byte other_team = current_team == 1 ? (byte)2 : (byte)1;
                        Instance.terrainData[x_pos, y_pos] = other_team;
                        pixelCount[other_team - 1]++;

                        float mask_value = other_team == 1 ? TEAM1_MASK_VALUE : TEAM2_MASK_VALUE;
                        Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1f, 1f, 1f, mask_value));
                    }
                    else
                    {
                        Instance.terrainData[x_pos, y_pos] = 0;
                        Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1f, 1f, 1f, TEAM2_MASK_VALUE));
                    }
                }
            }
        }
    }

    /*
    private static readonly Vector3[][] VertsTable = new Vector3[][]
    {
        null, // 0
        new[] { new Vector3(0, 1f), new Vector3(0, 0.5f), new Vector3(0.5f, 1) }, // 1
        new[] { new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 1) }, // 2
        new[] { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f) }, // 3
        new[] { new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) }, // 4
        new[] { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) }, // 5
        new[] { new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(1, 1), new Vector3(1, 0) }, // 6
        new[] { new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(0, 0.5f) }, // 7
        new[] { new Vector3(0, 0.5f), new Vector3(0, 0), new Vector3(0.5f, 0) }, // 8
        new[] { new Vector3(0, 0), new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(0, 1) }, // 9
        new[] { new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 0), new Vector3(1, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f) }, // 10
        new[] { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 0) }, // 11
        new[] { new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 0.5f), new Vector3(0, 0.5f) }, // 12
        new[] { new Vector3(0, 0), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f), new Vector3(1, 0) }, // 13
        new[] { new Vector3(1, 1), new Vector3(1, 0), new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 1) } // 14
    };
    private static readonly int[][] TriangsTable = new int[][]
    {
        null, // 0
        new[] { 2, 1, 0 }, // 1
        new[] { 0, 1, 2 }, // 2
        new[] { 0, 1, 2, 0, 2, 3 }, // 3
        new[] { 0, 1, 2 }, // 4
        new[] { 0, 1, 2, 3, 4, 5, 4, 0, 5, 0, 2, 5 }, // 5
        new[] { 0, 1, 2, 0, 2, 3 }, // 6
        new[] { 2, 3, 1, 3, 4, 1, 4, 0, 1 }, // 7
        new[] { 2, 1, 0 }, // 8
        new[] { 1, 0, 2, 0, 3, 2 }, // 9
        new[] { 0, 1, 2, 5, 4, 3, 1, 4, 2, 4, 5, 1 }, // 10
        new[] { 0, 1, 2, 0, 2, 3, 4, 0, 3 }, // 11
        new[] { 0, 3, 2, 0, 2, 1 }, // 12
        new[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 }, // 13
        new[] { 0, 1, 4, 1, 3, 4, 1, 2, 3 } // 14
    };
    */
    private readonly Vector3[] PIXEL_VERTS = new Vector3[4]
    {
        new Vector3(0, 0),
        new Vector3(1, 0),
        new Vector3(0, 1),
        new Vector3(1, 1)
    };
    private readonly int[] PIXEL_TRIANGS = new int[6] { 2, 1, 0, 2, 3, 1 };
    private void UpdateEdgesMesh()
    {
        edgesVertices.Clear();
        edgesTriangles.Clear();

        int width = terrainMask.width;
        int height = terrainMask.height;

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                int a = terrainData[i, j] == 1 ? 1 : 0;
                int b = terrainData[i + 1, j] == 1 ? 1 : 0;
                int c = terrainData[i + 1, j + 1] == 1 ? 1 : 0;
                int d = terrainData[i, j + 1] == 1 ? 1 : 0;

                int result = a + b + c + d;

                if (result == 0 || result == 4) continue;

                int vertices_count = edgesVertices.Count;
                Vector3 offset = new Vector3(i + 0.5f, j + 0.5f, 0f);

                for (int k = 0; k < PIXEL_VERTS.Length; k++)
                {
                    edgesVertices.Add(PIXEL_VERTS[k] + offset);
                }

                for (int k = 0; k < PIXEL_TRIANGS.Length; k++)
                {
                    edgesTriangles.Add(vertices_count + PIXEL_TRIANGS[k]);
                }
            }
        }

        edgesMesh.Clear();
        edgesMesh.SetVertices(edgesVertices);
        edgesMesh.SetTriangles(edgesTriangles, 0);

        edgesMesh.RecalculateNormals();
        edgesMesh.RecalculateTangents();

        edgesMeshFilter.mesh = edgesMesh;
        edgesMeshCollider.sharedMesh = edgesMesh;
    }

    public static Team ReadPixelAt(float x, float y)
    {
        float viewport_x = (x / Screen.width) + 0.5f;
        float viewport_y = (y / Screen.height) + 0.5f;

        x = Mathf.FloorToInt(viewport_x * Instance.MapWidth);
        y = Mathf.FloorToInt(viewport_y * Instance.MapHeight);

        return Instance.terrainData[(int)x, (int)y] == 1 ? Team.Team1 : Instance.terrainData[(int)x, (int)y] == 2 ? Team.Team2 : Team.None;
    }
}