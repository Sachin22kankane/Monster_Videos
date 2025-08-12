using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    void LateUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * 10f);
    }
}
