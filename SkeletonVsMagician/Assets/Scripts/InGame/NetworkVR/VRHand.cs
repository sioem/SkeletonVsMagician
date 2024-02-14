using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHand : MonoBehaviour
{
    public Action onHandDamageAction;
    [SerializeField] private PhotonView pv;
    public SkinnedMeshRenderer meshRenderer;

    private void Awake()
    {
        this.pv = this.GetComponentInParent<PhotonView>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!this.pv.IsMine)
        {
            if (other.CompareTag("Rocket") || other.CompareTag("Hammer"))
            {
                //Debug.Log("VR Hand Damage!");
                onHandDamageAction();
            }
        }

        if (other.CompareTag("CannonBall") && this.pv.IsMine) //vr¿¡¼­¸¸ 
        {
            onHandDamageAction();
        }        
    }
}
