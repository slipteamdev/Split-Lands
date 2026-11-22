
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Torpedo : MonoBehaviour
{
    Vector2 direction;
    float speed;
    Sprite destruction;
    Map.Team team;
    private Rigidbody rb;
    private GameObject area;
    private float showAreaTime = 0.25f;

    bool moving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector2 direction, float speed, Sprite destruction, Map.Team team)
    {
        this.direction = direction;
        this.speed = speed;
        this.destruction = destruction;
        this.team = team;
        rb = GetComponent<Rigidbody>();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        rb.linearVelocity = direction * speed;

        area = this.gameObject.transform.GetChild(0).gameObject;
        area.SetActive(false);

        moving = true;
        if (Map.ReadPixelAt(transform.localPosition.x, transform.localPosition.y) != team)
        {
            Explode();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            // Debug.Log(transform.localPosition);
            if (Map.ReadPixelAt(transform.localPosition.x, transform.localPosition.y) != team)
            {
                Explode();
                moving = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("MapEdge") || collision.gameObject.CompareTag("Ray") || collision.gameObject.CompareTag("Player")) //si choca con un mesh colider que deberia ser el rayito
        //{
        Explode();
        //}
    }

    private void Explode()
    {
        moving = false;
        rb.linearVelocity = Vector2.zero;
        this.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Image>().enabled = false;

        StartCoroutine(ShowArea());
    }

    IEnumerator ShowArea()
    {
        area.SetActive(true);


        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Map.Draw(this.transform.localPosition, destruction, team, direction);

        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);

        Destroy(this.gameObject);
    }
}
