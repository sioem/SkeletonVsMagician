using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0.0";
    public static NetworkManager instance;

    public Action<string> onJoinRoomRequest;
    public Action onJoinRoomUIAction;
    public Action onPlayerEnterRoomUIAction;
    public Action onPlayerLeaveRoomUIAction;
    public Action<Player> onPlayerEnterRoomAction;
    public Action<Player> onPlayerLeaveRoomAction;

    public bool IsVR { get; private set; }
    public string Nickname { get; private set; }
    public Dictionary<string, Material> PcPlayerBodyColors { get; } = new Dictionary<string, Material>();
    public Player[] ArrPcPlayers { get; set; } = new Player[4];
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            NetworkManager.instance = this;
        }

        this.onJoinRoomRequest = (roomCode) =>
        {
            PhotonNetwork.JoinRoom(roomCode);
        };
    }

    private void Connect(bool isVR, string nickName)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = version;
            PhotonNetwork.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
            Nickname = nickName;
            IsVR = isVR;
        }
    }
    public void ConnectPC(string nickName)
    {
        Connect(false, nickName);
    }

    public void ConnectVR(string nickName)
    {
        Connect(true, nickName);
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("������ ������ ����");
        if (PhotonNetwork.IsConnected) PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        //Debug.Log("�κ� ����");
        if (this.IsVR)
        {
            this.CreateRoom();
        }
        else
        {
            this.JoinLobbyScene();
        }
    }

    public override void OnLeftRoom()
    {
        var pcSettingManager = GameObject.FindAnyObjectByType<PcSettingManager>();
        if (pcSettingManager != null)
        {
            Destroy(pcSettingManager.gameObject);
        }

        //�ʱ�ȭ 
        for (int i = 0; i < ArrPcPlayers.Length; i++)
        {
            ArrPcPlayers[i] = null;
        }
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("�뿡 ����");
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("RoomScene");
        //Debug.LogFormat("�� �̸�: {0}", PhotonNetwork.CurrentRoom.Name);
        //Debug.LogFormat("�� ������ ��: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);        
        PhotonNetwork.LoadLevel("JoinRoomFailedScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.LogFormat("�濡 ������ �ο�:{0}", PhotonNetwork.CurrentRoom.PlayerCount);
        this.onPlayerEnterRoomUIAction();
        this.onPlayerEnterRoomAction(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.LogFormat("���� ���� �ο�:{0}", PhotonNetwork.CurrentRoom.PlayerCount);
        this.onPlayerLeaveRoomUIAction();
        this.onPlayerLeaveRoomAction(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.Log("Disconnected from server. Cause: " + cause);
        //Debug.Log("���� ����!");

        var pcSettingManager = GameObject.FindAnyObjectByType<PcSettingManager>();
        if (pcSettingManager != null)
        {
            Destroy(pcSettingManager.gameObject);
        }

        SceneManager.LoadScene("FirstScene");
        
        //�ʱ�ȭ 
        for(int i=0; i<ArrPcPlayers.Length; i++)
        {
            ArrPcPlayers[i] = null;  
        }
    }

    public void CloseRoom()
    {
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }
    private void JoinLobbyScene()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
    private void CreateRoom()
    {
        int rndRoomCode = UnityEngine.Random.Range(1000, 10000); //1000���� 9999������ ���� ���� ���� 
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.IsOpen = true;
        options.IsVisible = false;
        PhotonNetwork.CreateRoom(rndRoomCode.ToString(), options);
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        //Debug.LogFormat("<color=green>���� �÷��̾� ���ӺҰ�</color>");
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("GameScene");
    }

    public void ReOpenRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        //Debug.LogFormat("<color=green>���� �÷��̾� ���Ӱ���</color>");
    }
}
