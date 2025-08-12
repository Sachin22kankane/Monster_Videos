using System;
using DG.Tweening;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] Collider collider;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Boid boid))
        {
            if (Vector3.Distance(PlayerController.instance.transform.position, boid.transform.position) < 5)
            {
                boid.transform.DOLookAt(transform.position, 0.2f,AxisConstraint.Y);
            }
        }
    }
    
}
