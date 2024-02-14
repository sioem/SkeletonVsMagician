using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Haptic : MonoBehaviour
{
    public void OnSelect()
    {
        this.vibratorSensor();
    }

    private void vibratorSensor()
    {
        float primaryIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float primaryHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        float secondIndexTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        float secondHandTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);

        //�޼�
        if (primaryHandTrigger > 0)
        {
            StartCoroutine(VibratorController(0.6f, 1, 1, OVRInput.Controller.LTouch));
        }
        //�޼�
        else if (primaryIndexTrigger > 0)
        {
            StartCoroutine(VibratorController(0.6f, 1, 1, OVRInput.Controller.LTouch));
        }
        //������
        else if (secondHandTrigger > 0)
        {
            StartCoroutine(VibratorController(0.6f, 1, 1, OVRInput.Controller.RTouch));
        }
        //������
        else if (secondIndexTrigger > 0)
        {
            StartCoroutine(VibratorController(0.6f, 1, 1, OVRInput.Controller.RTouch));
        }
    }

    private IEnumerator VibratorController(float duration, float frequency, float amplitude, OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);

        yield return new WaitForSeconds(duration);

        OVRInput.SetControllerVibration(0, 0, controller); //���� ����
    }
}
