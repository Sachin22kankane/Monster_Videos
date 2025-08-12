using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
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

    void Awake()
    {
        instance = this;
        
        noise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        defaultAmplitude = noise.AmplitudeGain;
        defaultFrequency = noise.FrequencyGain;
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
}

