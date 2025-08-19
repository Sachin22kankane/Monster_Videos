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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            rightOption.DOScale(Vector3.one, 0.15f).OnComplete(() => { Time.timeScale = 0.1f; });
            leftOption.DOScale(Vector3.one, 0.15f);
        }
    }

    public CinemachineCamera virtualCamera;
    public void Build()
    {
        canvas.transform.DOScale(Vector3.zero,0.25f);
        canvas.SetActive(false);
        rightOption.gameObject.SetActive(false);
        leftOption.gameObject.SetActive(false);
        Time.timeScale = 1f;
        fenceParent.SetActive(true);
        StartCoroutine(SpawnFence());
        CameraShake.instance.ChangeFov(70,0.25f);
    }
    
    public void GunUpgrade()
    {
        canvas.transform.DOScale(Vector3.zero,0.25f);
        canvas.SetActive(false);
        rightOption.gameObject.SetActive(false);
        leftOption.gameObject.SetActive(false);
        Time.timeScale = 1f;
        fenceParent.SetActive(true);
        PlayerController.instance.GetComponent<Shooting>().SelectGunSet(1);
        //CameraShake.instance.ChangeFov(70,0.25f);
    }

    IEnumerator SpawnFence()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < fences.Length; i++)
        {
            fences[i].localScale = Vector3.zero;
            fences[i].gameObject.SetActive(true);
            fences[i].DOScale(Vector3.one, 0.15f);
            particle[i].Play();
            yield return new WaitForSeconds(0.15f);
        }
    }
    
}
