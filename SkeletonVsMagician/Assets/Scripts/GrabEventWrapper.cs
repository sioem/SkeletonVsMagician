using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabEventWrapper : MonoBehaviour
{
    [SerializeField]
    private FollowRigidBody followRigidBodyLeft;
    [SerializeField]
    private FollowRigidBody followRigidBodyRight;

    private BoxCollider grabbedObjCollider;

    public void Grabbed(GameObject go)
    {
        grabbedObjCollider = go.GetComponent<BoxCollider>();
    }

    private void OnOffObjCollider(bool onOff)
    {
        grabbedObjCollider.enabled = onOff;
    }

    private void OnOffRigidColliderLeft(bool onOff)
    {
        this.followRigidBodyLeft.boxCollider.center = this.grabbedObjCollider.center;
        this.followRigidBodyLeft.boxCollider.size = this.grabbedObjCollider.size;    
    }

    private void OnOffRigidColliderRight(bool onOff)
    {
        this.followRigidBodyRight.boxCollider.center = this.grabbedObjCollider.center;
        this.followRigidBodyRight.boxCollider.size = this.grabbedObjCollider.size; 
    }

    public void GrabRight()
    {
        if(grabbedObjCollider != null)
        {
            this.followRigidBodyRight.chaseTrans = this.grabbedObjCollider.transform;
            this.OnOffObjCollider(false);
            this.OnOffRigidColliderRight(true);
        }
      
    }

    public void GrabLeft()
    {
        if(grabbedObjCollider != null)
        {
            this.followRigidBodyLeft.chaseTrans = this.grabbedObjCollider.transform;
            this.OnOffObjCollider(false);
            this.OnOffRigidColliderLeft(true);
        }
        
    }

    public void ReleaseRight()
    {
        if(grabbedObjCollider != null)
        {
            this.OnOffObjCollider(true);
            this.OnOffRigidColliderRight(false);
            this.followRigidBodyRight.chaseTrans = null;
            this.followRigidBodyRight.boxCollider.enabled = false;
        }
    }

    public void ReleaseLeft()
    {
        if(grabbedObjCollider != null)
        {
            this.OnOffObjCollider(true);
            this.OnOffRigidColliderLeft(false);
            this.followRigidBodyLeft.chaseTrans = null;
            this.followRigidBodyLeft.boxCollider.enabled = false;
        }
    }
}
