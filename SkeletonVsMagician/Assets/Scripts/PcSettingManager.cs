using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcSettingManager : MonoBehaviour
{
    public static PcSettingManager instance;
    public Player[] arrPcPlayers = new Player[4];
    public PhotonView pv;

    public bool isCount = false;
    public float elaspedTime = 0;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            PcSettingManager.instance = this;
        }
    }

    //재시작 로직
    private void Start()
    {
        this.AddReStartEvent();
    }

    private void Update()
    {
        if (this.isCount)
        {
            this.elaspedTime += Time.deltaTime;
        }

        if (this.elaspedTime > 4f && PhotonNetwork.IsMasterClient)
        {
            this.isCount = false;
            this.elaspedTime = 0;
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public void AddReStartEvent()
    {
        EventDispatcher.instance.AddEventHandler(100,
            (evtType) =>
            {
                //Debug.Log("<color=yellow>재시작 이벤트 실행!!!</color>");
                this.isCount = true;
                this.elaspedTime = 0;
            });
    }

    [PunRPC]
    private void SetArrPcPlayers(Player arr1, Player arr2, Player arr3, Player arr4)
    {
        this.arrPcPlayers[0] = arr1;
        this.arrPcPlayers[1] = arr2;
        this.arrPcPlayers[2] = arr3;
        this.arrPcPlayers[3] = arr4;
    }
}
