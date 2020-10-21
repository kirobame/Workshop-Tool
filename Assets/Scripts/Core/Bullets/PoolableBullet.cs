using UnityEngine;

public class PoolableBullet : Poolable<Bullet>
{
    public Token Tag => tag;
    [SerializeField] private Token tag;
}