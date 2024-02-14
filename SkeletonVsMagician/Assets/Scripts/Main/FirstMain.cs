using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class FirstMain : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] private Button btnGame;
    [SerializeField] private Button btnQuit;

    [Header("Select Mode")]
    [SerializeField] private Button btnVR;
    [SerializeField] private Button btnPC;

    [Header("NickName")]
    [SerializeField] private TMP_Text txtNickName;
    [SerializeField] private Button btnChangeNickname;

    [Header("State")]
    [SerializeField] private TMP_Text txtState;

    [SerializeField] private GameObject loadingGo;
    [SerializeField] private MyOVRScreenFade oFade;

    public bool isVR;

    private Dictionary<int, string> nicknames = new Dictionary<int, string>();


    // Start is called before the first frame update
    void Start()
    {
        //pc ���̵� �����ϸ�, �ε� �� 
        this.oFade.onFadeEnd = () =>
        {
            SceneManager.LoadSceneAsync("LoadingScene");
        };

        this.LoadNicknameDatas();
        this.SetNickname(NetworkManager.instance.Nickname);
        this.ButtonExecutionEvents();
        //this.SetISVR(NetworkManager.instance.IsVR);
        if (this.isVR)
        {
            this.txtState.text = string.Format("����� <color=yellow>Skeleton</color> �Դϴ�");
        }
        else
        {
            this.txtState.text = string.Format("����� <color=yellow>Magician</color> �Դϴ�");
        }

    }

    private void ButtonExecutionEvents()
    {
        this.btnQuit.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            this.AppQuit();
        });
        this.btnGame.onClick.AddListener(() =>
        {
            EventDispatcher.instance.SendEvent(1000);
            if (this.isVR)
            {
                this.oFade.FadeOut();
                NetworkManager.instance.ConnectVR(this.txtNickName.text);
            }
            else
            {
                //pc ���̵� 
                this.loadingGo.SetActive(true);
                NetworkManager.instance.ConnectPC(this.txtNickName.text); //���� ���� 
            }
        });

        this.btnChangeNickname.onClick.AddListener(() => {
            EventDispatcher.instance.SendEvent(1000);
            this.ChangeNickname();
        });

        if(this.btnVR != null)
        {
            this.btnVR.onClick.AddListener(() =>
            {
                EventDispatcher.instance.SendEvent(1000);
                this.isVR = true;
                this.txtState.text = string.Format("����� <color=yellow>Skeleton</color> �Դϴ�");
                //�ε�â ����
            });
        }
      
        if(this.btnPC != null)
        {
            this.btnPC.onClick.AddListener(() =>
            {
                EventDispatcher.instance.SendEvent(1000);
                this.isVR = false;
                this.txtState.text = string.Format("����� <color=yellow>Magician</color> �Դϴ�");

                //�ε�â ����                                                               
            });
        }
    }

    private void AppQuit()
    {
        //Debug.Log("�� ����");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

    private void LoadNicknameDatas()
    {
        TextAsset aseet = Resources.Load<TextAsset>("Data/nickname_data"); //json�� �� ����

        //������ȭ 
        NicknameData[] nicknameDatas = JsonConvert.DeserializeObject<NicknameData[]>(aseet.text);
        foreach (var nicknameData in nicknameDatas)
        {
            //Debug.LogFormat("id:{0} nickname : {1}", nicknameData.id, nicknameData.nickname);
            this.nicknames.Add(nicknameData.id, nicknameData.nickname);
        }
    }

    private void SetNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            int rnd = UnityEngine.Random.Range(0, this.nicknames.Count); //0~19���� ����
            this.txtNickName.text = this.nicknames[rnd];
        }
        else
        {
            this.txtNickName.text = nickname;
        }

    }

    private void ChangeNickname()
    {
        int rnd = UnityEngine.Random.Range(0, this.nicknames.Count); //0~19���� ����
        this.txtNickName.text = this.nicknames[rnd];
    }

    private void SetISVR(bool isVR)
    {
        this.isVR = isVR;
    }
}
