using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(RagdollController))]
public class Enemy : MonoBehaviour
{
    private RagdollController _ragdollController;

    public bool IsRagdollActivated => _ragdollController.IsRagdollActive;
    
    public void SetRagdollActive(bool status) => _ragdollController.SetRagdollActive(status);

    private void Start()
    {
        _ragdollController = GetComponent<RagdollController>();
    }
}

