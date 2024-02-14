using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPcFade : MonoBehaviour
{
    [SerializeField] private PCFade pcFade;

    [PunRPC]
    private void AllPcFadeOut()
    {
        this.pcFade.FadeOut(true);
    }
}
