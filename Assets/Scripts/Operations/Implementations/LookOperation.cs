using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewLookOperation", menuName = "Custom/Operations/Look")]
public class LookOperation : Operation<Vector2, OperationHandler>
{
    [SerializeField] private Vector3 offset;
    
    protected override void Execute(Vector2 args, Object[] parameters)
    {
        var position = source.transform.position + offset;
        
        var camera = Repository.GetFirst<Camera>(parameters[0] as Token);
        var ray = camera.ScreenPointToRay(args);
        
        var plane = new Plane(Vector3.up, position);
        if (plane.Raycast(ray, out var hitDistance))
        {
            var point = ray.GetPoint(hitDistance);
            var direction = (point - position).normalized;

            source.transform.rotation = Quaternion.LookRotation(direction);
            Debug.DrawRay(position, direction * 5, Color.red);          
        }
        Perform(args, parameters);
    }
}