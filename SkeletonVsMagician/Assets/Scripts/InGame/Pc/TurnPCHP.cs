using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPCHP : MonoBehaviour
{
    [SerializeField]
    private GameObject[] arrPCGo = new GameObject[5];

    private Vector3 offset = new Vector3(0f, 0.25f, 0f);

    public float radius = 0.3f;
    public float rotateSpeed = 1f;
    public int goCount;

    public Transform playerTrans;

    private void Start()
    {
        //this.UpdateGoPosition();
        this.SetGoPosition();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject go in arrPCGo)
        {
            if (this.goCount > 1 && go.activeSelf)
            {
                this.OrbitAroundCenter(go.transform);
            }
        }
    }

    public void UpdateGoPosition()
    {
        if (this.goCount == 1)
        {
            Vector3 objectPosition = this.transform.position;
            this.arrPCGo[0].transform.position = objectPosition;
            this.arrPCGo[0].transform.LookAt(this.transform.forward);
        }
        else
        {
            for (int i = 0; i < goCount; i++)
            {
                float angle = i * 360f / this.goCount;
                Vector3 objectPosition = this.transform.localPosition + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
                this.arrPCGo[i].transform.localPosition = new Vector3(objectPosition.x,0,objectPosition.z);
            }
        }
    }

    private void SetGoPosition()
    {
        if (this.goCount == 1)
        {
            Vector3 objectPosition = this.transform.position;
            this.arrPCGo[0].transform.position = objectPosition;
            this.arrPCGo[0].transform.LookAt(this.transform.forward);
        }
        else
        {
            for (int i = 0; i < goCount; i++)
            {
                float angle = i * 360f / this.goCount;
                Vector3 objectPosition = this.transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
                this.arrPCGo[i].transform.position = objectPosition;
            }
        }
    }

    private void OrbitAroundCenter(Transform goTransform)
    {
        goTransform.RotateAround(this.transform.position, this.playerTrans.up, this.rotateSpeed * Time.deltaTime);

      //  Vector3 lookDir = goTransform.position - this.transform.position;
      //  goTransform.rotation = Quaternion.LookRotation(lookDir);
    }

    public void UpdateGoActive()
    {
        if(this.goCount != 5) arrPCGo[this.goCount].SetActive(false);
    }
}
