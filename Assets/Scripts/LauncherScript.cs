using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_InputField usernameInput;
    public TMP_InputField roomNameInput;
    public TMP_Text statusText;
    public Button createRoomButton;
    public Button joinRoomButton;

    private void Start()
    {
        Debug.Log("Initializing.");
        // Disable buttons until connected
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        statusText.text = "Connecting to Photon...";
        PhotonNetwork.ConnectUsingSettings(); // Immediately connect
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");
        statusText.text = "Connected! You can create or join rooms.";

        // Enable buttons now that connection is ready
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;

        PhotonNetwork.AutomaticallySyncScene = true;

        // THIS BETTER BE THE THING THAT'S FUCKING UP MY SHIT, BRO PLEASE.
        // N O P E .
        PhotonNetwork.JoinLobby();

        Debug.Log("Joined Lobby — matchmaking now works.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon: " + cause);
        statusText.text = "Disconnected: " + cause.ToString();

        // Disable buttons if disconnected
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

        // Optional: retry connection automatically
        PhotonNetwork.ConnectUsingSettings();
        statusText.text += " | Retrying connection...";
    }

    public void OnCreateRoomButton()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Still connecting to Photon...";
            return;
        }

        if (string.IsNullOrEmpty(usernameInput.text))
        {
            statusText.text = "Enter a username!";
            return;
        }

        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        PhotonNetwork.NickName = usernameInput.text;
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomNameInput.text, options);
        statusText.text = "Creating room...";
    }

    public void OnJoinRoomButton()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Still connecting to Photon...";
            return;
        }

        if (string.IsNullOrEmpty(usernameInput.text))
        {
            statusText.text = "Enter a username!";
            return;
        }

        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        PhotonNetwork.NickName = usernameInput.text;
        PhotonNetwork.JoinRoom(roomNameInput.text);
        statusText.text = "Joining room...";
    }

    public override void OnCreatedRoom()
    {
        statusText.text = "Room created! Waiting for other players...";
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined room! Loading Level 1...";
        PhotonNetwork.LoadLevel("Level 1"); // Only load here
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Create failed: " + message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Join failed: " + message;
    }
}
