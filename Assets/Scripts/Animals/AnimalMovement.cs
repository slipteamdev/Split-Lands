using System.Collections.Generic;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float atraccionForce = 3.0f;

    private Vector3 objetive;
    private Vector3 velocity;
    private Rigidbody rb;

    private Vector2 moveInput;

    private Animator animator;
    private List<GameObject> liras = new List<GameObject>(); //aqui cosas

    public bool moving = true;
    public bool dieWhenObjetive = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (!moving) return;
        if (objetive == null) return;

        moveInput = objetive - transform.position;
        moveInput = moveInput.normalized;

        if (AnimalSpawner.liras.Count > 0 && this.gameObject.GetComponent<Caracol>() == null && this.gameObject.GetComponent<Monkey>() == null) // si hay liras
        {
            GameObject nearest = null;
            float minDist = float.MaxValue;
            foreach (GameObject lira in AnimalSpawner.liras)
            {
                if (lira == null) continue;

                float distance = Vector2.Distance(this.transform.position, lira.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    nearest = lira;
                }
            }

            if (nearest != null)
            {
                if (Vector3.Distance(this.transform.position, nearest.transform.position) > 1.0f)
                {
                    Vector2 dir = nearest.transform.position - this.transform.position;
                    dir = dir.normalized * atraccionForce;
                    moveInput = moveInput + dir;
                    moveInput = moveInput.normalized;
                }
            }

            /*

            foreach (GameObject lira in AnimalSpawner.liras)
            {
                if (Vector3.Distance(this.transform.position, lira.transform.position) > 1.0f)
                {
                    Vector2 dir = lira.transform.position - this.transform.position;
                    dir = dir.normalized * atraccionForce; 
                    moveInput = moveInput + dir;
                }
            }
            moveInput = moveInput.normalized;
            */
        }


        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f) * speed;

        if (animator != null)
        {
            animator.SetBool("derecha", moveInput.x >= 0);
        }

        if (Vector3.Distance(this.transform.position, objetive) < 1.0f)
        {
            velocity = Vector3.zero;
            if (dieWhenObjetive) // por si quiero que se muera al llegar al objetivo
            {
                //matar al bicho
                Die();
                return;
            }
        }

        if (rb == null) //por si no tiene rigidbody que se mueva con el transform, ESTO IGUAL DA PROBLEMAS PERO DE MOMENTO LO DEJO
        {
            transform.position = transform.position + velocity * Time.deltaTime;
        }
        else
        {
            rb.linearVelocity = velocity;
        }
    }

    public void Die()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        GameObject item = null;
        foreach (Transform child in children)
            if(child.TryGetComponent<Item>(out Item i))
            {
                item = child.gameObject;
                break;
            }

        AnimalSpawner.RemoveAnimal(this.gameObject);
        if (item != null) { ItemsManager.RemoveObject(item); }
        Destroy(this.gameObject);
    }

    public void Soltar()
    {
        if (transform.childCount <= 1) // si no tiene objeto
        {
            return;
        }

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra
        this.transform.GetChild(1).transform.SetParent(canva.transform);

    }

    public void AssignObjetive(Vector3 pos) // AQUI MANDALE POSITION, NO LOCALPOSITION, quero llorar
    {
        objetive = pos;
    }


}
