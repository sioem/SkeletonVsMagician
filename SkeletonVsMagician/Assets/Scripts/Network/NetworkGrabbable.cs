using Oculus.Interaction;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGrabbable : MonoBehaviour
{
    public InteractableUnityEventWrapper eventWrapper;
    public PhotonView pv;
    private Rigidbody rb;
    [SerializeField] private bool isPhysics = false;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.eventWrapper.WhenSelect.AddListener(() =>
        {
            if (isPhysics)
                this.pv.RPC("WhenSelect", RpcTarget.OthersBuffered, null);
        });
        this.eventWrapper.WhenUnselect.AddListener(() =>
        {
            if (isPhysics)
                this.pv.RPC("WhenUnSelect", RpcTarget.OthersBuffered, null);

        });
    }

    [PunRPC]
    private void WhenSelect()
    {
        this.rb.useGravity = false;
    }

    [PunRPC]
    private void WhenUnSelect()
    {
        this.rb.useGravity = true;
    }
}