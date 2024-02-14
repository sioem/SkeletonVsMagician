using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Vector3 target;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Rigidbody rBody;

    [SerializeField]
    private GameObject muzzleParticle;
    [SerializeField]
    private GameObject impactParticle;

    private PhotonView pv;

    private void Start()
    {
        this.pv = GetComponent<PhotonView>();   

        this.target = GameObject.FindAnyObjectByType<NetworkHead>().transform.position;
        Vector3 dir = (this.target - this.transform.position).normalized;
        this.rBody.AddForce(dir * this.speed, ForceMode.Impulse);

        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            muzzleParticle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(muzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VR Hand") || other.CompareTag("VR Head"))
        {
            //Debug.LogFormat("<color=lime>{0}¶û ºÎµúÇô¼­ Á¦°Å</color>", other.gameObject.name);
            if (photonView.IsMine)
            {
                this.pv.RPC("CreateImpactParticle", RpcTarget.All);
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    [PunRPC]
    private void CreateImpactParticle()
    {
        GameObject impactP = Instantiate(this.impactParticle, transform.position, transform.rotation);
        impactP.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(impactP, 1f);
    }
}
