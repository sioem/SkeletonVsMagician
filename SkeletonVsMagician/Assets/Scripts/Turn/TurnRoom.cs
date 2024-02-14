using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject[] arrVRGo;

    public float radius = 0.3f;
    public float rotateSpeed = 1f;
    public int goCount;

    private void Start()
    {
        this.goCount = this.arrVRGo.Length;
        this.UpdateGoPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject go in arrVRGo)
            {
                if (this.goCount > 1)
                {
                    this.OrbitAroundCenter(go.transform);
                }
            }
        }
    }

    public void UpdateGoPosition()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (this.goCount == 1)
            {
                Vector3 objectPosition = this.transform.position;
                this.arrVRGo[0].transform.position = objectPosition;
                this.arrVRGo[0].transform.LookAt(this.transform.forward);
            }
            else
            {
                for (int i = 0; i < goCount; i++)
                {
                    float angle = i * 360f / this.goCount;
                    Vector3 objectPosition = this.transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
                    this.arrVRGo[i].transform.position = objectPosition;
                }
            }
        }
    }

    private void OrbitAroundCenter(Transform goTransform)
    {
        goTransform.RotateAround(this.transform.position, Vector3.up, this.rotateSpeed * Time.deltaTime);

        Vector3 lookDir = goTransform.position - this.transform.position;
        goTransform.rotation = Quaternion.LookRotation(lookDir);
    }
}
