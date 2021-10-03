using System;
using System.Collections;
using UnityEngine;

public class PlayerBody : HitableObstacle
{
    [SerializeField] private int _lives;
    [SerializeField] private float _respawnCooldown = 2f;
    private CoroutineObject _respawnRoutine;

    public static PlayerBody Instance { get; private set; }
    public int LivesCount => _lives;
    public event Action OnDying;
    public event Action<int> OnHit;

    public void MakeInvulnerable() => DoInvulnerable();

    public override void GetHit()
    {
        _lives--;
        OnHit?.Invoke(_lives);

        if (_lives == 0)
        {
            Die();
        }
        else
        {
            _respawnRoutine.Start();
        }
    }

    protected override void OnOtherObstacleEnter() => GetHit();

    private void Awake()
    {
        if (Instance != null) Debug.LogError($"Multiple {GetType().Name} instance!");
        else Instance = this;
    
        _respawnRoutine = new CoroutineObject(this, RespawnRoutine);
    }

    private IEnumerator RespawnRoutine()
    {
        Collider.enabled = false;
        SpriteRenderer.enabled = false;
        PlayerMovement.Instance.StopAllMovement(true);
        
        yield return new WaitForSeconds(_respawnCooldown);
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        SpriteRenderer.enabled = true;
        PlayerMovement.Instance.StartMovement();
        Collider.enabled = true;
        DoInvulnerable();
    }

    private void Die()
    {
        OnDying?.Invoke();
        Destroy(gameObject);
    }
}

