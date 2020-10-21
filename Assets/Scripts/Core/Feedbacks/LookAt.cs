using UnityEngine;

public class LookAt : BaseHover
{
    [SerializeField] private Token targetToken;

    private Transform target;

    void Awake() => target = Repository.GetFirst<Transform>(targetToken);
    
    protected override void Update()
    {
        base.Update();

        var direction = (target.position + transform.position.y * Vector3.up) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}