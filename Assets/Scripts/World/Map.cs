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
    
    public Texture2D terrainMask;
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

        UpdateBorders();
    }

    public static void Draw(Vector2 position, Sprite destruction, Team team) => Draw(((int)position.x), ((int)position.y), destruction, team);
    public static void Draw(int x, int y, Sprite destruction, Team team)
    {
        int width = destruction.texture.width;
        int height = destruction.texture.height;
        int half_width = width / 2;
        int half_height = height / 2;

        float viewportX = ((float)x / Screen.width) + 0.5f;
        float viewportY = ((float)y / Screen.height) + 0.5f;

        x = Mathf.FloorToInt(viewportX * Instance.MapWidth);
        y = Mathf.FloorToInt(viewportY * Instance.MapHeight);

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                int x_pos = x + i - half_width;
                int y_pos = y + j - half_height;

                if (x_pos < 0 || x_pos >= Instance.MapWidth
                    || y_pos < 0 || y_pos >= Instance.MapHeight) continue;

                Color color = destruction.texture.GetPixel(i, j);

                if (color.a < 0.5f) continue;

                byte teamIndex = (byte)(team + 1);

                if (Instance.terrainData[x_pos, y_pos] != teamIndex)
                {
                    Instance.terrainData[x_pos, y_pos] = color == Color.white ? (byte)0 : teamIndex;
                }






                Instance.terrainMask.SetPixel(x_pos, y_pos, new Color(1, 1, 1, team == Team.Team1 ? TEAM1_MASK_VALUE : TEAM2_MASK_VALUE));
            }

        Instance.terrainMask.Apply();

        Instance.UpdateBorders();
    }

    private void UpdateBorders()
    {

    }
}
