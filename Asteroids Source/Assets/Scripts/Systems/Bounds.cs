using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    private Collider2D _collider;

    public bool CheckFullyContains(Collider2D resident)
    {
        return _collider.bounds.Contains(resident.bounds.max) && _collider.bounds.Contains(resident.bounds.min);
    }

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }
}
