using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BulletSpawner : Singleton<BulletSpawner>
{
    [SerializeField] private Sprite _friendBulletSprite;
    [SerializeField] private Sprite _enemyBulletSprite;

    public Sprite FriendBulletSprite => _friendBulletSprite;
    public Sprite EnemyBulletSprite => _enemyBulletSprite;
    public event Action OnBulletSpawn;

    public Bullet SpawnBullet(Vector2 position, float zRotation, Bullet.BulletType bulletType)
    {
        ObjectPool.Instance.Spawn(position, Quaternion.Euler(0f, 0f, zRotation), out Bullet bullet);
        bullet.StopFlying();
        bullet.SetType(bulletType);

        OnBulletSpawn?.Invoke();
        return bullet;
    }
}

