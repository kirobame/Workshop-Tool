using UnityEngine;

public interface IHittable
{
    bool Hit(Bullet bullet, RaycastHit hit);
    bool HasAlreadyBeenHit { get; }
}