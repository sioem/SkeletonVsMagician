using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRigidBody : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PhotonView pv;

    public Transform chaseTrans;
    public BoxCollider boxCollider;

    private void Start()
    {
        this.boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine && this.chaseTrans != null)
        {
            this.rb.Move(this.chaseTrans.position, this.chaseTrans.rotation);
            if(Vector3.Distance(this.transform.position, this.chaseTrans.position) <= 0.1f)
            {
                this.boxCollider.enabled = true;
            }
        }
    }
}
