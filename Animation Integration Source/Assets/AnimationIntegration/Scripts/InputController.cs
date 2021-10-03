using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{
    public Vector2 Movement { get; private set; }
    public event Action OnSpacePressed;

    private Vector3 _prevMovement;

    private void Update()
    {
        Movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space)) OnSpacePressed?.Invoke();
    }

    public Vector3 GetMousePoint()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        plane.Raycast(ray, out float dist);
        Debug.DrawRay(ray.origin, ray.direction * dist);
        return ray.GetPoint(dist);
    }

    public Vector3 GetMousePoint(Transform transformToIgrnoreY)
    {
        Vector3 mousePoint = GetMousePoint();
        mousePoint.y = transformToIgrnoreY.position.y;
        return mousePoint;
    }
}
