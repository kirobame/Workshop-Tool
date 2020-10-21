using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveOperation", menuName = "Custom/Operations/Move")]
public class MoveOperation : Operation<Vector2,OperationHandler>
{
    private const float castHeight = 0.5f;
    
    public float Speed
    {
        get => runtimeSpeed;
        set => runtimeSpeed = value;
    }
    
    [SerializeField] private float speed;
    [SerializeField] private int rayAmount;
    [SerializeField] private float angleCheck;
    [SerializeField] private LayerMask collisionMask;
    
    [HideInInspector] private float runtimeSpeed;

    void OnEnable() => runtimeSpeed = speed;
    
    protected override void During(Vector2 args, Object[] parameters)
    {
        var magnitude = (runtimeSpeed * Time.deltaTime);
        var delta = new Vector3(args.x, 0, args.y) * magnitude;

        var direction = delta.normalized;
        var origin = source.transform.position + direction * 0.5f + castHeight * Vector3.up;
        
        var ray = new Ray(origin + delta, Vector3.down);
        if (!Physics.Raycast(ray, 100f)) return;

        var halfAngle = angleCheck * 0.5f;
        var start = Quaternion.AngleAxis(-halfAngle, Vector3.up) * direction;

        var step = angleCheck / rayAmount;
        for (var i = 0; i < rayAmount; i++)
        {
            var castDirection = Quaternion.AngleAxis(step * i, Vector3.up) * start;
            var castOrigin = source.transform.position + castDirection * 0.5f + castHeight * Vector3.up;
            ray = new Ray(castOrigin, castDirection);
            
            if (Physics.Raycast(ray, magnitude, collisionMask)) return;
        }

        source.transform.position += delta;
    }
}