using UnityEngine;
using Photon.Pun;

public class PlayerCameraSetup : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>(true);

        if (pv.IsMine)
        {
            // Enable the camera only for the local player
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            // Disable remote players’ cameras
            playerCamera.gameObject.SetActive(false);
        }
    }
}
