using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public CanvasGroup fadeOut_CG;

    public void BeginLoading()
    {
        StartCoroutine(FadeToBlack());
    }

    private float fadeIncrement = 0.01f;

    IEnumerator FadeToBlack()
    {
        while(fadeOut_CG.alpha > 0f)
        {
            fadeOut_CG.alpha -= fadeIncrement;
            yield return new WaitForSeconds(fadeIncrement);
        }

        yield return Utility.Load(1, 0f);
    }
}
