using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public bool IsRagdollActive => !_animator.enabled;

    public void SetRagdollActive(bool status) => _animator.enabled = !status;
}
