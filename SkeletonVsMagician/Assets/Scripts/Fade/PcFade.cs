using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCFade : MonoBehaviour
{
    private Image dim;

    public Action onFadeFinished;

    private void Awake()
    {
        this.dim = GetComponent<Image>();
    }

    public void FadeIn()
    {
        StartCoroutine(this.CoFadeIn(1f, 0f));
    }

    public void FadeOut(bool isConnecting)
    {
        StartCoroutine(this.CoFadeOut(0f, 1f, isConnecting));
    }

    public void GameFadeIn(float startAlpha, float endAlpha)
    {
        StartCoroutine(this.CoFadeIn(startAlpha, endAlpha));
    }

    public void GameFadeOut(float startAlpha, float endAlpha, bool isConnecting)
    {
        StartCoroutine(this.CoFadeOut(startAlpha, endAlpha, isConnecting));
    }

    private IEnumerator CoFadeIn(float startAlpha, float endAlpha)
    {
        Color alpha = this.dim.color;
        alpha.a = startAlpha;
        this.dim.color = alpha;

        while (true)
        {
            if (alpha.a <= endAlpha)
            {
                break;
            }
            alpha.a -= 0.01f;
            this.dim.color = alpha;
            yield return null;
        }
    }

    private IEnumerator CoFadeOut(float startAlpha, float endAlpha, bool isConnecting)
    {
        Color alpha = this.dim.color;
        alpha.a = startAlpha;
        this.dim.color = alpha;

        while (true)
        {
            if (alpha.a >= endAlpha)
            {
                if (!isConnecting)
                {
                    this.onFadeFinished();
                }
                break;
            }
            alpha.a += 0.01f;
            this.dim.color = alpha;
            yield return null;
        }
    }
}
