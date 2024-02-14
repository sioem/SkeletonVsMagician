using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VrDamageEffect : MonoBehaviour
{
    private PostProcessVolume volume;
    private Vignette vignette;

    private float smoothness = 0;
    private Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        this.volume = GetComponent<PostProcessVolume>();
        this.volume.profile.TryGetSettings<Vignette>(out vignette);
        this.vignette.enabled.Override(true);

        EventDispatcher.instance.AddEventHandler((short)VRPlayer.eVREventType.VRHIT_EFFECT,
           (evtType) =>
           {
               if(this.coroutine != null)
               {
                   StopCoroutine(this.coroutine);
                   this.coroutine = null;
               }
               //ÄÚ·çÆ¾ 
               this.coroutine = this.StartCoroutine(VrDamage());
           });
    }

    private IEnumerator VrDamage()
    {
        while (true)
        {
            smoothness += 0.03f;
            if (smoothness >= 1)
            {
                smoothness = 1;
                break;
            }
            this.vignette.smoothness.Override(smoothness);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            smoothness -= 0.03f;
            if (smoothness <= 0)
            {
                smoothness = 0;
                break;
            }
            this.vignette.smoothness.Override(smoothness);
            yield return null;
        }
    }
}
