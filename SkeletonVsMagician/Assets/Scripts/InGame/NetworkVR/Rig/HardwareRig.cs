using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RigPart
{
    None,
    Headset,
    LeftController,
    RightController,
    Undefined
}
public class HardwareRig : MonoBehaviour
{
    public HardwareHead headset;
    public HardwareHand leftHand;
    public HardwareHand rightHand;

    public NetworkRig networkRig;

    public GameObject controllers;
    public GameObject controllerHands;
    
    private void Start()
    {
        EventDispatcher.instance.AddEventHandler((short)VRPlayer.eVREventType.VRChangeController, (evtType) =>
        {
            this.ControllerChange();
        });
    }

    private void ControllerChange()
    {
        //Debug.Log("Controller change Event!!!");
        this.controllers.SetActive(true);
        this.controllerHands.SetActive(false);
    }

    private void Update()
    {
        if(this.networkRig != null)
        {
            this.networkRig.TrasformSet(this.transform);

            this.networkRig.headset.SetLocalPosition(this.headset.transform.localPosition);
            this.networkRig.leftHand.SetLocalPosition(this.leftHand.transform.localPosition);
            this.networkRig.rightHand.SetLocalPosition(this.rightHand.transform.localPosition);

            this.networkRig.headset.SetLocalRotation(this.headset.transform.localRotation);
            this.networkRig.leftHand.SetLocalRotation(this.leftHand.transform.localRotation);
            this.networkRig.rightHand.SetLocalRotation(this.rightHand.transform.localRotation);
        }
    }

    private void OnDisable()
    {
        EventDispatcher.instance.RemoveEventHandler(((short)VRPlayer.eVREventType.VRChangeController), (evtType) =>
        {
            this.ControllerChange();
        });
    }
}
