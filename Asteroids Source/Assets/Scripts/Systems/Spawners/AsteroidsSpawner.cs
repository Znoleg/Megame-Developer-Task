using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnData
{
    public Vector2 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public SpawnData(Vector2 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

[System.Serializable]
public class AsteroidSettings
{
    [SerializeField] private AsteroidSize _size;
    [SerializeField] private FloatRange _speed;
    [SerializeField] private List<Sprite> _possibleSprites = new List<Sprite>();

    public AsteroidSize Size => _size;
    public FloatRange Speed => _speed;
    public IReadOnlyList<Sprite> Sprites => _possibleSprites;

    public AsteroidSettings(AsteroidSize size, FloatRange floatRange)
    {
        _size = size;
        _speed = floatRange;
    }
}

public class AsteroidsSpawner : Singleton<AsteroidsSpawner>
{
    [SerializeField] private int _startingAsteroidCount = 2;
    [Header("Respawn settings")]
    [SerializeField] private int _asteroidIncreaseCount = 1;
    [SerializeField] private float _respawnCooldown = 2f;
    [SerializeField][Tooltip("No asteroids respawning until all UFOs dies")] private bool _noUfoOnRespawn = true;
    [Header("Asteroid speed settings")]
    [SerializeField] private List<AsteroidSettings> _asteroidSpeeds = new List<AsteroidSettings>
    {
        new AsteroidSettings(AsteroidSize.Big, new FloatRange(0.75f, 1.25f)),
        new AsteroidSettings(AsteroidSize.Medium, new FloatRange(1.25f, 2f)),
        new AsteroidSettings(AsteroidSize.Small, new FloatRange(2f, 3f))
    };
    [SerializeField][Tooltip("Possible offset from the player when spawning. 0 means straight to the player")] 
    private FloatRange _spawnAngleOffset = new FloatRange(-45f, 45f);
    private int _currentStartAsteroids;
    private int _aliveAsteroids;

    public event Action OnAsteroidsSpawn;

    public void SpawnAsteroids(AsteroidSize asteroidSize, bool sameSpeed, params SpawnData[] asteroidData)
    {
        Asteroid[] asteroidsToSpawn = new Asteroid[asteroidData.Length];
        for (int i = 0; i < asteroidData.Length; i++)
        {
            ObjectPool.Instance.TrySpawn(ast => ast.Size == asteroidSize, 
                asteroidData[i].Position, asteroidData[i].Rotation, out asteroidsToSpawn[i]);
        }

        float speed = GetRandomSpeed(asteroidSize);
        foreach (Asteroid asteroid in asteroidsToSpawn)
        {
            asteroid.SetSprite(GetRandomSprite(asteroidSize));
            asteroid.StartFlying(speed);
            if (!sameSpeed) speed = GetRandomSpeed(asteroidSize);
        }
    }

    private void Start()
    {
        _aliveAsteroids = _currentStartAsteroids = _startingAsteroidCount;

        AsteroidsDestroyer.Instance.OnAsteroidDestroy += CountAlive;
        GameManager.Instance.OnGameStart += SpawnStartingAsteroids;
    }

    private void CountAlive(AsteroidSize asteroidSize)
    {
        if (asteroidSize == AsteroidSize.Small) _aliveAsteroids--;
        else _aliveAsteroids++;
        if (_aliveAsteroids == 0)
        {
            _currentStartAsteroids += _asteroidIncreaseCount;
            var waitRoutine = new CoroutineObject(this, WaitForRespawnReady);
            waitRoutine.Finished += SpawnStartingAsteroids;
            waitRoutine.Start();
        }
    }

    private void SpawnStartingAsteroids()
    {
        SpawnData[] asteroidDatas = new SpawnData[_currentStartAsteroids];
        for (int i = 0; i < _currentStartAsteroids; i++)
        {
            Vector2 position = GetRandomBoundPostition();
            float zRotation = Utils.GetZRotation((Vector2)PlayerMovement.Instance.transform.position - position)
                + _spawnAngleOffset.RandomValue;
            
            asteroidDatas[i] = new SpawnData(position, Quaternion.Euler(0f, 0f, zRotation));
        }

        _aliveAsteroids = _currentStartAsteroids;
        SpawnAsteroids(AsteroidSize.Big, false, asteroidDatas);
        
        OnAsteroidsSpawn?.Invoke();
    }

    private IEnumerator WaitForRespawnReady()
    {
        yield return new WaitForSeconds(_respawnCooldown);
        if (_noUfoOnRespawn && UFOSpawner.Instance.UFOCount != 0) yield return new WaitUntil(() => UFOSpawner.Instance.UFOCount == 0);
        yield break;
    }

    private Sprite GetRandomSprite(AsteroidSize asteroidSize)
    {
        IReadOnlyList<Sprite> sprites = _asteroidSpeeds.Find(pair => pair.Size == asteroidSize).Sprites;
        return sprites[Random.Range(0, sprites.Count)];
    }

    private float GetRandomSpeed(AsteroidSize asteroidSize)
    {
        FloatRange speedRange = _asteroidSpeeds.Find(pair => pair.Size == asteroidSize).Speed;
        return Random.Range(speedRange.Min, speedRange.Max);
    }

    private Vector2 GetRandomBoundPostition()
    {
        const float boundsOffset = 0.2f;

        float xBound;
        float yBound;
        int staticBound = Random.Range(0, 2);
        if (staticBound == 0) // x
        {
            xBound = Random.Range(0, 2);
            yBound = Random.Range(0f, 1f);
            xBound = xBound == 0 ? xBound - boundsOffset : xBound + boundsOffset;
        }
        else // y
        {
            xBound = Random.Range(0f, 1f);
            yBound = Random.Range(0, 2);
            yBound = yBound == 0 ? yBound - boundsOffset : yBound + boundsOffset;
        }
        
        Vector3 xViewportPosition = new Vector3(xBound, yBound, 0f);
        return Camera.main.ViewportToWorldPoint(xViewportPosition);
    }

    private void OnDestroy()
    {
        AsteroidsDestroyer.Instance.OnAsteroidDestroy -= CountAlive;
        GameManager.Instance.OnGameStart -= SpawnStartingAsteroids;
    }
}
