using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform _rotatingParent;
    //[SerializeField][Range(0f, 360f)] private float _maxRotationAngle;
    [SerializeField] private float _movingOffset = 45f;
    [SerializeField] private Transform _looker;
    [SerializeField] private Transform _lookAt;
    private bool _isEnabled = true;

    public void SetEnabled(bool status) => _isEnabled = status;

    private void LateUpdate()
    {
        if (!_isEnabled) return;
        LocalRotate();
    }

    private void LocalRotate()
    {
        Vector3 direction = _lookAt.position - _looker.position;
        Vector3 forward = transform.forward;

        Vector3 localRotation = Utils.GetLocalEulerRotation(direction, forward, ignoreY: true);
        //localRotation.x = Mathf.Clamp(localRotation.x, -_maxRotationAngle / 2, _maxRotationAngle / 2);
        localRotation.x += _rotatingParent.localEulerAngles.y;
        if (PlayerController.Instance.IsMoving) localRotation.x += _movingOffset;
        
        _looker.localEulerAngles = localRotation;
    }

    private void GlobalRotate()
    {
        Vector3 direction = _lookAt.position - _looker.position;
        Vector3 forward = Vector3.forward;

        _looker.eulerAngles = Utils.GetGlobalEulerRotation(direction, forward, ignoreY: true);
    }
}
