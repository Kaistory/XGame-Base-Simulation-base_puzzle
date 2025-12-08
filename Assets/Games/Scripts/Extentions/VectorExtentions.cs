using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtention
{
    public static float MagnitudeXZ(this Vector3 v)
    {
        return (float)Math.Sqrt(v.x * (double)v.x + v.z * (double)v.z);
    }
    
    public static float MagnitudeXY(this Vector3 v)
    {
        return (float)Math.Sqrt(v.x * (double)v.x + v.y * (double)v.y);
    }
    
    public static float MagnitudeY(this Vector3 v)
    {
        return  (float)Math.Sqrt(v.y * (double)v.y);
    }
}