using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHead : MonoBehaviour
{
    public Action onHeadDamageAction;
    [SerializeField] private PhotonView pv;
    public MeshRenderer meshRenderer;

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
                //Debug.Log("VR Head Damage!");
                onHeadDamageAction();
            }
        }

        if (other.CompareTag("CannonBall") && this.pv.IsMine) //vr¿¡¼­¸¸ 
        {
            onHeadDamageAction();
        }
    }
}
