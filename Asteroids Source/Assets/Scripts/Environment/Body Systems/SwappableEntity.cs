using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(IFlyDirection))]
public class SwappableEntity : MonoBehaviour
{
    private IFlyDirection _flyDirection;

    private void Start()
    {
        _flyDirection = GetComponent<IFlyDirection>();
        if (_flyDirection == null) Debug.LogError($"Must assign {nameof(IFlyDirection)} on {gameObject.name}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Bounds _))
        {
            ScreenSwaper.Instance.TrySwap(transform, _flyDirection);
        }
    }
}

