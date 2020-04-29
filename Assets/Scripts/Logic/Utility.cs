using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utility
{
    /***
     *
     * Dedicated class for helper/utility functions that I find myself using often.
     * 
     ***/

    /***
     * Checks if the target transform is in range of the source transform. 
     * Note: This requires both game objects to have colliders!
     ***/
    public static bool CheckIfTransformInRange(Transform target, Transform source, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(target.position, range);

        foreach (Collider col in hitColliders)
        {
            if (col.transform.Equals(source))
            {
                return true;
            }
        }
        return false;
    }

    public static IEnumerator Fade(CanvasGroup cg, bool fadeIn, float fadeIncrement, float secondsOffset)
    {
        yield return new WaitForSeconds(secondsOffset);

        if(fadeIn)
        {
            while (cg.alpha < 1f)
            {
                cg.alpha += fadeIncrement;
                yield return new WaitForSeconds(fadeIncrement);
            }
        }
        else
        {
            while (cg.alpha > 0f)
            {
                cg.alpha -= fadeIncrement;
                yield return new WaitForSeconds(fadeIncrement);
            }
        }

        yield return null;
    }

    public static IEnumerator Load(int sceneIndex, float secondsOffset)
    {
        yield return new WaitForSeconds(secondsOffset);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }
}
