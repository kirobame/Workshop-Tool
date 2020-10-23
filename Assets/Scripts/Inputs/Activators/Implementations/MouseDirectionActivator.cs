using UnityEngine;

[CreateAssetMenu(fileName = "NewMouseDirectionActivator", menuName = "Custom/Activators/Mouse Direction")]
public class MouseDirectionActivator : Activator<Vector2>
{
    public override string ShortName => "MDir";

    [SerializeField] private Token cameraToken;
    
    [SerializeField] private Token sourceToken;
    [SerializeField] private Vector3 offset;
    [SerializeField] private RenderTexture renderTarget;

    private float ratio;
    
    public override void Initialize() => ratio = (float)Screen.width / renderTarget.width;

    protected override void OnPerformed(Vector2 input)
    {
        var source = Repository.GetFirst<Transform>(sourceToken);
        var camera = Repository.GetFirst<Camera>(cameraToken);
        
        var position = source.transform.position + offset;

        var ray = camera.ScreenPointToRay(input / ratio);
        
        var plane = new Plane(Vector3.up, position);
        if (plane.Raycast(ray, out var hitDistance))
        {
            var point = ray.GetPoint(hitDistance);
            var direction = (point - position).normalized;
            
            Execute(direction);
        }
    }
}