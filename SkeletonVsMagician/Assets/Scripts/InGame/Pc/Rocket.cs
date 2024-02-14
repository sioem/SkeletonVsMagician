using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private PhotonView pv;
    private void Start()
    {
        pv = GetComponent<PhotonView>();
        Destroy(this.gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Rocket"))
        {
            //Debug.LogFormat(" <color=cyan>{0} rocket hit</color>", other.tag);
            if (pv.IsMine) PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
