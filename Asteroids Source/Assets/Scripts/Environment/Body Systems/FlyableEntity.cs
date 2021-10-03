using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlyableEntity : MonoBehaviour, IFlyDirection
{
    private float _speed;
    private CoroutineObject _flyRoutine;
    private float _maxDistance;
    private Vector2 _direction;

    public float TraveledDistance { get; private set; } = 0f;
    public Vector2 FlyDirection => transform.up;
    public event Action OnMaxDistancePass;

    public void StartFlying(float speed, Vector2 direction, float maxDistance = float.PositiveInfinity)
    {
        TraveledDistance = 0f;

        _speed = speed;
        _maxDistance = maxDistance;
        _direction = direction;
        _flyRoutine = new CoroutineObject(this, FlyRountine);
        _flyRoutine.Start();
    }

    public void StopFlying()
    {
        if (_flyRoutine == null || !_flyRoutine.IsProcessing) return;
        _speed = 0f;
        _flyRoutine.Stop();
        _flyRoutine = null;
    }

    private IEnumerator FlyRountine()
    {
        while (true)
        {
            float step = _speed * Time.deltaTime;
            transform.Translate(_direction * step);
            TraveledDistance += step;
            if (TraveledDistance >= _maxDistance)
            {
                OnMaxDistancePass?.Invoke();
                yield break;
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (_flyRoutine != null && _flyRoutine.IsProcessing)
        {
            _flyRoutine.Stop();
            _flyRoutine = null;
        }
    }
}

