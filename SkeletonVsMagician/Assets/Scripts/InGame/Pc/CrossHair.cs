using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private Image circleImg;
    private Color color;

    private void Start()
    {
        this.color = this.circleImg.color;
    }
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, this.distance))
        {
            //hit �Ȱ� �ִٸ�
            this.color.a = 1.0f;
            this.circleImg.color = this.color;
        }
        else
        {
            //hit �Ȱ� ���ٸ�
            this.color.a = 0.5f;
            this.circleImg.color = this.color;
        }
    }
}
