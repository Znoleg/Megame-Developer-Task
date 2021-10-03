using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private Pool<Asteroid> _asteroidsPool;
    [SerializeField] private Pool<Bullet> _bulletsPool;

    public static ObjectPool Instance { get; private set; }

    public void ReturnToPool<T>(T component) where T : MonoBehaviour, IPoolObject
    {
        component.gameObject.SetActive(false);
       
        if (typeof(T) == typeof(Asteroid))
        {
            _asteroidsPool.MarkReturned();
        }
        else if (typeof(T) == typeof(Bullet))
        {
            _bulletsPool.MarkReturned();
        }
    }

    public bool TrySpawn<T>(Func<T, bool> predicate, Vector3 position,
        Quaternion rotation, out T component) where T : MonoBehaviour, IPoolObject
    {
        component = null;
        if (typeof(T) == typeof(Asteroid))
        {
            if (!_asteroidsPool.TryGetElement(predicate as Func<Asteroid, bool>, out Asteroid asteroid)) return false;
            component = asteroid as T;
            _asteroidsPool.PushElement(asteroid);
        }
        else if (typeof(T) == typeof(Bullet))
        {
            if (!_bulletsPool.TryGetElement(predicate as Func<Bullet, bool>, out Bullet bullet)) return false;
            component = bullet as T;
            _bulletsPool.PushElement(bullet);
        }

        SetUpObject(component.gameObject, position, rotation);
        return component != null;
    }

    public void Spawn<T>(Vector3 position, Quaternion rotation, out T component) where T : MonoBehaviour, IPoolObject
    {
        component = null;
        if (typeof(T) == typeof(Asteroid))
        {
            component = _asteroidsPool.PopAndPush() as T;
        }
        else if (typeof(T) == typeof(Bullet))
        {
            component = _bulletsPool.PopAndPush() as T;
        }
        
        SetUpObject(component.gameObject, position, rotation);
    }

    private void SetUpObject(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.SetActive(true);
    }

    private void Start()
    {
        _asteroidsPool.CreateInstances(transform);
        _bulletsPool.CreateInstances(transform);
    }

    private void Awake()
    {
        if (Instance != null) Debug.LogError($"Multiple {GetType().Name} instance!");
        else Instance = this;
    }
}