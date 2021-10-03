using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishingController : Singleton<FinishingController>
{
    [SerializeField] [Range(2f, 5f)] private float _finishDistance = 5f;
    [SerializeField] [Range(10f, 180f)] private float _checkPlayerBehindDegree = 165f;
    [SerializeField] private bool _killOnlyFromBehind;
    private Enemy _closestEnemy;
    private float _closestEnemyDistance;
    private Transform _playerTransform;

    public bool IsReadyToFinish => _closestEnemy != null;
    public event Action OnEnemyEnter;
    public event Action OnEnemiesLeft;

    private void Start()
    {
        _playerTransform = PlayerController.Instance.transform;
        InputController.Instance.OnSpacePressed += TryFinishEnemy;
    }

    private void TryFinishEnemy()
    {
        if (!IsReadyToFinish) return;
        PlayerController.Instance.FinishEnemy(_closestEnemy);
        SetClosestEnemy(null);
    }

    // Can be replaced by collider checking
    private void Update()
    {
        Enemy closestEnemy = null;
        foreach (Enemy enemy in EnemiesManager.Instance.AliveEnemies)
        {
            if (CanFinishEnemy(enemy, out float distanceToEnemy))
            {
                if (closestEnemy != null && enemy != _closestEnemy)
                {
                    if (distanceToEnemy < _closestEnemyDistance) closestEnemy = enemy;
                }
                else
                {
                    closestEnemy = enemy;
                    _closestEnemyDistance = distanceToEnemy;
                }
            }
        }

        SetClosestEnemy(closestEnemy);
    }

    private void SetClosestEnemy(Enemy closest)
    {
        bool wasNull = _closestEnemy == null;
        bool currentIsNull = closest == null;

        _closestEnemy = closest;
        if (wasNull && !currentIsNull) OnEnemyEnter?.Invoke();
        else if (!wasNull && currentIsNull) Invoke(nameof(TryInvokeEnemiesLeft), 0.5f); // Delay to make sure that Update didn't find enemy
    }

    void TryInvokeEnemiesLeft()
    {
        if (_closestEnemy == null) OnEnemiesLeft?.Invoke();
    }

    private bool CanFinishEnemy(Enemy enemy, out float distance)
    {
        distance = (enemy.transform.position - _playerTransform.position).magnitude;
        return distance <= _finishDistance && !enemy.IsRagdollActivated 
            && (!_killOnlyFromBehind || IsPlayerBehind(enemy));
    }

    private bool IsPlayerBehind(Enemy enemy)
    {
        Vector3 directionToPlayer = PlayerController.Instance.transform.position - enemy.transform.position;
        float angle = Utils.GetVectorAngle(directionToPlayer, -enemy.transform.forward, ignoreY: true);
        return Mathf.Abs(angle) <= _checkPlayerBehindDegree / 2;
    }

    private void OnDestroy()
    {
        InputController.Instance.OnSpacePressed -= TryFinishEnemy;
    }
}
