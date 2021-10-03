using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlyableEntity))]
public class Bullet : Obstacle, IPoolObject
{
    private FlyableEntity _flyable;
    private BulletType _currentType = BulletType.Friend;

    public void SetType(BulletType bulletType)
    {
        if (bulletType == _currentType) return;
        
        if (bulletType == BulletType.Friend) SpriteRenderer.sprite = BulletSpawner.Instance.FriendBulletSprite;
        else SpriteRenderer.sprite = BulletSpawner.Instance.EnemyBulletSprite;
        _currentType = bulletType;
    }

    public void StartFlying(float speed)
    {
        _flyable.StartFlying(speed, Vector2.up, ScreenSwaper.Instance.ScreenWorldSize.x);
    }

    public void StartFlying(float speed, Collider2D ignoreCollider, bool ignoreInfinite = false)
    {
        //StopFlying();
        Physics2D.IgnoreCollision(Collider, ignoreCollider, true);
        StartFlying(speed);
        if (!ignoreInfinite)
        {
            new CoroutineObject(this, () => ReturnCollision(ignoreCollider)).Start();
        }
    }

    public void StopFlying()
    {
        _flyable.StopFlying();
    }

    protected override void OnHitableEnter(Collider2D entered, IHitable hitable)
    {
        hitable.GetHit();
        DestroyBullet();
    }

    private void Awake()
    {
        _flyable = GetComponent<FlyableEntity>();
        _flyable.OnMaxDistancePass += DestroyBullet;
    }

    private void DestroyBullet()
    {
        StopFlying();
        ObjectPool.Instance.ReturnToPool(this);
    }

    private IEnumerator ReturnCollision(Collider2D ignoredCollider)
    {
        const float checkDelay = 0.1f;
        yield return new WaitForSeconds(checkDelay);
        
        yield return new WaitUntil(() => Collider == null || ignoredCollider == null || Collider.Distance(ignoredCollider).distance > 0f);
        if (Collider == null || ignoredCollider == null) yield break;
        Physics2D.IgnoreCollision(Collider, ignoredCollider, false);
    }

    private void OnValidate()
    {
        if (SpriteRenderer == null) transform.GetComponentInChildren<SpriteRenderer>(); 
    }

    public enum BulletType
    {
        Friend, Enemy
    }
}

