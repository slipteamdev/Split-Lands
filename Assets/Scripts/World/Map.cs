using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private SpriteMask terrain2SpriteMask;
    [SerializeField] private Sprite terrainsStartMask;

    private void Awake()
    {
        terrain2SpriteMask.sprite = Texture2D.Instantiate(terrainsStartMask);

        Debug.Log(terrain2SpriteMask.sprite.name);

        terrain2SpriteMask.sprite.texture.SetPixel(4, 4, Color.black);
        terrain2SpriteMask.sprite.texture.Apply();
    }
}
