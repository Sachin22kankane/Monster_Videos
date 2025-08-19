using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    
    private void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
