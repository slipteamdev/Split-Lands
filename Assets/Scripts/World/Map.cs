using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    //  TEMP
    public enum Team { Team1, Team2 }
    //

    private static Map Instance;

    [SerializeField] private Image mask;
    [SerializeField] private Sprite startMask;
    [SerializeField] private Sprite startMaskEdge;
    [Space]
    [SerializeField] private MeshFilter edgesMeshFilter;
    [SerializeField] private MeshCollider edgesMeshCollider;

    private Texture2D terrainMask;
    private byte[,] terrainData;

    public int MapWidth => terrainMask.width;
    public int MapHeight => terrainMask.height;

    private const float TEAM1_MASK_VALUE = 0f;
    private const float TEAM2_MASK_VALUE = 1f;


    private void Awake()
    {
        Instance = this;

        InitMask();
    }

    private void InitMask()
    {
        terrainMask = Instantiate(startMask.texture);

        terrainData = new byte[terrainMask.width, terrainMask.height];
        edgesMesh = new Mesh();

        mask.sprite = Sprite.Create(terrainMask, startMask.rect, new Vector2(0.5f, 0.5f), 100);

        for (int i = 0; i < terrainMask.width; i++)
            for (int j = 0; j < terrainMask.height; j++)
            {
                Color color = startMaskEdge.texture.GetPixel(i, j);

                if (color.r < 0.5f && color.b > 0.5f)
                {
                    terrainMask.SetPixel(i, j, new Color(1f, 1f, 1f, TEAM1_MASK_VALUE));
                    terrainData[i, j] = 1;
                }
                else if(color.r > 0.5f && color.b < 0.5f)
                {
                    terrainMask.SetPixel(i, j, new Color(1f, 1f, 1f, TEAM2_MASK_VALUE));
                    terrainData[i, j] = 2;
                }
                else
                {
                    terrainMask.SetPixel(i, j, new Color(1f, 1f, 1f, TEAM2_MASK_VALUE));
                    terrainData[i, j] = 0;
                }
            }

        terrainMask.Apply();

        UpdateEdgesMesh();
    }

    public static void Draw(Vector2 position, Sprite destruction, Team team) => Draw(((int)position.x), ((int)position.y), destruction, team);
    public static void Draw(int x, int y, Sprite destruction, Team team)
    {
        int width = destruction.texture.width;
        int height = destruction.texture.height;
        int half_width = width / 2;
        int half_height = height / 2;

        float viewport_x = ((float)x / Screen.width) + 0.5f;
        float viewport_y = ((float)y / Screen.height) + 0.5f;

        x = Mathf.FloorToInt(viewport_x * Instance.MapWidth);
        y = Mathf.FloorToInt(viewport_y * Instance.MapHeight);

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                int x_pos = x + i - half_width;
                int y_pos = y + j - half_height;

                if (x_pos < 0 || x_pos >= Instance.MapWidth
                    || y_pos < 0 || y_pos >= Instance.MapHeight) continue;

                Color color = destruction.texture.GetPixel(i, j);

                if (color.a < 0.5f) continue;

                byte team_index = (byte)(team + 1);

                if (Instance.terrainData[x_pos, y_pos] != team_index) Instance.terrainData[x_pos, y_pos] = color == Color.white ? (byte)0 : team_index;

                Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1, 1, 1, team == Team.Team1 ? TEAM1_MASK_VALUE : TEAM2_MASK_VALUE));
            }

        //  FixNonsenseEdges(x - half_width, y - half_height, destruction, team);

        Instance.terrainMask.Apply();

        Instance.UpdateEdgesMesh();
    }
    private static void FixNonsenseEdges(int x, int y, Sprite destruction, Team team)
    {
        int width = destruction.texture.width;
        int height = destruction.texture.height;

        byte target = team == Team.Team1 ? (byte)2 : (byte)1;

        for (int a = 0; a < width; a++)
            for (int b = 0; b < height; b++)
            {
                Color color = destruction.texture.GetPixel(a, b);

                if (color != Color.white) continue;

                int x_pos = x + a;
                int y_pos = y + b;

                if (x_pos < 0 || x_pos >= Instance.MapWidth
                    || y_pos < 0 || y_pos >= Instance.MapHeight) continue;

                bool found_target = false;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        int x_pixel_pos = x_pos + i;
                        int y_pixel_pos = y_pos + i;

                        if (Instance.terrainData[x_pixel_pos, y_pixel_pos] == target)
                        {
                            found_target = true;
                            break;
                        }
                    }

                    if (found_target) break;
                }

                if (!found_target)
                {
                    Instance.terrainData[x_pos, y_pos] = (byte)(team + 1);
                    Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1, 1, 1, team == Team.Team1 ? TEAM1_MASK_VALUE : TEAM2_MASK_VALUE));
                }
            }
    }

    private Mesh edgesMesh;
    private void UpdateEdgesMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 1; i < terrainMask.width - 1; i++)
            for (int j = 1; j < terrainMask.height - 1; j++)
            {
                int a = Mathf.Max(terrainData[i, j] == 0 ? 1 : 0);
                int b = terrainData[i + 1, j] == 0 ? 1 : 0;
                int c = terrainData[i + 1, j + 1] == 0 ? 1 : 0;
                int d = terrainData[i, j + 1] == 0 ? 1 : 0;

                int value = a * 8 + b * 4 + c * 2 + d;
                Vector3[] verts;
                int[] triangs;

                switch (value)
                {
                    case 0:
                    default:
                        continue;

                    case 1:
                        verts = new Vector3[]
                        { new Vector3(0, 1f), new Vector3(0, 0.5f), new Vector3(0.5f, 1) };

                        triangs = new int[]
                        { 2, 1, 0};
                        break;

                    case 2:
                        verts = new Vector3[]
                        { new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 1) };

                        triangs = new int[]
                        { 0, 1, 2};
                        break;

                    case 3:
                        verts = new Vector3[]
                        { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f) };

                        triangs = new int[]
                        { 0, 1, 2, 0, 2, 3};
                        break;

                    case 4:
                        verts = new Vector3[]
                        { new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };

                        triangs = new int[]
                        { 0, 1, 2};
                        break;

                    case 5:
                        verts = new Vector3[]
                        { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };

                        triangs = new int[]
                        { 0, 1, 2, 3, 4, 5, 4, 0, 5, 0, 2, 5};
                        break;

                    case 6:
                        verts = new Vector3[]
                        { new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(1, 1), new Vector3(1, 0) };

                        triangs = new int[]
                        { 0, 1, 2, 0, 2, 3};
                        break;

                    case 7:
                        verts = new Vector3[]
                        { new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(0, 0.5f) };

                        triangs = new int[]
                        { 2, 3, 1, 3, 4, 1, 4, 0, 1};
                        break;

                    case 8:
                        verts = new Vector3[]
                        { new Vector3(0, 0.5f), new Vector3(0, 0), new Vector3(0.5f, 0) };

                        triangs = new int[]
                        { 2, 1, 0};
                        break;

                    case 9:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(0, 1) };

                        triangs = new int[]
                        { 1, 0, 2, 0, 3, 2};
                        break;

                    case 10:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 0), new Vector3(1, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f) };

                        triangs = new int[]
                        { 0, 1, 2, 5, 4, 3, 1, 4, 2, 4, 5, 1 };
                        break;

                    case 11:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 0) };

                        triangs = new int[]
                        { 0, 1, 2, 0, 2, 3, 4, 0, 3};
                        break;

                    case 12:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 0.5f), new Vector3(0, 0.5f) };

                        triangs = new int[]
                        { 0, 3, 2, 0, 2, 1};
                        break;

                    case 13:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(0, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f), new Vector3(1, 0) };

                        triangs = new int[]
                        { 0, 1, 2, 0, 2, 3, 0, 3, 4};
                        break;

                    case 14:
                        verts = new Vector3[]
                        { new Vector3(1, 1), new Vector3(1, 0), new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 1) };

                        triangs = new int[]
                        { 0, 1, 4, 1, 3, 4, 1, 2, 3};
                        break;

                    case 15:
                        verts = new Vector3[]
                        { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0) };

                        triangs = new int[]
                        { 0, 1, 2, 0, 2, 3};
                        break;
                }

                foreach (int t in triangs) triangles.Add(vertices.Count + t);
                foreach (Vector3 v in verts) vertices.Add(3 * v + new Vector3(i - 0.5f, j - 0.5f));
            }

        edgesMesh.Clear();

        edgesMesh.indexFormat = vertices.Count >= short.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;

        edgesMesh.SetVertices(vertices);
        edgesMesh.SetTriangles(triangles, 0);

        edgesMesh.RecalculateNormals();
        edgesMesh.RecalculateTangents();

        edgesMeshFilter.mesh = edgesMesh;
        edgesMeshCollider.sharedMesh = edgesMesh;
    }
}
