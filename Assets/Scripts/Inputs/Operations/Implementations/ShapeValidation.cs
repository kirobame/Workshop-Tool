using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeValidation", menuName = "Custom/Operations/Shape Validation")]
[OperationParameterTypes(typeof(Object))]
[OperationParameterNames("Optional")]
public class ShapeValidation : Operation<Shape,OperationHandler>
{
    [SerializeField] private Shape shape;

    protected override void During(Shape args, Object[] parameters)
    {
        if (args == shape) CallPerform(Null.Default, parameters);
    }
}