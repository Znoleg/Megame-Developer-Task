using System.Collections;
using UnityEngine;

public class PlayerShooter : Shooter
{
    [SerializeField] private float _cooldown = 0.33f;
    [SerializeField] private Transform _shootPoint;
    private CoroutineObject _cooldownRoutine;

    public void Shoot()
    {
        if (_cooldownRoutine.IsProcessing) return;
        Shoot(_shootPoint.position, _shootPoint.up);
        _cooldownRoutine.Start();
    }

    private void Awake() => _cooldownRoutine = new CoroutineObject(this, WaitForCooldown);

    private IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
    }
}
