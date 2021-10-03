using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerBody), typeof(PlayerShooter))]
public class PlayerMovement : Singleton<PlayerMovement>, IFlyDirection
{
    [SerializeField] [Range(0.5f, 1.25f)] private float _acceleration = 1f;
    [SerializeField] [Range(2f, 5f)] private float _maxSpeed = 5f;
    [SerializeField] [Range(1f, 2f)] private float _rotationSpeed = 1f;
    private PlayerBody _playerBody;
    private PlayerShooter _shooter;
    private Rigidbody2D _rigidbody;
    private bool _canMove;
    private CoroutineObject _moveRoutine;
    private CoroutineObject _rotateRoutine;
    private CoroutineObject _updDirRoutine;
    private Vector3 _prevPosition;

    public Vector2 FlyDirection { get; private set; }
    public event Action OnMoveStart;
    public event Action OnMoveStop;

    public void StopAllMovement(bool restartSpeed)
    {
        if (!_canMove) return;
        _canMove = false;

        if (_moveRoutine.IsProcessing) StopMoving();
        if (_rotateRoutine.IsProcessing) _rotateRoutine.Stop();
        if (restartSpeed) _rigidbody.velocity = Vector2.zero;
        InputEvents.OnShootKeyDown -= Shoot;
        InputEvents.OnMoveKeyDown -= StartMoving;
        InputEvents.OnMoveKeyUp -= StopMoving;
    }

    public void StartMovement()
    {
        if (_canMove) return;
        _canMove = true;

        _rotateRoutine.Start();
        InputEvents.OnShootKeyDown += Shoot;
        InputEvents.OnMoveKeyDown += StartMoving;
        InputEvents.OnMoveKeyUp += StopMoving;
    }

    protected override void Awake()
    {
        base.Awake();

        _rotateRoutine = new CoroutineObject(this, RotateRoutine);
        _moveRoutine = new CoroutineObject(this, MoveRoutine);
        _updDirRoutine = new CoroutineObject(this, UpdateFlyDirection);
    }

    private void StartMoving()
    {
        _moveRoutine.Start();
        OnMoveStart?.Invoke();
    }
    private void StopMoving()
    {
        OnMoveStop?.Invoke();
        _moveRoutine.Stop();
    }
    private void OnPause() => StopAllMovement(false);
    private void OnContinue() => StartMovement();
    private void OnGameStart() => _playerBody.MakeInvulnerable();

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerBody = GetComponent<PlayerBody>();
        _shooter = GetComponent<PlayerShooter>();
        _prevPosition = transform.position;

        _updDirRoutine.Start();

        GameManager.Instance.OnGameStart += OnGameStart;
        GameManager.Instance.OnContinue += OnContinue;
        GameManager.Instance.OnPause += OnPause;
    }

    private IEnumerator UpdateFlyDirection()
    {
        while (true)
        {
            Vector3 currentPos = transform.position;
            FlyDirection = currentPos - _prevPosition;
            _prevPosition = currentPos;

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator RotateRoutine()
    {
        print(InputReciever.Instance != null);
        while (true)
        {
            if (InputReciever.Instance.InputEvents.GetFaceDirection(out Vector2 direction)) Rotate(direction);
            yield return null;
        }
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            _rigidbody.AddForce(transform.up * _acceleration);
            if (_rigidbody.velocity.magnitude > _maxSpeed)
            {
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void Rotate(Vector2 target)
    {
        float rotation = Utils.GetZRotation(target);
        float singleStep = _rotationSpeed * Time.deltaTime;
        Quaternion q = Quaternion.AngleAxis(rotation, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, q, singleStep);
    }

    private void Shoot()
    {
        _shooter.Shoot();
    }

    private void OnDestroy()
    {
        StopAllMovement(false);
        _updDirRoutine.Stop();
        GameManager.Instance.OnGameStart -= OnGameStart;
        GameManager.Instance.OnPause -= OnPause;
        GameManager.Instance.OnContinue -= OnContinue;
    }
}
