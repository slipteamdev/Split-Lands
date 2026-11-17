using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Vector3 objetive;
    private Vector3 velocity;
    private Rigidbody rb;

    private Vector2 moveInput;

    public bool moving = true;
    public bool dieWhenObjetive = true;
    void FixedUpdate()
    {
        if (!moving) return;
        if (objetive == null) return;

        moveInput = objetive - transform.position;
        moveInput = moveInput.normalized;
        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f) * speed;

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
