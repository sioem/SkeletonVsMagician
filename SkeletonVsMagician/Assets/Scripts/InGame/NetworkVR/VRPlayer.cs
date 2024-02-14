using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour
{
    public enum eVREventType
    {
        VRDIE=0, VRChangeController=1, VRHIT_UPDATE_HP=2, VRHIT_EFFECT =3
    }
    private int maxHp = 200;
    private int hp;
    [SerializeField] private VRHead vrHead;
    [SerializeField] private VRHand vrLeftHand;
    [SerializeField] private VRHand vrRightHand;

    [SerializeField] private Material damageMaterial;
    private PhotonView pv;

    private Coroutine headCoroutine;
    private Coroutine leftCoroutine;
    private Coroutine rightCoroutine;

    private Action onDie;
    private void Awake()
    {
        this.hp = this.maxHp;
        this.pv = this.GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        this.onDie = () =>
        {
            this.gameObject.SetActive(false); // NetworkRig 비활성화 
        };
        this.vrHead.onHeadDamageAction = () =>
        {
            this.HitDamage(15);
            this.StartHeadCoroutine(0.8f);

            this.pv.RPC("HitDamage", RpcTarget.Others, 15);
            this.pv.RPC("StartHeadCoroutine", RpcTarget.Others, 0.8f);
        };

        this.vrLeftHand.onHandDamageAction = () =>
        {
            this.HitDamage(10);
            this.StartLeftHandCoroutine(0.8f);

            this.pv.RPC("HitDamage", RpcTarget.Others, 10);
            this.pv.RPC("StartLeftHandCoroutine", RpcTarget.Others, 0.8f);
        };

        this.vrRightHand.onHandDamageAction = () =>
        {
            this.HitDamage(10);
            this.StartRightHandCoroutine(0.8f);

            this.pv.RPC("HitDamage", RpcTarget.Others, 10);
            this.pv.RPC("StartRightHandCoroutine", RpcTarget.Others, 0.8f);
        };
    }
    [PunRPC]
    private void HitDamage(int damage)
    {
        this.hp -= damage;
        if (this.hp > 0)
        {
            EventDispatcher.instance.SendEvent((short)eVREventType.VRHIT_UPDATE_HP, this.hp);
            if(this.pv.IsMine) EventDispatcher.instance.SendEvent((short)eVREventType.VRHIT_EFFECT); //VR 일때만 호출 
        }
        if (this.hp <= 0)
        {
            this.hp = 0;
            EventDispatcher.instance.SendEvent((short)eVREventType.VRDIE); //0
            onDie();
        }
    }

    private IEnumerator SetHeadDamageMaterial(float time)
    {
        vrHead.meshRenderer.material = this.damageMaterial;

        yield return new WaitForSeconds(time);

        vrHead.meshRenderer.material = null;
    }

    private IEnumerator SetLeftHandDamageMaterial(float time)
    {
        Material[] materialsArray = vrLeftHand.meshRenderer.materials;
        materialsArray[1] = damageMaterial;
        vrLeftHand.meshRenderer.materials = materialsArray;

        yield return new WaitForSeconds(time);

        materialsArray[1] = null;
        vrLeftHand.meshRenderer.materials = materialsArray;
    }

    private IEnumerator SetRightHandDamageMaterial(float time)
    {
        Material[] materialsArray = vrRightHand.meshRenderer.materials;
        materialsArray[1] = damageMaterial;
        vrRightHand.meshRenderer.materials = materialsArray;

        yield return new WaitForSeconds(time);

        materialsArray[1] = null;
        vrRightHand.meshRenderer.materials = materialsArray;
    }
    [PunRPC]
    private void StartHeadCoroutine(float time)
    {
        if (headCoroutine != null)
        {
            StopCoroutine(headCoroutine);
            headCoroutine = null;
        }
        if (this.gameObject.activeSelf == true)
        {
            this.headCoroutine = this.StartCoroutine(this.SetHeadDamageMaterial(time));
        }
    }
    [PunRPC]
    private void StartLeftHandCoroutine(float time)
    {
        if (leftCoroutine != null)
        {
            StopCoroutine(leftCoroutine);
            leftCoroutine = null;
        }
        if (this.gameObject.activeSelf == true)
        {
            this.leftCoroutine = this.StartCoroutine(this.SetLeftHandDamageMaterial(time));
        }

    }
    [PunRPC]
    private void StartRightHandCoroutine(float time)
    {
        if (rightCoroutine != null)
        {
            StopCoroutine(rightCoroutine);
            rightCoroutine = null;
        }
        if (this.gameObject.activeSelf == true)
        {
            this.rightCoroutine = this.StartCoroutine(this.SetRightHandDamageMaterial(time));
        }
    }


}
