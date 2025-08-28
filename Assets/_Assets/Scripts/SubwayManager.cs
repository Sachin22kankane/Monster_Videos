using System;
using System.Collections;
using UnityEngine;

public class SubwayManager : MonoBehaviour
{
    public static SubwayManager instance;

    [SerializeField] private Barrel[] barrels;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Explode();
            CameraShake.instance.SwitchCamera(4);
        }
    }

    private void Start()
    {
        StartCoroutine(StartChasingScene());
    }

    IEnumerator StartChasingScene()
    {
        yield return new WaitForSeconds(0.15f);
        CameraShake.instance.SwitchCamera(1);
    }

    void Explode()
    {
        StartCoroutine(ExplodeOneByOne());
    }

    public IEnumerator ExplodeOneByOne()
    {
        for (int i = 0; i < barrels.Length; i++)
        {
            barrels[i].Explode();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
