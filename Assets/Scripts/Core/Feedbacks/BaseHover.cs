using UnityEngine;

public abstract class BaseHover : MonoBehaviour
{
    [SerializeField] private Vector2 yRange;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private float yTimeGoal;

    protected float yTime;
    
    protected virtual void Update()
    {
        if (yTime > yTimeGoal) yTime = 0f;

        var position = transform.position;
        position.y = Mathf.Lerp(yRange.x, yRange.y, yCurve.Evaluate(Mathf.Clamp01(yTime / yTimeGoal)));

        transform.position = position;
        yTime += Time.deltaTime;
    }
}