using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMain : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnLeaveRoom;

    [Header("Room Info Text")]
    [SerializeField] private TMP_Text txtRoomCode;
    [SerializeField] private TMP_Text txtRoomPlayerCount;

    [Header("Player Nickname Text")]
    [SerializeField] private TMP_Text txtGiantNickname;
    [SerializeField] private TMP_Text[] txtKngihtNickname;

    [SerializeField]
    private PCFade pcFade;

    [SerializeField] private MyOVRScreenFade oFade;

    [SerializeField] private PhotonView pvGiantDoll;

    [SerializeField] private GameObject[] pcCharacters;

    private Player[] arrPcPlayers = new Player[4];

    // Start is called before the first frame update
    void Awake()
    {
        if (this.oFade != null)
        {
            this.oFade.onFadeEnd = () =>
            {
                NetworkManager.instance.StartGame();
            };
        }
        this.btnStart.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            // NetworkManager.instance.StartGame();
            this.oFade.FadeOut();
            this.pvGiantDoll.RPC("AllPcFadeOut", RpcTarget.Others, null);
            //저장
            this.SaveArrPcPlayers();
        });

        this.btnLeaveRoom.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            NetworkManager.instance.CloseRoom();
        });
        this.txtRoomCode.text = PhotonNetwork.CurrentRoom.Name;

        this.SetGiantNickname();
        //이벤트 등록 
        this.UpdatePcPlayerNickname();
        //이벤트 호출
        NetworkManager.instance.onJoinRoomUIAction();
        if (PhotonNetwork.IsMasterClient) NetworkManager.instance.ReOpenRoom();
        this.PcPlayerCheck(); // 방 들어왔을 때만 됨
        this.arrPcPlayers = NetworkManager.instance.ArrPcPlayers;

        for (int i = 0; i < arrPcPlayers.Length; i++) //시작하자마자 끄기
        {
            pcCharacters[i].SetActive(false);
        }
    }

    private void Start()
    {
        //this.SetPool();
        if (!PhotonNetwork.IsMasterClient)
        {
            this.btnStart.gameObject.SetActive(false);
        }
        if (pcFade != null)
        {
            this.pcFade.FadeIn();
        }
    }

    private void Update()
    {
        this.SetEnablePcCharacters();
    }

    private void SetEnablePcCharacters()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < arrPcPlayers.Length; i++)
            {
                if (arrPcPlayers[i] != null)
                {
                    pcCharacters[i].SetActive(true);
                    pcCharacters[i].GetComponent<PcCharacter>().pv.RPC("SetEnAble", RpcTarget.Others);
                }
                else
                {
                    pcCharacters[i].SetActive(false);
                    pcCharacters[i].GetComponent<PcCharacter>().pv.RPC("SetDisable", RpcTarget.Others);
                }
            }
        }
    }

    private void UpdatePcPlayerNickname()
    {
        NetworkManager.instance.onJoinRoomUIAction = () =>
        {
            this.SetEnterKnightNickname();
            this.txtRoomPlayerCount.text = string.Format("{0}/{1}",
                PhotonNetwork.CurrentRoom.PlayerCount - 1, PhotonNetwork.CurrentRoom.MaxPlayers - 1);
        };

        NetworkManager.instance.onPlayerEnterRoomUIAction = () =>
        {
            this.SetEnterKnightNickname();
            this.txtRoomPlayerCount.text = string.Format("{0}/{1}",
                PhotonNetwork.CurrentRoom.PlayerCount - 1, PhotonNetwork.CurrentRoom.MaxPlayers - 1); //거인 제외 
        };

        NetworkManager.instance.onPlayerLeaveRoomUIAction = () =>
        {
            this.SetLeaveKnightNickname();
            this.txtRoomPlayerCount.text = string.Format("{0}/{1}",
                PhotonNetwork.CurrentRoom.PlayerCount - 1, PhotonNetwork.CurrentRoom.MaxPlayers - 1); //거인 제외
        };
    }

    private void SetGiantNickname()
    {
        this.txtGiantNickname.text = PhotonNetwork.MasterClient.NickName;
    }
    private void SetEnterKnightNickname()
    {
        List<Player> players = new List<Player>();

        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            var player = pair.Value;
            if (player.IsMasterClient == false)
            {
                players.Add(player); //마스터 제외하고 리스트에 넣기 
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            this.txtKngihtNickname[i].text = player.NickName; //닉네임 세팅 
        }
    }

    private void SetLeaveKnightNickname()
    {
        foreach (var playerNickname in this.txtKngihtNickname)//전부 닉네임 초기화 
        {
            playerNickname.text = string.Format("Empty");

        }
        //초기화 하고 위에서 부터 다시 세팅 
        List<Player> players = new List<Player>();

        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            var player = pair.Value;
            if (player.IsMasterClient == false)
            {
                players.Add(player); //마스터 제외하고 리스트에 넣기 
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            this.txtKngihtNickname[i].text = player.NickName; //닉네임 세팅 
        }
    }

    private void PcPlayerCheck()
    {
        NetworkManager.instance.onPlayerEnterRoomAction = (newPlayer) =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < arrPcPlayers.Length; i++)
                {
                    if (arrPcPlayers[i] == null)
                    {
                        arrPcPlayers[i] = newPlayer;
                        break;
                    }
                }
            }
        };

        NetworkManager.instance.onPlayerLeaveRoomAction = (leftPlayer) =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < arrPcPlayers.Length; i++)
                {
                    if (arrPcPlayers[i] == leftPlayer)
                    {
                        arrPcPlayers[i] = null;
                        break;
                    }
                }
            }
        };
    }

    private void SaveArrPcPlayers()
    {
        PcSettingManager.instance.arrPcPlayers = this.arrPcPlayers;
        PcSettingManager.instance.pv.RPC("SetArrPcPlayers", RpcTarget.Others,
            this.arrPcPlayers[0], this.arrPcPlayers[1], this.arrPcPlayers[2], this.arrPcPlayers[3]);
    }

}
