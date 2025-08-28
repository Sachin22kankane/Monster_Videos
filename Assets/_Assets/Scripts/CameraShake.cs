using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public Camera cam;
    [SerializeField] private CinemachineCamera virtualCamera;
   // [SerializeField] private float shakeAmplitude = 1.5f;
   // [SerializeField] private float shakeFrequency = 2f;

    //private CinemachineBasicMultiChannelPerlin noise;
    //private float defaultAmplitude;
    //private float defaultFrequency;
    private Coroutine shakeCoroutine;
    [SerializeField] RectTransform canvasRect;
    private CinemachineFollow followComponent;

    [SerializeField] private CinemachineCamera[] allCameras;
    [SerializeField] private List<CinemachineBasicMultiChannelPerlin> allPerlins;

    [Serializable]
    public class CameraShakeSet
    {
        public float defaultAmplitude;
        public float defaultFrequency;
        public CinemachineBasicMultiChannelPerlin noise;
    }
    
    private List<CameraShakeSet> shakeSets = new List<CameraShakeSet>();
    
    public bool hook1;

    void Awake()
    {
        instance = this;
        for (int i = 0; i < allCameras.Length; i++)
        {
            var noise = allCameras[i].GetCinemachineComponent(
                CinemachineCore.Stage.Noise
            ) as CinemachineBasicMultiChannelPerlin;

// Safety check in case noise is null
            if (noise != null)
            {
                CameraShakeSet shakeSet = new CameraShakeSet()
                {
                    noise = noise,
                    defaultAmplitude = noise.AmplitudeGain,
                    defaultFrequency = noise.FrequencyGain
                };

                // Add to your list
                shakeSets.Add(shakeSet);
            }
        }
        //noise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        //defaultAmplitude = noise.AmplitudeGain;
        //defaultFrequency = noise.FrequencyGain;
        followComponent = virtualCamera.GetComponent<CinemachineFollow>();
    }

    private void Start()
    {
        if (hook1 == true)
        {
            Vector3 currentRotation = virtualCamera.transform.eulerAngles;
            Vector3 targetRotation = new Vector3(60, currentRotation.y, currentRotation.z);
            ChangeFollowOffset(new Vector3(0,13,-10));
            virtualCamera.transform
                .DORotate(targetRotation, 2)
                .SetEase(Ease.InOutSine).SetDelay(2);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchFov();
        }
    }

    public void SwitchCamera(int index)
    {
        foreach (CinemachineCamera camera in allCameras)
        {
            camera.Priority = 0;
        }
        allCameras[index].Priority = 10;
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

    private int previousShakeIndex;
    public void Shake(float duration,float shakeAmplitude,float shakeFrequency,int shakeSetIndex = 0)
    {
        if (shakeSetIndex < previousShakeIndex)
        {
            return;
        }
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        previousShakeIndex = shakeSetIndex;
        print("shake camera " + shakeSetIndex);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration,shakeAmplitude, shakeFrequency,shakeSetIndex));
    }

    private IEnumerator ShakeRoutine(float duration,float shakeAmplitude,float shakeFrequency,int shakeSetIndex = 0)
    {
        shakeSets[shakeSetIndex].noise.AmplitudeGain = shakeAmplitude;
        shakeSets[shakeSetIndex].noise.FrequencyGain = shakeFrequency;
        print("Set shake values " + shakeSetIndex);

        yield return new WaitForSeconds(duration);

        print(shakeSetIndex + " Reset Shake Values");
        print(shakeSets[shakeSetIndex].defaultAmplitude + " " +  shakeSets[shakeSetIndex].defaultFrequency );
        shakeSets[shakeSetIndex].noise.AmplitudeGain = shakeSets[shakeSetIndex].defaultAmplitude;
        shakeSets[shakeSetIndex].noise.FrequencyGain = shakeSets[shakeSetIndex].defaultFrequency;
        previousShakeIndex = 0;
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

    private bool zoomin = true;
    void SwitchFov()
    {
        zoomin = !zoomin;
        if (zoomin)
        {
            ChangeFov(45,1f);
        }
        else
        {
            ChangeFov(70,1f);
        }
       
    }
}

