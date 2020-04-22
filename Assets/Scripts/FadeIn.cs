using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeIn : MonoBehaviour
{
    private CanvasGroup fadeIn_CG;

    void Start()
    {
        fadeIn_CG = GetComponent<CanvasGroup>();
        StartCoroutine(Fade());
    }

    private float fadeIncrement = 0.01f;

    IEnumerator Fade()
    {
        while(fadeIn_CG.alpha > 0f)
        {
            fadeIn_CG.alpha -= fadeIncrement;
            yield return new WaitForSeconds(fadeIncrement);
        }
        yield return null;
    }
}
