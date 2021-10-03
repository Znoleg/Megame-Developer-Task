using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidsDestroyer : Singleton<AsteroidsDestroyer>
{
    [SerializeField] private FloatRange _breakDegree = new FloatRange(-45f, 45f);

    public event Action<AsteroidSize> OnAsteroidDestroy;

    public void BreakAsteroid(Asteroid asteroid)
    {
        AsteroidSize asteroidSize = asteroid.Size;
        if (asteroidSize == AsteroidSize.Small)
        {
            ObjectPool.Instance.ReturnToPool(asteroid);
        }
        else
        {
            Vector2 position = asteroid.transform.position;
            Quaternion rotation = asteroid.transform.rotation;

            DoShrink(asteroidSize, position, rotation);

            asteroid.StopFlying();
            ObjectPool.Instance.ReturnToPool(asteroid);
        }
        OnAsteroidDestroy?.Invoke(asteroidSize);
    }

    private void DoShrink(AsteroidSize asteroidSize, Vector2 asteroidPosition, Quaternion asteroidRotation)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (asteroidSize == AsteroidSize.Small)
        {
            Debug.LogWarning("Trying to shrink small asteroid!");
            return;
        }
#endif
        
        SpawnData[] asteroidDatas = new SpawnData[2];
        asteroidDatas[0] = ConstructData(asteroidPosition, asteroidRotation, Random.Range(_breakDegree.Min, 0f));
        asteroidDatas[1] = ConstructData(asteroidPosition, asteroidRotation, Random.Range(0f, _breakDegree.Max));
        AsteroidsSpawner.Instance.SpawnAsteroids(asteroidSize - 1, true, asteroidDatas);
    }

    private SpawnData ConstructData(Vector2 asteroidPosition, Quaternion asteroidRotation, float degree)
    {
        const float circleRndRadius = 1f;
        return new SpawnData(asteroidPosition + Random.insideUnitCircle * circleRndRadius, 
            asteroidRotation * Quaternion.Euler(Vector3.forward * degree));
    }
}
