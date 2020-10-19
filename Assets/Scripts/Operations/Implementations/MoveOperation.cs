using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveOperation", menuName = "Custom/Operations/Move")]
public class MoveOperation : Operation<Vector2,OperationHandler>
{
    public float Speed
    {
        get => runtimeSpeed;
        set => runtimeSpeed = value;
    }
    
    [SerializeField] private float speed;
    [HideInInspector] private float runtimeSpeed;

    void OnEnable() => runtimeSpeed = speed;
    
    protected override void Execute(Vector2 args, Object[] parameters)
    {
        source.transform.position += new Vector3(args.x, 0, args.y) * (runtimeSpeed * Time.deltaTime);
        Perform(args, parameters);
    }
}