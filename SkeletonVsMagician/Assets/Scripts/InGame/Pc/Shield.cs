using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrowObject"))
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 income = rb.velocity;
                Vector3 dir = other.transform.position - transform.position;
                Vector3 normal = dir.normalized;
                Vector3 reflect = Vector3.Reflect(income, normal);
                rb.velocity = reflect;
            }
        }
    }
}
