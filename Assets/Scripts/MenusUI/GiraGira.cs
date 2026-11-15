using UnityEngine;

public class GiraGira : MonoBehaviour
{
    [SerializeField] private float speed;


    private void Start()
    {
    }

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
