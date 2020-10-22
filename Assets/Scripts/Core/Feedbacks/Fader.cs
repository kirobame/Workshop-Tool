using System.Collections;
using UnityEngine;

public abstract class Fader : MonoBehaviour
{
    [SerializeField] private AnimationCurve fadeInCurve, fadeOutCurve;
    [SerializeField] private Vector2 times;

    private Coroutine routine;

    public virtual void FadeIn()
    {
        HandleInterruption();
        routine = StartCoroutine(FadeRoutine(fadeInCurve, new Vector2(0f,1f), times.x));
    }
    public virtual void  FadeOut()
    {
        HandleInterruption();
        routine = StartCoroutine(FadeRoutine(fadeOutCurve, new Vector2(1,0f), times.y));
    }

    protected void HandleInterruption()
    {
        if (routine != null) StopCoroutine(routine);
    }
    
    private IEnumerator FadeRoutine(AnimationCurve curve, Vector2 range, float goal)
    {
        var time = 0f;
        while (time < goal)
        {
            Apply(Mathf.Lerp(range.x,range.y, curve.Evaluate(time / goal)));
            
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        Apply(Mathf.Lerp(range.x,range.y, curve.Evaluate(1f)));

        routine = null;
    }

    protected abstract void Apply(float alpha);
}