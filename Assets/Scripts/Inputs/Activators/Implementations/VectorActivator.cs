using UnityEngine;

[CreateAssetMenu(fileName = "NewVectorActivator", menuName = "Custom/Activators/Vector2")]
public class VectorActivator : UpdatableActivator<Vector2>
{
    public override string ShortName => "V2d";
}