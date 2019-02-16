using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    string version = "1";

    public GameObject cameraPrefab;

    #region Base Callbacks

    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region PUN Callbacks

    public override void OnConnectedToMaster() {
#if UNITY_EDITOR
        Debug.Log("Connecting to master...");
#endif

        base.OnConnectedToMaster();

        Debug.Log(PhotonNetwork.CountOfRooms);

        JoinBaseRoom();
    }

    public override void OnJoinedRoom() {
#if UNITY_EDITOR
        Debug.Log("Found a room!");
#endif
        base.OnJoinedRoom();

        Debug.Log(string.Format("Joined {0} with {1} players!", PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount));
    }

    public override void OnDisconnected(DisconnectCause cause) {
#if UNITY_EDITOR
        Debug.Log("Disconnected!");
#endif
        base.OnDisconnected(cause);
    }

    public override void OnCreatedRoom() {
        base.OnCreatedRoom();

        Debug.Log("Made a room :)");
    }

    #endregion

    /// <summary>
    /// Handle the initial connection to the servers
    /// </summary>
    public void Connect() {
        if (PhotonNetwork.IsConnected) {
            JoinBaseRoom();
        } else {
            PhotonNetwork.GameVersion = version;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinBaseRoom() {
        RoomOptions opts = new RoomOptions();
        opts.IsVisible = true;
        opts.MaxPlayers = 4;
        opts.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Our Room", opts, TypedLobby.Default);

        PhotonNetwork.Instantiate("NetCam", Vector3.zero, Quaternion.identity);
    }
}

