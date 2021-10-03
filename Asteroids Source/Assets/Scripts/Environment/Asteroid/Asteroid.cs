using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsteroidSize
{
    Small = 0, Medium = 1, Big = 2
}

[RequireComponent(typeof(FlyableEntity))]
public class Asteroid : HitableObstacle, IPoolObject
{
    [SerializeField] private AsteroidSize _asteroidSize;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private FlyableEntity _flyable;

    public AsteroidSize Size => _asteroidSize;

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public override void GetHit()
    {
        AsteroidsDestroyer.Instance.BreakAsteroid(this);
    }

    public void StartFlying(float speed)
    {
        if (_flyable == null) _flyable = GetComponent<FlyableEntity>();
        _flyable.StartFlying(speed, Vector2.up);
    }

    public void StopFlying()
    {
        if (_flyable == null) return;
        _flyable.StopFlying();
    }

    protected override void OnOtherObstacleEnter() => GetHit();

    private void OnValidate()
    {
        if (_spriteRenderer == null) _spriteRenderer.GetComponentInChildren<SpriteRenderer>();
    }
}
