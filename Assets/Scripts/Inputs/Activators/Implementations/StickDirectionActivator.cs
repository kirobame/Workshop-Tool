using UnityEngine;

[CreateAssetMenu(fileName = "NewStickDirectionActivator", menuName = "Custom/Activators/Stick Direction")]
public class StickDirectionActivator : Activator<Vector2>
{
    public override string ShortName => "StDir";

    protected override void OnPerformed(Vector2 input) => Execute(new Vector3(input.x, 0, input.y).normalized);
}