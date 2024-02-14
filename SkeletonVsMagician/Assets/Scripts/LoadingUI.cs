using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtLoading;

    private float time;

    private void Start()
    {
        this.Init();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        this.UpdateUI(time);
    }

    private void Init()
    {
        this.time = 0f;
        this.txtLoading.text = "Loading";
    }

    private void UpdateUI(float time)
    {
        if (time > 4f)
        {
            this.Init();
        }
        else if (time > 3f)
        {
            this.txtLoading.text = "Loading...";
        }
        else if (time > 2f)
        {
            this.txtLoading.text = "Loading..";
        }
        else if (time > 1f)
        {
            this.txtLoading.text = "Loading.";
        }
    }
}
