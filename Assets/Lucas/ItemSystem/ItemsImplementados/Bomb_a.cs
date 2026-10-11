using Mono.Cecil.Cil;
using Unity.ProjectAuditor.Editor;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bomb_a : Item
{
    [SerializeField] private Sprite destruction;
    [SerializeField] private float speed;

    private PlayerInputController player;
    private Rigidbody rb;
    private GameObject area;
    private float showAreaTime = 0.25f;


    private void Start()
    {
        area = this.gameObject.transform.GetChild(0).gameObject;
        area.SetActive(false);
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    public override void Execute()
    {
        player = this.PlayerOwner.GetComponent<PlayerInputController>();

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);



        Vector2 direction = player.orientation;
        transform.position = player.transform.position + new Vector3(direction.x * 2, direction.y * 2, 0);
        this.GetComponent<Collider>().enabled = true;
        rb.linearVelocity = direction * speed;

        //StartCoroutine(ThrowBomb());

        // Mensaje identificador
        Debug.Log($" ha usado el objeto " + $"{ItemName} .");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.GetComponent<MeshCollider>() != null ) //si choca con un mesh colider que deberia ser el rayito
        {
            rb.linearVelocity = Vector2.zero;
            this.GetComponent<Collider>().enabled = false;
            this.gameObject.GetComponent<Image>().enabled = false;

            StartCoroutine(ShowArea());
        }
    }

    IEnumerator ShowArea()
    {
        area.SetActive(true);


        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Map.Draw(this.transform.localPosition, destruction, player.team, direction);

        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);

        Destroy(this.gameObject);
    }
}
