using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    #region Fields
    [Header("Move settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField][Range(0f, 360f)] private float _rotationSpeed;
    [Header("Animation settings")]
    [SerializeField][Range(2f, 7f)] private float _finishMoveSpeed = 4f;
    [SerializeField][Range(1f, 3f)] private float _finishingBackDistance = 1.5f;
    [SerializeField] private bool _saveLegsRotationOnStop = true;
    [SerializeField] private float _moveAnimationOffset = 90f;
    [Header("References")]
    [SerializeField] private LookAt _upperBodyLooker;
    [SerializeField] private Transform _upperBody;
    [SerializeField] private Transform _root;
    private WeaponContainer _weaponContainer;
    private AnimatorController _animatorController;
    private Camera _camera;
    private Vector3 _targetBodyRotatation = Vector3.forward;
    private Vector3 _rootForward;
    private Vector3 _previousMovement;
    private bool _canMove = true;
    #endregion

    public Vector3 MoveVector { get; private set; }
    public bool IsMoving => MoveVector != Vector3.zero;

    public int FacingSign
    {
        get
        {
            Vector3 faceForawrd = _upperBody.up;
            faceForawrd.y = 0f;
            
            bool isRight = Utils.GetVectorAngle(_rootForward, faceForawrd, true) > 0;
            Debug.DrawRay(transform.position, faceForawrd * 10, Color.green);
            Debug.DrawRay(transform.position, _rootForward * 10, Color.red);
            if (isRight) Debug.Log("Right");
            else Debug.Log("Left");
            return isRight ? 1 : -1;
        }
    }

    public void FinishEnemy(Enemy enemy)
    {
        if (!_canMove) return;

        _canMove = false;
        _root.transform.eulerAngles = Vector3.zero;
        _upperBodyLooker.transform.eulerAngles = Vector3.zero;
        _upperBodyLooker.SetEnabled(false);
        _weaponContainer.ChangeWeapon(WeaponType.Sword);

        Vector3 endPosition = enemy.transform.position - (enemy.transform.forward * _finishingBackDistance);
        Vector3 playerRotation = -(endPosition - enemy.transform.position);

        var moveRoutine = new CoroutineObject(this, () => MoveToSmoothly(endPosition, _finishMoveSpeed));
        moveRoutine.Finished += OnMoveEnd;
        moveRoutine.Start();

        CoroutineObjectBase animationRoutine;

        Debug.DrawRay(enemy.transform.position, -enemy.transform.forward, Color.blue, 3f);
        Debug.DrawRay(enemy.transform.position, -playerRotation, Color.yellow, 3f);
        
        void OnMoveEnd()
        {
            moveRoutine.Finished -= OnMoveEnd;
            transform.rotation = Quaternion.LookRotation(playerRotation, Vector3.up);

            animationRoutine = _animatorController.PlayHitAnimation(out float animationLength);
            animationRoutine.Finished += OnAnimationEnd;

            var waitForKillRoutine = Utils.CreateWaitCoroutine(this, animationLength / 3);
            waitForKillRoutine.Finished += KillEnemy;
            waitForKillRoutine.Start();

            void KillEnemy()
            {
                waitForKillRoutine.Finished -= KillEnemy;
                EnemiesManager.Instance.RespawnEnemy(enemy);
            }

            void OnAnimationEnd()
            {
                animationRoutine.Finished -= OnAnimationEnd;
                transform.eulerAngles = Vector3.zero;
                _weaponContainer.ChangeWeapon(WeaponType.Gun);
                _upperBodyLooker.SetEnabled(true);
                _canMove = true;
            }
        }
    }

    private void Start()
    {
        _camera = Camera.main;
        _weaponContainer = GetComponent<WeaponContainer>();
        _animatorController = GetComponentInChildren<AnimatorController>();

        _rootForward = _root.forward;
    }

    private void Update()
    {
        int sign = FacingSign;
        if (_canMove)
        {
            Vector3 input = InputController.Instance.Movement;
            MoveVector = Move(input);

            if (MoveVector != Vector3.zero)
            {
                GetRotation(MoveVector);
                RotateRoot();
            }
            else if (_saveLegsRotationOnStop && _previousMovement != Vector3.zero)
            {
                const float rotationTime = 0.26f;
                new CoroutineObject(this, () => SetIdleRootSmoothly(rotationTime)).Start();
                //SetIdleRootRotation();
            }

            _previousMovement = MoveVector;
        }
        
        _animatorController.SetSpeed(Mathf.Max(Mathf.Abs(MoveVector.x), Mathf.Abs(MoveVector.y)));
    }

    private void SetIdleRootRotation()
    {
        _root.transform.localEulerAngles += new Vector3(0f, _moveAnimationOffset, 0f);
    }

    private IEnumerator SetIdleRootSmoothly(float time)
    {
        float step = _moveAnimationOffset / time * Time.fixedDeltaTime / 1f;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            _root.transform.localEulerAngles += new Vector3(0f, step, 0f);
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator MoveToSmoothly(Vector3 endPosition, float speed)
    {
        MoveVector = new Vector3(1f, 1f); // not very cool
        while (transform.position != endPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void GetRotation(Vector3 moveVector)
    {
        Vector3 direction = moveVector;
        Vector3 localForward = transform.up;
        localForward.y = 0f;
        Debug.DrawRay(transform.position, direction * 10, Color.red);

        float step = _rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
        _targetBodyRotatation = Vector3.RotateTowards(_targetBodyRotatation, direction, step, 10f);
        Debug.DrawRay(transform.position, _targetBodyRotatation * 10, Color.black);
    }

    private void RotateRoot()
    {
        _rootForward = Quaternion.Euler(0f, _moveAnimationOffset, 0f) * new Vector3(_root.forward.x, 0f, _root.forward.z);

        float angle = Utils.GetVectorAngle(_targetBodyRotatation, _rootForward, true);
        _root.Rotate(new Vector3(0f, -angle * Time.deltaTime * 100f, 0f), Space.World);
    }

    private Vector3 Move(Vector2 input)
    {
        Vector3 moveVector = Quaternion.Euler(0f, _camera.gameObject.transform.eulerAngles.y, 0f) * 
            (Vector3.right * input.x + Vector3.forward * input.y).normalized;
        Vector3 targetPostition = transform.position + moveVector * _speed * Time.deltaTime;
        transform.position = targetPostition;
        return moveVector;
    }
}
