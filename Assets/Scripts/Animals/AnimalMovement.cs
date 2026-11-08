using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Vector3 objetive;
    private Vector3 velocity;
    private Rigidbody rb;

    private Vector2 moveInput;
    void FixedUpdate()
    {
        if (Vector3.Distance(this.transform.position, objetive) < 1.0f)
        {
            //matar al bicho
            Die();
            return;
        }
        moveInput = objetive - transform.position;
        moveInput = moveInput.normalized;
        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f) * speed;

        if (rb == null) //por si no tiene rigidbody que se mueva con el transform, ESTO IGUAL DA PROBLEMAS PERO DE MOMENTO LO DEJO
        {
            transform.position = transform.position + velocity * Time.deltaTime;
        }
        else
        {
            rb.linearVelocity = velocity;
        }
    }

    private void Die()
    {
        GameObject item = this.gameObject.GetComponentInChildren<Item>().gameObject;
        AnimalSpawner.RemoveAnimal(this.gameObject);
        if (item != null) { ItemsManager.RemoveObject(item); }
        Destroy(gameObject);
    }

    public void AssignObjetive(Vector3 pos)
    {
        objetive = pos;
    }


}
