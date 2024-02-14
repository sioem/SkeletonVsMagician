using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialChanger : MonoBehaviour
{
    public Material defaultMaterial;
    public Material changeMaterial;
    public Transform follow;

    public Camera mainCam;

    private List<RaycastHit> hitList = new List<RaycastHit>();
    private List<Ray> rays = new List<Ray>();

    public float offset;

    // Update is called once per frame
    void Update()
    {
        if (follow != null) this.ProcessHits(); //거리 계산 대상 -> CamTrans
    }

    private void ProcessHits()
    {
        var playerDistance = Vector3.Distance(mainCam.transform.transform.position, this.follow.transform.position);
        this.rays = this.CreateRays(this.mainCam.transform.position, this.mainCam.transform.forward, this.mainCam.transform.right * this.offset);
        int layerMask = 1 << LayerMask.NameToLayer("Wall");

        foreach (Ray ray in this.rays)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

            if (hitList.Count > 0)
            {
                for (int i = 0; i < hitList.Count; i++)
                {
                    this.BackDefaultMaterial(hits, hitList, i);
                }
            }
        }

        hitList.Clear();

        foreach (Ray ray in this.rays)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
            this.ChangeMaterial(hits, playerDistance);
        }
        
    }

    private List<Ray> CreateRays(Vector3 pos, Vector3 dir, Vector3 offSet)
    {
        var rays = new List<Ray>();

        for (int i = -3; i <= 3; i++)//-2 -1 0 1 2 
        {
            Vector3 adjustedOffset = offSet * i;

            Ray ray = new Ray(pos + adjustedOffset, dir);
            rays.Add(ray);
            Debug.DrawRay(pos + adjustedOffset, dir, Color.red);
        }

        return rays;
    }


    private void BackDefaultMaterial(RaycastHit[] hits, List<RaycastHit> hitList, int i)
    {
        for (int j = 0; j < hits.Length; j++)
        {
            if (!hitList.Contains(hits[j]))
            {
                hitList[i].collider.gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }
    }

    private void ChangeMaterial(RaycastHit[] hits, float playerDistance)
    {
        foreach (RaycastHit hit in hits)
        {
            hitList.Add(hit);

            var dot = Vector3.Dot(this.mainCam.transform.forward, hit.collider.gameObject.transform.forward);

            if (hit.distance < playerDistance && dot > 0.5f)
            {
                var render = hit.collider.gameObject.GetComponent<MeshRenderer>();
                if (render != null) render.material = changeMaterial;
            }
        }
    }
}
