using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : Item
{
    [SerializeField] private Sprite destruction;
    private GameObject area;
    private float showAreaTime = 0.25f;
    private Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        area = this.gameObject.transform.GetChild(0).gameObject;
        area.SetActive(false);

    }
    public override void Execute()
    {

        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos
        direction = playerContext.orientation;
        Map.Draw(this.PlayerOwner.transform.localPosition, destruction, playerContext.Team, direction);
        StartCoroutine(ShowArea());
    }

    IEnumerator ShowArea()
    {


        this.gameObject.GetComponent<Image>().enabled = false;
        this.transform.position = playerContext.transform.position;
        area.SetActive(true);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);

        Destroy(this.gameObject);
    }
}
