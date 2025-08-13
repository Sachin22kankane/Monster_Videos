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
        foreach (Transform fence in fences)
        {
            fence.localScale = Vector3.zero;
            fence.DOScale(Vector3.one, 0.75f).SetDelay(0.25f);
        }
        CameraShake.instance.ChangeFov(70);
        
    }
    
}
