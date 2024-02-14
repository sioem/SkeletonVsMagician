using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private GameObject muzzleParticle;
    [SerializeField]
    private GameObject impactParticle;

    public PhotonView pv;

    public Action onImpactParticleFinish;

    private void Awake()
    {
        this.onImpactParticleFinish = () =>
        {
            if (this.pv.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        this.pv = GetComponent<PhotonView>();

        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            muzzleParticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(muzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Rocket") && !other.CompareTag("CannonBall"))
        {
            //Vector3 contact = other.ClosestPoint(transform.position);
            //transform.position = contact;

            //if (pv.IsMine)
            //{
            //    PhotonNetwork.Destroy(this.gameObject);
            //    this.pv.RPC("CreateImpactParticle", RpcTarget.All);
            //}            
            this.pv.RPC("CreateImpactParticle", RpcTarget.All);
        }
    }

    [PunRPC]
    private void CreateImpactParticle()
    {
        GameObject impactP = Instantiate(this.impactParticle, transform.position, transform.rotation);
        impactP.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(impactP, 1f);
        this.onImpactParticleFinish();     
    }
}
