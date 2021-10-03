using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : Singleton<EnemiesManager>
{
    [SerializeField] private float _corpseLifetime = 5f;
    [SerializeField] private Enemy _enemyPrefab;
    private readonly List<Enemy> _aliveEnemies = new List<Enemy>();
    private readonly Vector3 _enemiesRespawnPosMin = new Vector3(-10f, 0f, -10f);
    private readonly Vector3 _enemiesRespawnPosMax = new Vector3(10f, 0f, 10f);

    public IReadOnlyList<Enemy> AliveEnemies => _aliveEnemies;

    public void RespawnEnemy(Enemy enemyToRespawn)
    {
        Vector3 respawnPosition = new Vector3(
            Random.Range(_enemiesRespawnPosMin.x, _enemiesRespawnPosMax.x),
            Random.Range(_enemiesRespawnPosMin.y, _enemiesRespawnPosMax.y),
            Random.Range(_enemiesRespawnPosMin.z, _enemiesRespawnPosMax.z));

        DestroyEnemy(enemyToRespawn);
        var waitRoutine = Utils.CreateWaitCoroutine(this, _corpseLifetime);
        waitRoutine.Finished += SpawnOnPosition;
        waitRoutine.Start();

        void SpawnOnPosition()
        {
            waitRoutine.Finished -= SpawnOnPosition;
            SpawnEnemy(respawnPosition);
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        Enemy enemyInstance = Instantiate(_enemyPrefab);
        enemyInstance.transform.position = position;
        enemyInstance.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        _aliveEnemies.Add(enemyInstance);
    }

    public void DestroyEnemy(Enemy enemyToDestroy)
    {
        if (!_aliveEnemies.Contains(enemyToDestroy)) return;

        enemyToDestroy.SetRagdollActive(true);
        var waitRoutine = Utils.CreateWaitCoroutine(this, _corpseLifetime);
        waitRoutine.Finished += OnFinish;
        waitRoutine.Start();

        void OnFinish()
        {
            waitRoutine.Finished -= OnFinish;
            DestroyCorpse(enemyToDestroy);
        }
    }

    private void Start()
    {
       _aliveEnemies.AddRange(FindObjectsOfType<Enemy>(true));
    }

    private void DestroyCorpse(Enemy corpse)
    {
        _aliveEnemies.Remove(corpse);
        Destroy(corpse.gameObject);
    }
}
