using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform mainCam;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            mainCam = GameObject.Find("Main Camera").transform;
        }
        else
        {
            mainCam = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.LookAt(transform.position + mainCam.rotation * Vector3.forward, mainCam.rotation * Vector3.up);
        }
    }
}
