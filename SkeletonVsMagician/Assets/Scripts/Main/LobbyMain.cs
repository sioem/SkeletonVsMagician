using OVR;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class LobbyMain : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private Button btnEnterRoom;
    [SerializeField] private TMP_Text txtBtnEnterRoom;
    [SerializeField] private Button btnBackToFirst;

    [SerializeField] private PCFade pcFade;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private GameObject roomJoinFailedPopup;


    // Start is called before the first frame update
    void Start()
    {
        this.txtNickname.text = NetworkManager.instance.Nickname;

        this.pcFade.onFadeFinished = () =>
        {
            string roomCode = this.roomCodeInputField.text;
            NetworkManager.instance.onJoinRoomRequest(roomCode); // �� ����

            SceneManager.LoadSceneAsync("LoadingScene");    //�ε�â ���� 
        };

        this.btnEnterRoom.onClick.AddListener(() => {
            EventDispatcher.instance.SendEvent(1000);
            string roomCode = this.roomCodeInputField.text;
            if (string.IsNullOrEmpty(roomCode))
            {
                //Debug.Log("<color=red>�� ��ȣ�� �Է����ּ���!</color>");
                this.roomCodeInputField.placeholder.GetComponent<TMP_Text>().color = Color.red;
            }
            else
            {
                //Debug.Log("�� ����!");
                this.pcFade.FadeOut(false);
            }
        });

        this.btnBackToFirst.onClick.AddListener(() => {
            EventDispatcher.instance.SendEvent(1000);
            NetworkManager.instance.CloseRoom();
        });
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(this.roomCodeInputField.text))
        {
            this.txtBtnEnterRoom.color = new Color(1, 0.62f, 0.22f, 1); //��Ȳ��
        }
        else
        {
            this.txtBtnEnterRoom.color = Color.gray;
        }
    }


}
