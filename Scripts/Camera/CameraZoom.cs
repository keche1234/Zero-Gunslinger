using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    [Range(0.01f, 7)]
    [SerializeField] private float zoomSpeed;
    [Range(0.01f, 31)]
    [SerializeField] private float maxZoomIn;
    [Range(0.01f, 31)]
    [SerializeField] private float maxZoomOut;
    private float start;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Returns true if the zoom did not need to be corrected
     */
    public bool Zoom(float f)
    {
        transform.position += transform.forward * f * zoomSpeed;
        if (transform.position.z - start > maxZoomIn)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, start + maxZoomIn);
            return false;
        }
        else if (start - transform.position.z > maxZoomOut)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, start - maxZoomOut);
            return false;
        }
        return true;
    }
}
