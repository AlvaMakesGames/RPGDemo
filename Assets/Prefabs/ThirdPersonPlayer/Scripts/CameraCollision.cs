using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    private float minDist = 1f;
    private float maxDist = 3f;
    private float zoomMin = 3f;
    private float zoomMax = 8f;
    private float zoom;
    [SerializeField] private float zoomSens = 2;
    private float smoothing = 10f;
    private Vector3 dollyDirection;
    private float distance;

    void Start()
    {
        dollyDirection = transform.localPosition.normalized;
        zoom = zoomMin;
    }
    
    void Update()
    {
        Vector3 desiredCameraPosition = transform.parent.TransformPoint(dollyDirection * maxDist);
        RaycastHit hit;

        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSens;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

        maxDist = zoom;

        if (Physics.Linecast(transform.parent.position, desiredCameraPosition, out hit))
            distance = Mathf.Clamp(hit.distance, minDist, maxDist);
        else
            distance = maxDist;

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDirection * distance, smoothing * Time.deltaTime);
    }
}
