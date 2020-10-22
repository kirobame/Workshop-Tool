using UnityEngine;

public class EndTitle : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool state;

    public void SlideIn()
    {
        animator.SetTrigger("In");
        state = true;
    }
    public void TrySlideOut()
    {
        if (state == false) return;
        
        animator.SetTrigger("Out");
        state = false;
    }
}