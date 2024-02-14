using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcCharacter : MonoBehaviour
{
    public PhotonView pv;
    [HideInInspector] public Material myMaterial;
    [HideInInspector] public string nickName;

    [PunRPC]
    public void SetNickname(string name)
    {
        this.nickName = name;
    }

    [PunRPC]
    private void SetEnAble()
    {
        this.gameObject.SetActive(true);
    }

    [PunRPC]
    private void SetDisable()
    {
        this.gameObject.SetActive(false);
    }
}
