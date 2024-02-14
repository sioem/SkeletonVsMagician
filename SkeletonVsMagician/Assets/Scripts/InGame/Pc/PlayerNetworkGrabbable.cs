using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkGrabbable : MonoBehaviour, IPunOwnershipCallbacks
{
    public InteractableUnityEventWrapper eventWrapper;

    public PhotonView pv;
    private Rigidbody rb;
    [SerializeField] private bool isPhysics = false;
    public Player player;
    public bool isGrab = false;
    public bool isGrabbed = false;
    [SerializeField] private PcPlayer pcPlayer;

    [SerializeField] private HandGrabInteractable interactable;

    private float throwSpeed;
    private float controllerSpeed;
    private OVRInput.Controller controller;

    public float ThrowSpeed { get => throwSpeed; }
    public float ControllerSpeed { get => controllerSpeed; }

    void Awake()
    {
        this.rb = GetComponent<Rigidbody>();

        this.interactable.WhenInteractorViewAdded += Interactable_WhenInteractorViewAdded;
    }

    private void Interactable_WhenInteractorViewAdded(IInteractorView obj)
    {
        var x = obj as HandGrabInteractor;
        if (x.gameObject.name == "RightHandGrabInteractor")
        {
            this.controller = OVRInput.Controller.RTouch;
        }
        else if (x.gameObject.name == "LeftHandGrabInteractor")
        {
            this.controller = OVRInput.Controller.LTouch;
        }

    }

    [PunRPC]
    public void PlayerInit(Player player)
    {
        this.player = player;
    }
    private void Start()
    {
        this.pcPlayer.onImpactAction = () =>
        {
            this.pv.RequestOwnership(); //vr이 request
        };

        this.pcPlayer.onPlayerRequestAction = () =>
        {
            this.pv.RPC("PcPlayerRequest", this.player, null);
        };
        this.eventWrapper.WhenSelect.AddListener(() =>
        {
            this.pv.RequestOwnership();
            if (isPhysics)
            {
                this.isGrab = true;
                this.pv.RPC("WhenSelect", RpcTarget.OthersBuffered, null);
            }
        });
        this.eventWrapper.WhenUnselect.AddListener(() =>
        {
            if (isPhysics)
            {
                this.isGrab = false;
                this.isGrabbed = true;
                this.throwSpeed = OVRInput.GetLocalControllerVelocity(this.controller).magnitude;

                this.pv.RPC("WhenUnSelect", RpcTarget.OthersBuffered, null);
                this.pv.RPC("WhenPlayerUnSelect", this.player, null);
                this.controller = 0; //none
            }
        });
    }

    private void Update()
    {
        if (this.controller != 0)
        {
            this.controllerSpeed = OVRInput.GetLocalControllerVelocity(this.controller).magnitude;
        }
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    [PunRPC]
    private void WhenSelect()
    {
        this.rb.useGravity = false;
        this.isGrab = true;
        this.rb.isKinematic = true;
        this.pcPlayer.isGround = false;
    }

    [PunRPC]
    private void WhenUnSelect()
    {
        this.rb.useGravity = true;
        this.rb.isKinematic = false;
        this.isGrab = false;
        this.isGrabbed = true;
    }

    [PunRPC]
    private void WhenPlayerUnSelect()
    {
        this.pv.RequestOwnership();
    }
    [PunRPC]
    private void PcPlayerRequest()
    {
        this.pv.RequestOwnership();
    }
    #region OwnerShipCallback
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView.IsMine && this.player != requestingPlayer && this.isGrab == false) //잡을 때, targetView.IsMine이 pc일 때만 TRUE
        {
            this.pv.TransferOwnership(requestingPlayer);
        }
        else if (targetView.IsMine && this.player == requestingPlayer && this.isGrab == false) //놓을 때, targetView.IsMine이 VR일 때만 TRUE
        {
            this.pv.TransferOwnership(this.player);
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
       
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        
    }
    #endregion
}