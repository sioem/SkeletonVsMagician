using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] private Animator pcAnim;

    public void AnimationControl()
    {
        //Debug.Log("AnimationControl");
        this.pcAnim.SetBool("isAttack", false);
    }
}
