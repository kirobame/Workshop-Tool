using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewLookOperation", menuName = "Custom/Operations/Look")]
public class LookOperation : Operation<Vector2, OperationHandler>
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Token cameraToken;
    [SerializeField] private RenderTexture renderTarget;

    private float ratio;
    
    public override void Initialize()
    {
        base.Initialize();
        ratio = (float)Screen.width / renderTarget.width;
    }

    protected override void During(Vector2 args, Object[] parameters)
    {
        var position = source.transform.position + offset;

        var camera = Repository.GetFirst<Camera>(cameraToken);
        var ray = camera.ScreenPointToRay(args / ratio);
        
        var plane = new Plane(Vector3.up, position);
        if (plane.Raycast(ray, out var hitDistance))
        {
            var point = ray.GetPoint(hitDistance);
            var direction = (point - position).normalized;
            
            source.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}