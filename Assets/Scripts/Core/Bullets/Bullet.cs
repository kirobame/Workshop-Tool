using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    
    public void Fire(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        transform.position += transform.forward * (Time.deltaTime * speed);
    }
}