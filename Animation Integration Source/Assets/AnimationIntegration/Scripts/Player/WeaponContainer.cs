using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponType
{
    Gun, Sword
}

[System.Serializable]
public class WeaponTypeMesh
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private Mesh _mesh;

    public WeaponType Type => _type;
    public Mesh Mesh => _mesh;
}

public class WeaponContainer : MonoBehaviour
{
    [SerializeField] private MeshFilter _weaponMeshFilter;
    [SerializeField] private List<WeaponTypeMesh> _weaponTypeMeshes = new List<WeaponTypeMesh>();

    public void ChangeWeapon(WeaponType weaponType)
    {
        if (!_weaponTypeMeshes.Any(pair => pair.Type == weaponType))
        {
            Debug.LogError($"Unknown {nameof(WeaponType)} {weaponType} in {GetType().Name}");
            return;
        }
       
        Mesh weaponMesh = _weaponTypeMeshes.Find(pair => pair.Type == weaponType).Mesh;
        _weaponMeshFilter.mesh = weaponMesh;
    }
}
