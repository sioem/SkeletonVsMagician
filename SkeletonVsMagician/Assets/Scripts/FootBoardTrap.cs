using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBoardTrap : MonoBehaviour
{
    private PhotonView pv;
    private bool isTouch = false;
    private float elapsedTime = 0;
    private Vector3 originalPos;

    [SerializeField] private Transform startTrans;
    private void Start()
    {
        //게임 끝났을 때 계속 발사하는거 막아야 됨
        this.pv = GetComponent<PhotonView>();
        this.originalPos = this.transform.position;
    }
    private void Update()
    {
        if (isTouch)
        {
            this.elapsedTime += Time.deltaTime;
        }

        if (this.elapsedTime >= 2f)
        {
            isTouch = false;
            this.transform.position = new Vector3(this.transform.localPosition.x, this.originalPos.y, this.transform.localPosition.z);
            this.elapsedTime = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && this.isTouch == false)
        {
            this.transform.position = new Vector3(this.transform.localPosition.x, this.transform.position.y - 0.03f, this.transform.localPosition.z);
            isTouch = true;
            if (this.pv.IsMine) this.BallSpawn();
        }
    }

    private void BallSpawn()
    {
        if (isTouch)
        {
            PhotonNetwork.Instantiate("FireBall", this.startTrans.position, this.startTrans.rotation);
        }
    }
}
