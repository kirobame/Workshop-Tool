using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewLookOperation", menuName = "Custom/Operations/Look")]
public class LookOperation : Operation<Vector3, OperationHandler>
{
    protected override void During(Vector3 args, Object[] parameters)
    {
        source.transform.rotation = Quaternion.LookRotation(args);
    }
}