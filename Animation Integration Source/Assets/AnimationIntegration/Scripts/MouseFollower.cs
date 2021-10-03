using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Transform _character;
    [SerializeField] private float _minMagnitude = 5f;

    private void Update()
    {
        Vector3 mousePoint = InputController.Instance.GetMousePoint(transform);
        Vector3 direction = mousePoint - _character.position;
        float magnitude = direction.magnitude;
        if (magnitude < _minMagnitude)
        {
            float yDiff = Mathf.Abs(transform.position.y - _character.position.y);
            float fraction = Mathf.Sqrt(_minMagnitude * _minMagnitude - yDiff * yDiff) / Mathf.Sqrt(magnitude * magnitude - yDiff * yDiff);
            mousePoint = new Vector3(_character.position.x + (mousePoint.x - _character.position.x) * fraction,
                mousePoint.y, _character.position.z + (mousePoint.z - _character.position.z) * fraction);
        }
        transform.position = mousePoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}

