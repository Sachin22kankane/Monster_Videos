using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class UpgradeTrigger : MonoBehaviour
{
    [SerializeField] Transform rightOption, leftOption;
    [SerializeField] private Transform[] fences;
    [SerializeField] private GameObject fenceParent;
    [SerializeField] private GameObject canvas;
    [SerializeField] private ParticleSystem[] particle;
    [SerializeField] private ParticleSystem novaEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            rightOption.DOScale(Vector3.one, 0.15f).OnComplete(() => {  });
            leftOption.DOScale(Vector3.one, 0.15f);
        }
    }

    public CinemachineCamera virtualCamera;
    public void Build()
    {
        canvas.transform.DOScale(Vector3.zero,0.25f);
        canvas.SetActive(false);
        novaEffect.gameObject.SetActive(true);
        rightOption.gameObject.SetActive(false);
        leftOption.gameObject.SetActive(false);
        fenceParent.SetActive(true);
        StartCoroutine(SpawnFence());
        CameraShake.instance.ChangeFov(70,0.25f);
    }
    
    public void GunUpgrade(int gunindex)
    {
        canvas.transform.DOScale(Vector3.zero,0.25f);
        canvas.SetActive(false);
        rightOption.gameObject.SetActive(false);
        novaEffect.gameObject.SetActive(true);
        leftOption.gameObject.SetActive(false);
        fenceParent.SetActive(true);
        PlayerController.instance.GetComponent<Shooting>().SelectGunSet(gunindex);
        //CameraShake.instance.ChangeFov(70,0.25f);
    }

    IEnumerator SpawnFence()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < fences.Length; i++)
        {
            fences[i].localScale = Vector3.zero;
            fences[i].gameObject.SetActive(true);
            fences[i].DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBounce);
            particle[i].Play();
            yield return new WaitForSeconds(0.15f);
        }
    }
    
}
