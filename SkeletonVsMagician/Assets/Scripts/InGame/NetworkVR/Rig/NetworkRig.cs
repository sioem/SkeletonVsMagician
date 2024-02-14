using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRig : MonoBehaviour
{
    public PhotonView photonView;
    public HardwareRig hardwareRig;
    public NetworkHead headset;
    public NetworkHand leftHand;
    public NetworkHand rightHand;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            this.hardwareRig = GameObject.FindAnyObjectByType<HardwareRig>();
            this.hardwareRig.networkRig = this;
            if (this.hardwareRig == null) Debug.LogError("Missing HardwareRig in the Scene");
        }
    }

    public void TrasformSet(Transform trans)
    {
        this.transform.position = trans.position;
        this.transform.rotation = trans.rotation;
    }


}
