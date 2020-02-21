using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

	public static bool isLessThan(this Vector3 p1, Vector3 p2)
    {
        if(p1.x < p2.x)
        {
            return true;
        } else if(p1.x > p2.x)
        {
            return false;
        }

        if(p1.y < p2.y)
        {
            return true;
        } else if(p1.y > p2.y)
        {
            return false;
        }

        if(p1.z < p2.z)
        {
            return true;
        } else if(p1.z > p2.z)
        {
            return false;
        }

        return false;
    }
}
