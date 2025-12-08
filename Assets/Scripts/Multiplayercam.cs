using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MultiPlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float smoothSpeed = 0.15f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 10f;

    private List<Transform> targets = new List<Transform>();
    private Vector3 velocity;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        UpdateTargets();
    }

    private void LateUpdate()
    {
        UpdateTargets();
        if (targets.Count == 0) return;

        Move();
        Zoom();
    }

    private void UpdateTargets()
    {
        targets.Clear();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            targets.Add(player.transform);
        }
    }

    private void Move()
    {
        // Get center point of all targets
        Vector3 center = GetCenterPoint();

        Vector3 desiredPosition = center + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private void Zoom()
    {
        if (!cam.orthographic) return;

        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }
}
