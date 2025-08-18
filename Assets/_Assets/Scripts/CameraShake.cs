using System;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public Camera cam;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float shakeAmplitude = 1.5f;
    [SerializeField] private float shakeFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float defaultAmplitude;
    private float defaultFrequency;
    private Coroutine shakeCoroutine;
    [SerializeField] RectTransform canvasRect;
    private CinemachineFollow followComponent;

    void Awake()
    {
        instance = this;
        
        noise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        defaultAmplitude = noise.AmplitudeGain;
        defaultFrequency = noise.FrequencyGain;
        followComponent = virtualCamera.GetComponent<CinemachineFollow>();
    }

    private void Start()
    {
        Vector3 currentRotation = virtualCamera.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(60, currentRotation.y, currentRotation.z);
        ChangeFollowOffset(new Vector3(0,13,-10));
        virtualCamera.transform
            .DORotate(targetRotation, 2)
            .SetEase(Ease.InOutSine).SetDelay(2);
        
    }
    
    public void ChangeFollowOffset(Vector3 newOffset)
    {
        DOTween.To(
            () => followComponent.FollowOffset,
            x => followComponent.FollowOffset = x,
            newOffset,
            2
        ).SetEase(Ease.InOutSine).SetDelay(2);
    }

    public void Shake(float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration));
    }

    private IEnumerator ShakeRoutine(float duration)
    {
        noise.AmplitudeGain = shakeAmplitude;
        noise.FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = defaultAmplitude;
        noise.FrequencyGain = defaultFrequency;
        shakeCoroutine = null;
    }

    public Vector2 WorldToCanvasPoint(Vector3 worldPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            cam.WorldToScreenPoint(worldPos),
            cam,
            out pos
        );
        return pos;
    }

    public void ChangeFov(float fov, float duration = 1f)
    {
        if (virtualCamera != null)
        {
            // Smoothly change the FOV
            DOTween.To(
                () => virtualCamera.Lens.FieldOfView,     // Getter
                x => virtualCamera.Lens.FieldOfView = x, // Setter
                fov,                                 // Target value
                duration                                   // Time
            );
        }
    }
}

