using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        EventDispatcher.instance.AddEventHandler<PcPlayer>((short)PcPlayer.ePcEventType.GrabImpact,
            (evtType, pcPlayer) =>
            {
                //Debug.Log("������ ������ ����");
                pcPlayer.HitDamage(10,pcPlayer.Player);
                pcPlayer.photonView.RPC("HitDamage", RpcTarget.Others, 10, pcPlayer.Player);
            });

        EventDispatcher.instance.AddEventHandler<PcPlayer>((short)PcPlayer.ePcEventType.ThrowObjectImpact,
            (evtType, pcPlayer) =>
            {
                //Debug.Log("���� ��ü�� �¾Ƽ� ������ ����");
                pcPlayer.HitDamage(10, pcPlayer.Player);
                pcPlayer.photonView.RPC("HitDamage", RpcTarget.Others, 10, pcPlayer.Player);
            });

        EventDispatcher.instance.AddEventHandler<PcPlayer>((short)PcPlayer.ePcEventType.GrabObjectImpact,
           (evtType, pcPlayer) =>
           {
               //Debug.Log("���� ��ü�� �¾Ƽ� ������ ����");
               pcPlayer.HitDamage(10, pcPlayer.Player);
               pcPlayer.photonView.RPC("HitDamage", RpcTarget.Others, 10, pcPlayer.Player);
           });

        EventDispatcher.instance.AddEventHandler<PcPlayer>((short)PcPlayer.ePcEventType.ThrowPlayer,
         (evtType, pcPlayer) =>
         {
             //Debug.Log("�������� ������ ����");
             pcPlayer.HitDamage(5, pcPlayer.Player);
             pcPlayer.photonView.RPC("HitDamage", RpcTarget.Others, 5, pcPlayer.Player);
         });
    }


}
