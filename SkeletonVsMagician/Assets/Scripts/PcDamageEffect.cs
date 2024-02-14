using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PcDamageEffect : MonoBehaviour
{
    private PostProcessVolume volume;
    private Vignette vignette; 

    private float smoothness = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.volume = GetComponent<PostProcessVolume>();
        this.volume.profile.TryGetSettings<Vignette>(out vignette);
        this.vignette.enabled.Override(true);

        EventDispatcher.instance.AddEventHandler((short)PcPlayer.ePcEventType.PCHIT,
           (evtType) =>
           {
               //ÄÚ·çÆ¾ 
               this.StartCoroutine(PcDamage());
           });
    }

    private IEnumerator PcDamage()
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
        // yield return null;
    }
}
