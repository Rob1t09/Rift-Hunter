using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float zoom = 1;
    public LayerMask obstacleMask;


    //všechno, co je právì skryté
    readonly List<Renderer> hidden = new List<Renderer>();


    private void LateUpdate()
    {
        if (target == null) return;

        //kamera
        transform.position = target.position + offset * zoom;
        //focus = kam komera kouká 
        Vector3 focus = target.position + Vector3.up;
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
