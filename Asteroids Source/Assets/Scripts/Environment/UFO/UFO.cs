using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Shooter), typeof(FlyableEntity))]
public class UFO : HitableObstacle
{
    [SerializeField] private FloatRange _shootFrequency = new FloatRange(2f, 5f);
    private Shooter _shooter;
    private FlyableEntity _flyable;
    private CoroutineObject _shootingRoutine;

    public event Action OnDying; 

    public void StartFlying(FlyDirection direction)
    {
        Vector2 vectorDir;

        if (direction == FlyDirection.Left) vectorDir = Vector2.left;
        else vectorDir = Vector2.right;
        _flyable.StartFlying(ScreenSwaper.Instance.ScreenWorldSize.x / 10f, vectorDir);

        DoInvulnerable();

        _shootingRoutine.Start();
    }

    public override void GetHit()
    {
        OnDying?.Invoke();
        Destroy(gameObject);
    }

    protected override void OnOtherObstacleEnter() => GetHit();

    private void Awake()
    {
        _flyable = GetComponent<FlyableEntity>();
        _shooter = GetComponent<Shooter>();

        _shootingRoutine = new CoroutineObject(this, StartShooting);
    }

    private IEnumerator StartShooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_shootFrequency.Min, _shootFrequency.Max));
            Shoot();
        }
    }

    private void Shoot()
    {
        if (PlayerMovement.Instance == null) return;

        const float additionalOffset = 0f;
        Vector2 startPosition = transform.position;
        Vector2 endPosition = PlayerMovement.Instance.transform.position;
        Vector2 direction = (endPosition - startPosition);

        float offset = Collider.bounds.size.x + additionalOffset;
        startPosition = Vector3.MoveTowards(startPosition, endPosition, offset);

        _shooter.Shoot(startPosition, direction, Collider);
    }

    public enum FlyDirection
    {
        Left = 0, Right = 1
    }
}

