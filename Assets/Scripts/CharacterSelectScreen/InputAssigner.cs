using UnityEngine;

public class InputAssigner : MonoBehaviour
{
    [SerializeField] private GameObject input1;
    [SerializeField] private GameObject input2;
    void Awake()
    {
        PlayerInputManagerInitialicer.Init(input1, input2);
    }


}
