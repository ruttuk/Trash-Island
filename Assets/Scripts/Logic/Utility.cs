using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
