using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class Carcel : MonoBehaviour
{

    [SerializeField] private Image mask;
    [SerializeField] private Sprite startMask;
    [SerializeField] private Sprite startMaskEdge;
    [Space]
    [SerializeField] private MeshFilter edgesMeshFilter;
    [SerializeField] private MeshCollider edgesMeshCollider;


    public int MapWidth => terrainMask.width;
    public int MapHeight => terrainMask.height;
    private List<Vector3> edgesVertices = new List<Vector3>(32000);
    private List<int> edgesTriangles = new List<int>(32000);
    private Mesh edgesMesh;

    RectTransform rect;

    float imageWidth;
    float imageHeight;

    float pixelWidth;
    float pixelHeight;

    private Texture2D terrainMask;


    private readonly Vector3[] PIXEL_VERTS = new Vector3[4]
    {
        new Vector3(0, 0),
        new Vector3(1, 0),
        new Vector3(0, 1),
        new Vector3(1, 1)
    };
    private readonly int[] PIXEL_TRIANGS = new int[6] { 2, 1, 0, 2, 3, 1 };

    private void Awake()
    {
        rect = mask.rectTransform;

        imageWidth = rect.rect.width;
        imageHeight = rect.rect.height;

        terrainMask = Instantiate(startMask.texture);

        pixelWidth = imageWidth / terrainMask.width;
        pixelHeight = imageHeight / terrainMask.height;

        InitMask();
    }

    private void InitMask()
    {
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
                    mask_pixels[index] = new Color(1f, 1f, 1f, 0f);
                }
                else if (color.r > 0.5f && color.b < 0.5f)
                {
                    mask_pixels[index] = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    mask_pixels[index] = new Color(1f, 1f, 1f, 1f);
                }
            }
        }

        terrainMask.SetPixels(mask_pixels);
        terrainMask.Apply();

        UpdateEdgesMesh();
    }

    private void UpdateEdgesMesh()
    {
        edgesVertices.Clear();
        edgesTriangles.Clear();

        int width = terrainMask.width;
        int height = terrainMask.height;

        Color[] pixels = terrainMask.GetPixels();

        Vector3 centerOffset = new Vector3(
            -terrainMask.width * pixelWidth / 2f,
            -terrainMask.height * pixelHeight / 2f,
            0f
        );

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int index = i + j * width;

                if (pixels[index].a < 0.5f)
                    continue;

                int vertices_count = edgesVertices.Count;

                Vector3 offset = new Vector3(
                    i * pixelWidth,
                    j * pixelHeight,
                    0f
                );

                for (int k = 0; k < PIXEL_VERTS.Length; k++)
                {
                    edgesVertices.Add(
                        new Vector3(
                            PIXEL_VERTS[k].x * pixelWidth,
                            PIXEL_VERTS[k].y * pixelHeight,
                            0f
                        ) + offset + centerOffset
                    );
                }

                for (int k = 0; k < PIXEL_TRIANGS.Length; k++)
                {
                    edgesTriangles.Add(
                        vertices_count + PIXEL_TRIANGS[k]
                    );
                }
            }
        }

        edgesMesh.Clear();
        edgesMesh.SetVertices(edgesVertices);
        edgesMesh.SetTriangles(edgesTriangles, 0);

        edgesMesh.RecalculateNormals();
        edgesMesh.RecalculateBounds();

        edgesMeshFilter.mesh = edgesMesh;
        edgesMeshCollider.sharedMesh = null;
        edgesMeshCollider.sharedMesh = edgesMesh;
    }


}
