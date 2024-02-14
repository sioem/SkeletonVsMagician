using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareHand : MonoBehaviour
{
    public RigPart side;
    public NetworkHand networkHand;
    private HardwareRig hardRig;

    [SerializeField]
    private OVRInput.Controller controller;
    private void Awake()
    {
        this.hardRig = this.GetComponentInParent<HardwareRig>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.LogFormat("networkHand: {0}", this.networkHand);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.side == RigPart.RightController)
        {
            var flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            //var pinch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

            if (this.hardRig != null && hardRig.networkRig != null)
            {
            //    this.hardRig.networkRig.SetRightHandFlex(flex);
             //   this.hardRig.networkRig.photonView.RPC("SetRightHandFlex", RpcTarget.Others, flex);
            }
        }
        else if (this.side == RigPart.LeftController)
        {
            var flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            //var pinch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

            if (this.hardRig != null && hardRig.networkRig != null)
            {
              //  this.hardRig.networkRig.SetLeftHandFlex(flex);
              //  this.hardRig.networkRig.photonView.RPC("SetLeftHandFlex", RpcTarget.Others, flex);
            }
        }
    }
}
