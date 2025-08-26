using System;
using DG.Tweening;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private GameObject mesh;
    private Collider col;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float impactRange;

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    public void Explode()
    {
        col.enabled = false;
        explosionEffect.Play();
        mesh.SetActive(false);
        Collider[] enemies = Physics.OverlapSphere(transform.position, impactRange, enemyLayerMask);
        foreach (Collider enemy in enemies)
        {
            enemy.gameObject.GetComponent<Enemy>().Damage();
        }
        CameraShake.instance.Shake(2,1.5f,0.5f);
    }
}
