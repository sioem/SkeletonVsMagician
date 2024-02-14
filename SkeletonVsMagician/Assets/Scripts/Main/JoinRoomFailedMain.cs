using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomFailedMain : MonoBehaviour
{
    [SerializeField]
    private Button btnJoinRoomFailed;

    private void Start()
    {
        this.btnJoinRoomFailed.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            PhotonNetwork.LoadLevel("LobbyScene");
        });
    }
}
