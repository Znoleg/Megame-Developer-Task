using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)] private float _bulletSpeed = 1f;
    [SerializeField] private Bullet.BulletType _bulletType;

    public void Shoot(Vector2 position, Vector2 direction)
    {
        float zRotation = Utils.GetZRotation(direction);
        Bullet bullet = BulletSpawner.Instance.SpawnBullet(position, zRotation, _bulletType);
        bullet.StartFlying(_bulletSpeed);
    }

    public void Shoot(Vector2 position, Vector2 direction, Collider2D ignoreCollider, bool ignoreInfinite = false)
    {
        float zRotation = Utils.GetZRotation(direction);
        BulletSpawner.Instance.SpawnBullet(position, zRotation, _bulletType).StartFlying(_bulletSpeed, ignoreCollider, ignoreInfinite);
    }
}