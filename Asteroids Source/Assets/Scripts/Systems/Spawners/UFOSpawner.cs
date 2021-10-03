using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class UFOSpawner : Singleton<UFOSpawner>
{
    [SerializeField] private UFO _ufoPrefab;
    [SerializeField] private FloatRange _spawnDelay = new FloatRange(20f, 40f);
    [SerializeField] [Tooltip("Does it need to refresh spawn cooldown when starting new level?")] private bool _refreshCooldown;
    [SerializeField] private FloatRange _viewportYBounds = new FloatRange(0.2f, 0.8f);
    private CoroutineObject _spawnRoutine;

    public int UFOCount { get; private set; } = 0;
    public event Action OnUFOSpawn;
    public event Action OnUFODeath;

    private void Start()
    {
        if (_refreshCooldown) GameManager.Instance.OnNewLevelStart += Respawn;
        else GameManager.Instance.OnGameStart += Respawn;
    }

    private void Respawn()
    {
        if (_spawnRoutine != null) // Refresh if already waiting and new level has started
        {
            _spawnRoutine.Stop();
            _spawnRoutine = null;
        }
        _spawnRoutine = new CoroutineObject(this, SpawnDelayed);
        _spawnRoutine.Start();
    }

    private IEnumerator SpawnDelayed()
    {
        float waitTime = Random.Range(_spawnDelay.Min, _spawnDelay.Max);
        yield return new WaitForSeconds(waitTime);

        int xViewport = Random.Range(0, 2);
        Vector2 viewportPosition = new Vector2(xViewport, Random.Range(_viewportYBounds.Min, _viewportYBounds.Max));
        Vector2 worldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);

        UFO instance = Instantiate(_ufoPrefab);
        instance.transform.position = worldPosition;
        instance.OnDying += DecreaseAndSpawn;

        UFO.FlyDirection flyDirection = UFO.FlyDirection.Left;
        if (xViewport == 0) flyDirection = UFO.FlyDirection.Right;
        instance.StartFlying(flyDirection);

        UFOCount++;
        OnUFOSpawn?.Invoke();
    }

    private void DecreaseAndSpawn()
    {
        UFOCount--;
        OnUFODeath?.Invoke();
        Respawn();
    }

    private void OnDestroy()
    {
        if (_refreshCooldown) GameManager.Instance.OnNewLevelStart -= Respawn;
        else GameManager.Instance.OnGameStart -= Respawn;
    }
}

