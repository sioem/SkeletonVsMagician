using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaOrb : MonoBehaviour
{
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private GameObject orb;

    [SerializeField] private PhotonView pv;

    [SerializeField] private float elapsedTime = 0.0f;
    [SerializeField] private AudioSource manaAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !pv.IsMine) //PC¸¸ 
        {
            this.pv.RPC("ChangeState", RpcTarget.AllViaServer, false);
        }
    }

    private void Update()
    {
        if (this.orb.activeSelf==false)
        {
            this.elapsedTime += Time.deltaTime;
        }

        if(this.elapsedTime > 10.0f && !pv.IsMine)
        {
            elapsedTime = 0.0f;
            this.ChangeState(true);
            this.pv.RPC("ChangeState", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    private void ChangeState(bool state)
    {
        this.col.enabled = state;
        this.orb.SetActive(state);
        if(state == false)
        {
            this.manaAudio.Play();
        }
    }

}
