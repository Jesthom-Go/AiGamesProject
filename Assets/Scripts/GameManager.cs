using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform spawnPointPlayer1;
    public Transform spawnPointPlayer2;

    private void Start()
    {
        Debug.Log("GameManager Start() - nothing happens here.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom called. Now spawning the player.");

        int index = PhotonNetwork.LocalPlayer.ActorNumber;
        Transform spawnPoint = (index == 1) ? spawnPointPlayer1 : spawnPointPlayer2;

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
    }
}
