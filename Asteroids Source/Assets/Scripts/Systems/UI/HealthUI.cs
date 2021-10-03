using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Order is necessary")]
    private Image[] _health;
    private int _lastHealthIndex;

    public void SetMaxHealth(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetActiveStatus(GetRightImage(i), true);
        }
        for (int i = count; i < _health.Length; i++)
        {
            SetActiveStatus(GetRightImage(i), false);
        }
        _lastHealthIndex = count - 1;
    }

    public void RemoveHealthByValue(int healthValue)
    {
        RemoveHealthByCount(_lastHealthIndex - (healthValue - 1));
    }

    public void RemoveHealthByCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetActiveStatus(GetRightImage(_lastHealthIndex), false);
            _lastHealthIndex--;
        }
    }

    public void AddHealthByCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetActiveStatus(GetRightImage(_lastHealthIndex), true);
            _lastHealthIndex++;
        }
    }

    private Image GetRightImage(int index) => _health[index];
    private void SetActiveStatus(Image image, bool isActive) => image.gameObject.SetActive(isActive); 
}
