using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private PlayerContext playerContext;

    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform rectTransform;

    private float originalScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = rectTransform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovementAnimation();
        UpdateVisualDirection();
    }
    private void UpdateMovementAnimation()
    {
        float currentSpeed = playerContext.Rb.linearVelocity.magnitude;
        animator.SetBool("AndarDerecha", currentSpeed > 0.001f);
    }

    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    private void UpdateVisualDirection()
    {
        if (playerContext.MoveInputAllowed)
        {
            if (playerContext.orientation.x >= 0.01)
            {
                rectTransform.localScale = new Vector3(originalScale, rectTransform.localScale.y, rectTransform.localScale.z);
            }
            if (playerContext.orientation.x <= -0.01)
            {
                rectTransform.localScale = new Vector3(-originalScale, rectTransform.localScale.y, rectTransform.localScale.z);
            }
        }
    }
}
