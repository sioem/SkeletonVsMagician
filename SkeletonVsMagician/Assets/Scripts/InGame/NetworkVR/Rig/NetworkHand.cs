using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHand : MonoBehaviour
{
    public PhotonView photonView;
    public NetworkRig rig;
    public RigPart side;

    public bool IsLocalNetworkRig => this.photonView.IsMine;

    public HardwareHand LocalHardwareHand =>
       IsLocalNetworkRig ? (side == RigPart.LeftController ? rig.hardwareRig.leftHand : rig.hardwareRig.rightHand) : null;

    public void SetLocalPosition(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }

    public void SetLocalRotation(Quaternion rot)
    {
        this.transform.localRotation = rot;
    }
}
