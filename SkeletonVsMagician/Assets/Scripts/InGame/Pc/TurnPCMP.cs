using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPCMP : MonoBehaviour
{
    [SerializeField]
    private GameObject[] arrPCGo = new GameObject[5];

    private Vector3 offset = new Vector3(0f, 0.25f, 0f);

    public float radius = 0.3f;
    public float rotateSpeed = 1f;
    public int goCount;

    private void Start()
    {
        this.goCount = 0;
        this.UpdateGoPosition();
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
                this.arrPCGo[i].transform.localPosition = new Vector3(objectPosition.x, 0, objectPosition.z);
            }
        }
    }

    private void OrbitAroundCenter(Transform goTransform)
    {
        goTransform.RotateAround(this.transform.position, this.transform.up, this.rotateSpeed * Time.deltaTime);

        Vector3 lookDir = goTransform.position - this.transform.position;
        goTransform.rotation = Quaternion.LookRotation(lookDir);
    }

    public void AddItem()
    {
        if (goCount < this.arrPCGo.Length)
        {
            this.arrPCGo[goCount].SetActive(true);
            this.goCount++;
            this.UpdateGoPosition();
        }
    }

    public void UseItem()
    {
        if (goCount > 0)
        {
            this.goCount--;
            this.arrPCGo[goCount].SetActive(false);
            this.UpdateGoPosition();
        }
    }
}
