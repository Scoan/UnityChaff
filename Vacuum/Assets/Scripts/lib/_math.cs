using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _Math
{
    public static class MyMath {

        public static float ReconstructZ(Vector2 in_xy)
        {
            /// Given an XY vector (as a Vector2, 3, or 4), computes the missing Z value assuming the vector should have a length of 1.
            ///
            return (1 - Mathf.Sqrt(Mathf.Pow(in_xy.x, 2.0f) + Mathf.Pow(in_xy.y, 2.0f)));    
        }
    }  
}