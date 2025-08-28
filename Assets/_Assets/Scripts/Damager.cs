using System;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Damage(damage);
            CameraShake.instance.Shake(0.2f * damage/50f,1.5f,1.25f);
        }
    }
}
