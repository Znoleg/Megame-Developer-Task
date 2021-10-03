using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Pool<T> where T : MonoBehaviour
{
    [SerializeField][Tooltip("Quantity of each prefab")] private int _createQuantity;
    [SerializeField] private int _expandCount = 10;
    [SerializeField] private List<T> _prefabs;
    private readonly List<T> _objects = new List<T>();
    private int _currentAvailable = 0;
    private Transform _owner;

    public void CreateInstances(Transform owner)
    {
        _owner = owner;
        ExpandPool(_createQuantity);
    }

    public void MarkReturned()
    {
        _currentAvailable++;
    }

    public T PopAndPush()
    {
        T element = PopElement();
        PushElement(element);
        return element;
    }

    public T PopElement()
    {
        T toReturn = _objects.First();
        _objects.Remove(toReturn);
        
        _currentAvailable--;
        if (_currentAvailable == 0) ExpandPool(_expandCount);
        return toReturn;
    }

    public bool TryGetElement(Func<T, bool> predicate, out T component)
    {
        component = _objects.FirstOrDefault(predicate);
        if (component == default)
        {
            return component = null;
        }
        _objects.Remove(component);
        return component;
    }

    public void PushElement(T element)
    {
        _objects.Add(element);
    }

    private void ExpandPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            foreach (T prefab in _prefabs)
            {
                T instance = MonoBehaviour.Instantiate(prefab, _owner);
                instance.gameObject.SetActive(false);
                _objects.Add(instance);
            }
        }
        
        _currentAvailable += count;
    }
}

