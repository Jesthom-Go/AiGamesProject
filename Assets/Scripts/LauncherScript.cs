using UnityEngine;
using Photon.Pun;

namespace Com.KKNKK.ThelianJob
{
    public class Launcher : MonoBehaviour
    {

        [SerializeField] private GameObject ControlPanel;
        [SerializeField] private GameObject ProgressLabel;

        string gameVer = "1";

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            ProgressLabel.SetActive(false);
            ControlPanel.SetActive(true);
        }

        public void Connect()
        {

            ProgressLabel.SetActive(true);
            ControlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVer;
            }
        }

    }
}