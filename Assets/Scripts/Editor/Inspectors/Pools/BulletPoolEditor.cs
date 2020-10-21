using UnityEditor;

[CustomEditor(typeof(BulletPool))]
[CanEditMultipleObjects]
public class BulletPoolEditor : PoolEditor<Bullet, PoolableBullet> { }