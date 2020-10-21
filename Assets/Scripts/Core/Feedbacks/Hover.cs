using UnityEngine;


public class Hover : BaseHover
{
    [SerializeField] private float rotationSpeed;
    
    protected override void Update()
    {
        base.Update();
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}