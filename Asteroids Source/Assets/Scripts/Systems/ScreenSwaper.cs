using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ScreenSwaper : Singleton<ScreenSwaper>
{
    private Camera _camera;
    private Vector3 _playerPoint1;
    private Vector3 _playerPoint2;

    public Vector2 ScreenWorldSize => GetScreenWorldSize();

    public bool IsInScreen(Transform transform, out Vector3 viewportPosition)
    {
        viewportPosition = _camera.WorldToViewportPoint(transform.position);
        return !(viewportPosition.x > 1 || viewportPosition.x < 0 || viewportPosition.y > 1 || viewportPosition.y < 0);
    }

    public bool TrySwap(Transform swapTransform, IFlyDirection flyDirection)
    {
        if (IsInScreen(swapTransform, out Vector3 viewportPosition)) return false;

        float angle = Vector2.SignedAngle(Vector2.right, flyDirection.FlyDirection);
        angle = Utils.Clamp0360(angle);
        int quarter = (int)(angle / 90f);
        if (angle % 90f > 0) quarter++;

        Vector2 entityPosition = swapTransform.position;
        bool outsideX = viewportPosition.x > 1f || viewportPosition.x < 0f;
        bool outsideY = viewportPosition.y > 1f || viewportPosition.y < 0f;

        if (outsideX && outsideY)
        {
            if (CheckCornerDirections((1, 1), (2, 2), (3, 3), (4, 4)))
            {
                entityPosition.x = -entityPosition.x;
                entityPosition.y = -entityPosition.y;
            }
            else if (CheckCornerDirections((1, 4), (2, 3), (3, 2), (4, 1)))
            {
                entityPosition.x = -entityPosition.x;
            }
            else if (CheckCornerDirections((1, 2), (2, 1), (3, 4), (4, 3)))
            {
                entityPosition.y = -entityPosition.y;
            }
        }
        else if (outsideX)
        {
            entityPosition.x = -entityPosition.x;
        }
        else
        {
            entityPosition.y = -entityPosition.y;
        }

        swapTransform.position = entityPosition;
        return true;

        bool CheckCornerDirections(params (int corner, int quarter)[] cornerDirections)
        {
            var (corner, _) = cornerDirections.Single(x => x.quarter == quarter);
            switch (corner)
            {
                case 1:
                    if (viewportPosition.x > 1f && viewportPosition.y > 1f)
                        return true;
                    break;
                case 2:
                    if (viewportPosition.x < 0f && viewportPosition.y > 1f)
                        return true;
                    break;
                case 3:
                    if (viewportPosition.x < 0f && viewportPosition.y < 0f)
                        return true;
                    break;
                case 4:
                    if (viewportPosition.x > 0f && viewportPosition.y < 0f)
                        return true;
                    break;

                default:
                    throw new ArgumentException("Corner must be from 1 to 4");
            }

            return false;
        }
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private Vector2 GetScreenWorldSize()
    {
        if (!_camera.orthographic) Debug.LogWarning($"Screen size in {GetType().Name} may be wrong");
        float height = _camera.orthographicSize * 2f;
        float width = height * _camera.aspect;
        return new Vector2(width, height);
    }
}

