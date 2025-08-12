using System;
using UnityEngine;

public class BloodParticle : PoolableObject
{
    [SerializeField] ParticleSystem particle;

    private void Awake()
    {
        var main = particle.main;
        main.stopAction = ParticleSystemStopAction.Disable;
    }

    public void Play(Vector3 dir)
    {
        gameObject.SetActive(true);
        particle.Play();
        transform.forward = dir;
    }

    private void OnDisable()
    {
        pool?.Release(this);
        gameObject.SetActive(false);
    }
}
