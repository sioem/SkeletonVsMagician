using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public static ButtonAudio instance;
    [SerializeField] private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            ButtonAudio.instance = this;
            this.AddButtonAudioEvent();
        }
    }

    public void AddButtonAudioEvent()
    {
        EventDispatcher.instance.AddEventHandler(1000, (evtType) =>
        {
            this.audioSource.Play();
        });
    }
}
