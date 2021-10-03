using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Obstacle : MonoBehaviour
{
    [SerializeField] private float _spawnInvulnerableTime = 0f;
    [SerializeField][Tooltip("Used to show invulnerable animation")] private SpriteRenderer _rendererToAnimate;
    [SerializeField] private Collider2D _collider2D;
    private const float _oneFadeTime = 0.5f;

    protected Collider2D Collider => _collider2D;
    protected SpriteRenderer SpriteRenderer => _rendererToAnimate;

    protected void DoInvulnerable()
    {
        if (_spawnInvulnerableTime != 0f)
        {
            var invulnerableRoutine = new CoroutineObject(this, InvulnerableRoutine);
            var fadeRoutine = new CoroutineObject(this, FadeSequenceRoutine);
            invulnerableRoutine.Finished += fadeRoutine.Stop;
            fadeRoutine.Start();
            invulnerableRoutine.Start();
        }
    }

    protected abstract void OnHitableEnter(Collider2D entered, IHitable hitable);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IHitable hitable))
        {
            OnHitableEnter(other, hitable);
        }
    }

    private IEnumerator InvulnerableRoutine()
    {
        _collider2D.enabled = false;
        yield return new WaitForSeconds(_spawnInvulnerableTime);
        _collider2D.enabled = true;
    }

    private IEnumerator FadeSequenceRoutine()
    {
        while (true)
        {
            Animations.PlayFadeAnimation(this, _rendererToAnimate, 0f, _oneFadeTime);
            yield return new WaitForSeconds(_oneFadeTime);
            Animations.PlayFadeAnimation(this, _rendererToAnimate, 1f, _oneFadeTime);
            yield return new WaitForSeconds(_oneFadeTime);
        }
    }

    private void OnValidate()
    {
        if (!GetComponent<Collider2D>().isTrigger) Debug.LogWarning($"{gameObject.name} collider isn't trigger!");
    }
}