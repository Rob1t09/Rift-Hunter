using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Kamera")]
    public Transform target;
    public Vector3 offset;
    public float zoom = 1;
    public LayerMask obstacleMask;

    [Header("Zoom")]
    InputAction zoomAction;
    public float zoomStep = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2f;


    //všechno, co je právì skryté
    readonly List<Renderer> hidden = new List<Renderer>();

    private void Awake()
    {
        zoomAction = InputSystem.actions.FindAction("Scroll");
    }

    private void LateUpdate()
    {
        if (target == null) return;

        //zoom
        float scroll = zoomAction.ReadValue<Vector2>().y;
        if (scroll != 0f)
        {
            zoom -= Mathf.Sign(scroll) * zoomStep; //mathf.sign vraci jen -1/1 nebo jen +-
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }



        
        
        //focus = kam komera kouká 
        Vector3 focus = target.position + Vector3.up;
        //kamera
        transform.position = focus + offset * zoom;
        transform.LookAt(focus);

        //vrátit vše, co bylo skryté v minulém snímku
        //projede list schovaných objektù a znovu je zobrazí a pak smaže objekty ze seznamu
        foreach (Renderer r in hidden)
        {
            if (r != null)
            {
                r.enabled = true;
            }
        }
        hidden.Clear();

        //najít všechny pøekážky mezi hráèem a kamerou
        Vector3 toCamera = transform.position - focus;
        //normalized = jen smìr, magnitude = jen délka
        RaycastHit[] hits = Physics.RaycastAll(focus, toCamera.normalized, toCamera.magnitude, obstacleMask);

        //skrýt je a zapamatovat si je
        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r != null)
            {
                r.enabled = false;
                hidden.Add(r);
            }
        }
    }
}
