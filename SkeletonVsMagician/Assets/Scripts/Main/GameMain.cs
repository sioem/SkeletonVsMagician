using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    #region Property
    [SerializeField]
    private List<GameObject> prefabs;
    [SerializeField]
    private CinemachineFreeLook cinemachineFreeLook;

    [SerializeField] private GameObject[] masterGo;
    [SerializeField] private GameObject[] clientGo;

    [SerializeField] private Button[] btnExitRooms;
    [SerializeField] private Button[] btnBackRooms;
    [SerializeField] private Button[] btnRematches;

    [Header("VR Result UI")]
    [SerializeField] private GameObject vrResultUI;
    [SerializeField] private GameObject vrWin;
    [SerializeField] private GameObject vrLose;

    [Header("PC UI")]
    [SerializeField] private GameObject[] pcAim;
    [SerializeField] private GameObject pcWin;
    [SerializeField] private GameObject pcLose;

    [SerializeField] private PCFade pcFade;
    [SerializeField] private GameObject pcStartUI;

    [SerializeField] private GameObject pcDieUI;

    [Header("VR Start UI")]
    [SerializeField] private GameOvrScreenFade oFade;
    [SerializeField] private GameObject vrStartUI;

    [SerializeField] private GameObject[] vrHpGos;
    [SerializeField] private TurnVRHP turnVRHP;

    private int pcDeathCount = 0;
    private bool isGameStart = false;

    private bool isMyPcDie = false;
    private PcPlayer[] arrPcPlayers;
    private int camIndex = 0;

    private PhotonView pv;

    [SerializeField] private Transform defaultSpawnTrans;
    [SerializeField] private MaterialChanger materialChanger;
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource fireAudio;
    #endregion

    private void Awake()
    {
        this.pv = this.GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        this.vrResultUI.SetActive(false);
        this.SetPool();
        this.DestoryGOs();

        this.CreatePlayer(PhotonNetwork.LocalPlayer);
        this.AddButtonEvents();
        this.DieEvents();

        EventDispatcher.instance.AddEventHandler<int>((short)VRPlayer.eVREventType.VRHIT_UPDATE_HP,
            (evtType, hp) =>
            {
                this.UpdateVRHP(this.vrHpGos, hp);
            });
    }

    
    private void Update()
    {
        // ����
        if (!PhotonNetwork.IsMasterClient && this.isMyPcDie)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (this.pcDieUI.activeSelf)
                {
                    this.pcDieUI.SetActive(false);
                }
                Transform camTrans = this.arrPcPlayers[camIndex].transform.Find("CamTrans");
                this.cinemachineFreeLook.LookAt = camTrans;
                this.cinemachineFreeLook.Follow = camTrans;
                this.camIndex++;
                if (this.camIndex >= this.arrPcPlayers.Length)
                {
                    camIndex = 0;
                }
            }
        }
        if (OVRInput.GetDown(OVRInput.Button.One) && !this.isGameStart) //A��ư Ŭ��
        {
            //���� ���� 
            this.vrStartUI.SetActive(false);
            this.oFade.FadeIn();
            this.pv.RPC("SetStartPcUI", RpcTarget.Others);
            this.StartAllPcPlayers();
            this.isGameStart = true;
        }
    }

    private void OnDisable()
    {
        EventDispatcher.instance.Clear();
        PcSettingManager.instance.AddReStartEvent();
        ButtonAudio.instance.AddButtonAudioEvent();
    }

    private void DieEvents()
    {
        EventDispatcher.instance.AddEventHandler((short)VRPlayer.eVREventType.VRDIE,
           (evtType) =>
           {
               this.GameEnd(false);
           });
        EventDispatcher.instance.AddEventHandler<Player>((short)PcPlayer.ePcEventType.PCDIE,
            (evtType, player) =>
            {
                this.pcDeathCount++;
                //Debug.LogFormat("pcDeathCount:{0}", pcDeathCount);
                if (player == PhotonNetwork.LocalPlayer)
                {
                    this.pcDieUI.SetActive(true);
                    this.arrPcPlayers = GameObject.FindObjectsOfType<PcPlayer>();
                    this.isMyPcDie = true;
                }

                if (this.pcDeathCount == PhotonNetwork.CurrentRoom.Players.Count - 1) //��ü pc �ο��� ���� ���� ��� 
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        this.pcDieUI.SetActive(false);
                    }
                    this.GameEnd(true);
                }
            });
    }

    private void UpdateVRHP(GameObject[] HPGo, int hp)
    {
        float hpBar = hp / 20f;
        int index = (int)hpBar;

        if (index + 1 < HPGo.Length && HPGo[index + 1].activeSelf)
        {
            HPGo[index + 1].SetActive(false);
            this.turnVRHP.goCount--;
        }

        if (Mathf.Approximately(hpBar, (int)hpBar))
        {
            HPGo[index].SetActive(false);
            this.turnVRHP.goCount--;
        }
        else
        {
            if (HPGo[index].GetComponentInChildren<ParticleSystem>() != null)
            {
                HPGo[index].GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void SetStartPcUI()
    {
        pcFade.GameFadeIn(0.66f, 0f);
        this.pcStartUI.SetActive(false);
    }
    private void GameEnd(bool isVRWin)
    {
        if (isVRWin)
        {
            if (PhotonNetwork.IsMasterClient) //VR ������ ���� 
            {
                this.SetPosVrResultUI(true);
                this.oFade.GameFadeOut();
                EventDispatcher.instance.SendEvent((short)VRPlayer.eVREventType.VRChangeController);
            }
            else
            {
                this.SetPcResultUI(false);
                this.pcFade.GameFadeOut(0, 0.66f, true);
            }
            //Debug.Log("VR Win!!");
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient) //VR ������ ���� 
            {
                this.SetPosVrResultUI(false);
                this.oFade.GameFadeOut();
                EventDispatcher.instance.SendEvent((short)VRPlayer.eVREventType.VRChangeController);
            }
            else
            {
                this.SetPcResultUI(true);
                this.pcFade.GameFadeOut(0, 0.66f, true);
            }
            //Debug.Log("VR DIE!!");
            this.StopAllPcPlayers();
            Cursor.lockState = CursorLockMode.None;
        }
        this.StartCoroutine(this.BgmVolumeDown());
    }

    private IEnumerator BgmVolumeDown()
    {
        while (true)
        {
            this.bgmAudio.volume -= 0.01f;
            this.fireAudio.volume -= 0.01f;
            yield return null;
            if(this.bgmAudio.volume <= 0)
            {
                this.fireAudio.Pause();
                this.bgmAudio.volume = 0;
                break;
            }
        }
    }

    private void SetPosVrResultUI(bool state)
    {
        var headPos = GameObject.FindAnyObjectByType<HardwareHead>().transform.position;
        this.vrResultUI.transform.position = new Vector3(headPos.x, headPos.y, headPos.z + 1.2f);
        this.vrResultUI.SetActive(true);

        if (state)
        {
            //Win Ȱ��ȭ
            vrWin.gameObject.SetActive(true);
            vrLose.gameObject.SetActive(false);
        }
        else
        {
            //lose Ȱ��ȭ
            vrWin.gameObject.SetActive(false);
            vrLose.gameObject.SetActive(true);
        }

    }

    private void SetPcResultUI(bool state)
    {
        foreach (var aim in this.pcAim)
        {
            aim.SetActive(false);
        }
        if (state)
        {
            //Win Ȱ��ȭ
            pcWin.gameObject.SetActive(true);
            pcLose.gameObject.SetActive(false);
        }
        else
        {
            //lose Ȱ��ȭ
            pcWin.gameObject.SetActive(false);
            pcLose.gameObject.SetActive(true);
        }
    }

    private void CreatePlayer(Player player)
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>(); //�θ���� ���� ã��
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("VrPlayer", Vector3.zero, Quaternion.identity);
        }
        else
        {
            GameObject playerGo = PhotonNetwork.Instantiate("PcPlayer", this.defaultSpawnTrans.position, this.defaultSpawnTrans.rotation);
            playerGo.SetActive(false);

            playerGo.GetComponent<PlayerNetworkGrabbable>().PlayerInit(player);
            playerGo.GetComponent<PlayerNetworkGrabbable>().pv.RPC("PlayerInit", RpcTarget.OthersBuffered, player);

            var pcPlayer = playerGo.GetComponent<PcPlayer>();
            pcPlayer.Init(player);
            pcPlayer.photonView.RPC("Init", RpcTarget.OthersBuffered, player);

            this.SetPcPlayerTrans(playerGo, pcPlayer.UniqueNumber, points);

            Transform camTrans = GameObject.Find("CamTrans").transform;

            this.cinemachineFreeLook.Follow = camTrans;
            this.cinemachineFreeLook.LookAt = camTrans;
            this.materialChanger.follow = camTrans;
        }
    }

    private void SetPcPlayerTrans(GameObject go, int num , Transform[] points)
    {
        //Debug.LogFormat("Points pos:{0} rot:{1}", points[num].position, points[num].rotation);
        //Debug.LogFormat("Points name:{0}", points[num].gameObject.name);
        go.transform.position = points[num].position;
        go.transform.rotation = points[num].rotation;
        go.SetActive(true);
    }

    private void StopAllPcPlayers()
    {
        var pcPlayers = GameObject.FindObjectsOfType<PcPlayer>();

        foreach (var pc in pcPlayers)
        {
            pc.photonView.RPC("StopPcPlayer", RpcTarget.All);
        }
    }

    private void StartAllPcPlayers()
    {
        var pcPlayers = GameObject.FindObjectsOfType<PcPlayer>();

        foreach (var pc in pcPlayers)
        {
            pc.photonView.RPC("StartPcPlayer", RpcTarget.All);
        }
    }


    private void AddButtonEvents()
    {
        foreach (var btnExitRoom in this.btnExitRooms)
        {
            btnExitRoom.onClick.AddListener(() =>
            {
                EventDispatcher.instance.SendEvent(1000);
                PhotonNetwork.LeaveRoom();
            });
        }

        foreach (var btnBackRoom in this.btnBackRooms)
        {
            btnBackRoom.onClick.AddListener(() =>
            {
                EventDispatcher.instance.SendEvent(1000);
                //�ε��� �鷶�ٰ� ���� 
                //�÷��̾� ����Ʈ�� �ʱ�ȭ�� ���ɼ� ���� 
                NetworkManager.instance.ArrPcPlayers = PcSettingManager.instance.arrPcPlayers;
                this.DestorySettingManager();
                this.pv.RPC("DestorySettingManager", RpcTarget.Others);
                PhotonNetwork.LoadLevel("RoomScene");
            });
        }

        foreach (var btnRematch in this.btnRematches)
        {
            btnRematch.onClick.AddListener(() =>
            {
                EventDispatcher.instance.SendEvent(1000);
                //�ε��� �鷶�ٰ� ���� 
                //���� �� ��ε� �Ϸ��� RPC�� �ѷ��� �� 
                EventDispatcher.instance.SendEvent(100); //���߿� enum���� �ϱ� 
                PhotonNetwork.LoadLevel("LoadingScene");
            });
        }
    }
    [PunRPC]
    private void DestorySettingManager()
    {
        var playerSettingManager = GameObject.FindAnyObjectByType<PcSettingManager>();
        Destroy(playerSettingManager.gameObject);
    }

    //Ŀ�� ���α� �̾ȿ� ���� 
    private void DestoryGOs()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var go in this.clientGo)
            {
                Destroy(go);
            }
        }
        else
        {
            foreach (var go in this.masterGo)
            {
                Destroy(go);
            }
            Cursor.lockState = CursorLockMode.Locked; // Ŀ�����α�
        }
    }

    private void SetPool()
    {
        var pool = PhotonNetwork.PrefabPool as DefaultPool;
        foreach (GameObject prefab in this.prefabs)
        {
            if (!pool.ResourceCache.ContainsKey(prefab.name))
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }
}
