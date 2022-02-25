using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

static class ListExtensions
{
    public static T Pop<T>(this List<T> list)
    {
        T r = list[0];
        list.RemoveAt(0);
        return r;
    }

    public static List<T> Shuffled<T>(this List<T> list)  
    {  
        return list.OrderBy(item => Random.Range(0.0f, 100.0f)).ToList();
    }
}
