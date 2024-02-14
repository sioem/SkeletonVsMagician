using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoMain : MonoBehaviour
{
    [SerializeField]
    private PCFade pcFade;
    [SerializeField]
    private MyOVRScreenFade OVRScreenFade;

    private void Awake()
    {
        if(this.pcFade != null)
        {
            pcFade.onFadeFinished = () =>
            {
                SceneManager.LoadScene("FirstScene");
            };
        }
        if(this.OVRScreenFade != null)
        {
            OVRScreenFade.onFadeEnd = () =>
            {
                SceneManager.LoadScene("FirstScene");
            };
        }
       
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        if(this.pcFade != null)
        {
            this.pcFade.FadeIn();
        }

        yield return new WaitForSeconds(1.5f);

        if(this.pcFade != null)
        {
            this.pcFade.FadeOut(false);
        }
        if (this.OVRScreenFade != null)
        {
            this.OVRScreenFade.FadeOut();
        }
    }
}
