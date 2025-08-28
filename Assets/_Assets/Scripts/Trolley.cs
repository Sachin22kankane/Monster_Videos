using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform player;
    [SerializeField]  private Vector3 playerOffset;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private ParticleSystem[] explosions;
    [SerializeField] private Transform targetPos;

    private void Start()
    {
        player = PlayerController.instance.transform;
        StartMoving();
    }

    void StartMoving()
    {
        float distance = Vector3.Distance(transform.position, targetPosition.position);
        transform.DOMove(targetPosition.position, distance/speed).SetEase(Ease.Linear).OnUpdate(() =>
        {
            player.position = transform.position + playerOffset;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damage(1000);
            if (enemy.isBoss)
            {
                CameraShake.instance.Shake(1,2f,2f,1);
                Vector3 spawnPos = transform.position;
                smokeEffect.SetActive(true);
                var explosion = ObjectPooling.Instance.Spawn<BloodParticle>(PoolType.missileExplosion,spawnPos);
                explosion.Play(Vector3.zero);
            }
        }

        if (other.CompareTag("barrier"))
        {
            other.tag = "Untagged";
            StartCoroutine(BlastSequnce());
            CameraShake.instance.Shake(1,4f,4f,1);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                PlayerController.instance.JumpOutfromTrolley(targetPos.position);
                CameraShake.instance.SwitchCamera(2);
            });
        }
    }

    IEnumerator BlastSequnce()
    {
        for (int i = 0; i < explosions.Length; i++)
        {
            explosions[i].Play();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
}
