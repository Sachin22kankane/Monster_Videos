using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{
    [SerializeField] Transform rightOption, leftOption;
    [SerializeField] private Transform[] fences;
    [SerializeField] private GameObject fenceParent;
    
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
        rightOption.gameObject.SetActive(false);
        leftOption.gameObject.SetActive(false);
        Time.timeScale = 1f;
        fenceParent.SetActive(true);
        foreach (Transform fence in fences)
        {
            fence.localScale = Vector3.zero;
            fence.DOScale(Vector3.one, 0.75f).SetDelay(0.25f);
        }
        if (virtualCamera != null)
        {
            // Smoothly change the FOV
            DOTween.To(
                () => virtualCamera.Lens.FieldOfView,     // Getter
                x => virtualCamera.Lens.FieldOfView = x, // Setter
                70,                                 // Target value
                1f                                   // Time
            );
        }
    }
    
}
