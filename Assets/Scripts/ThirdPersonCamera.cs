using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float zoom = 1;


    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + (offset*zoom);
            transform.LookAt(target.position+Vector3.up);
        }
    }
}
