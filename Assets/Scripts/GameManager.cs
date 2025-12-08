using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    //OH MY GOD WHYYYY DIDN'T YOU WOOORRRKKKK OMGGGGGG PLSSSSS :SOB:
    //public GameObject playerPrefab;
    //public Transform spawnPointPlayer1;
    //public Transform spawnPointPlayer2;

    //private void Start()
    //{
    //    Debug.Log("GameManager Start() - nothing happens here.");
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("OnJoinedRoom called. Now spawning the player.");

    //    int index = PhotonNetwork.LocalPlayer.ActorNumber;
    //    Transform spawnPoint = (index == 1) ? spawnPointPlayer1 : spawnPointPlayer2;

    //    PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
    //}

    [Header("Multiplayer Player Prefab")] 
    public GameObject playerPrefab; 
    [Header("Spawn Points")] 
    public Transform spawnPointPlayer1; 
    public Transform spawnPointPlayer2; 
    private void Start()
    {
        if (!PhotonNetwork.InRoom) 
        { 
            Debug.Log("Not in room, spawning local player for testing."); 
            Instantiate(playerPrefab, spawnPointPlayer1.position, Quaternion.identity); 
            return; 
        }

        Debug.Log($"PhotonNetwork.LocalPlayer.ActorNumber = {PhotonNetwork.LocalPlayer.ActorNumber}"); // Choose spawn point

        Transform spawnPoint = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? spawnPointPlayer1 : spawnPointPlayer2; 
        Debug.Log($"Spawning local player prefab at {spawnPoint.position}"); // Spawn the local player over the network
        GameObject localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity); 
        if (localPlayer != null) 
            Debug.Log($"Local player instantiated: {localPlayer.name}"); 
        else 
            Debug.LogError("Failed to instantiate local player prefab!"); 
    } 
}