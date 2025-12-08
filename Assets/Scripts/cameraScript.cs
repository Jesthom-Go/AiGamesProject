using UnityEngine;
using Photon.Pun;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Transform target;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponentInParent<PhotonView>();
        cam = GetComponent<Camera>();

        if (pv != null && pv.IsMine)
        {
            // Enable camera only for local player after ownership is confirmed
            cam.enabled = true; // safer than SetActive
            target = pv.transform;
            Debug.Log($"Camera assigned to local player: {pv.name}");
        }
        else
        {
            cam.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (!cam.enabled || target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );

        transform.position = smoothedPosition;
    }
}
