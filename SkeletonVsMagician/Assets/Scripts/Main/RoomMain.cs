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
            //����
            this.SaveArrPcPlayers();
        });

        this.btnLeaveRoom.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            NetworkManager.instance.CloseRoom();
        });
        this.txtRoomCode.text = PhotonNetwork.CurrentRoom.Name;

        this.SetGiantNickname();
        //�̺�Ʈ ��� 
        this.UpdatePcPlayerNickname();
        //�̺�Ʈ ȣ��
        NetworkManager.instance.onJoinRoomUIAction();
        if (PhotonNetwork.IsMasterClient) NetworkManager.instance.ReOpenRoom();
        this.PcPlayerCheck(); // �� ������ ���� ��
        this.arrPcPlayers = NetworkManager.instance.ArrPcPlayers;

        for (int i = 0; i < arrPcPlayers.Length; i++) //�������ڸ��� ����
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
                PhotonNetwork.CurrentRoom.PlayerCount - 1, PhotonNetwork.CurrentRoom.MaxPlayers - 1); //���� ���� 
        };

        NetworkManager.instance.onPlayerLeaveRoomUIAction = () =>
        {
            this.SetLeaveKnightNickname();
            this.txtRoomPlayerCount.text = string.Format("{0}/{1}",
                PhotonNetwork.CurrentRoom.PlayerCount - 1, PhotonNetwork.CurrentRoom.MaxPlayers - 1); //���� ����
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
                players.Add(player); //������ �����ϰ� ����Ʈ�� �ֱ� 
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            this.txtKngihtNickname[i].text = player.NickName; //�г��� ���� 
        }
    }

    private void SetLeaveKnightNickname()
    {
        foreach (var playerNickname in this.txtKngihtNickname)//���� �г��� �ʱ�ȭ 
        {
            playerNickname.text = string.Format("Empty");

        }
        //�ʱ�ȭ �ϰ� ������ ���� �ٽ� ���� 
        List<Player> players = new List<Player>();

        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            var player = pair.Value;
            if (player.IsMasterClient == false)
            {
                players.Add(player); //������ �����ϰ� ����Ʈ�� �ֱ� 
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            this.txtKngihtNickname[i].text = player.NickName; //�г��� ���� 
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
