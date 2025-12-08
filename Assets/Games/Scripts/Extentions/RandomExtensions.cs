using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    public static void ChangeIndex<T>(this List<T> array, int idx1, int idx2)
    {
        (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
    }
    
    public static void Shuffle<T>(this T[] array)
    {
        var n = array.Length;
        while (n > 1)
        {
            var k = Random.Range(0, n);
            n--;
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    
    public static void Shuffle<T>(this List<T> array)
    {
        var n = array.Count;
        while (n > 1)
        {
            var k = Random.Range(0, n);
            n--;
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    
    public static void Shuffle<T>(this List<T> array, int startIndex, System.Random rnd)
    {
        var n = array.Count;
        while (n > startIndex)
        {
            var k = rnd.Next(startIndex, n);
            n--;
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}