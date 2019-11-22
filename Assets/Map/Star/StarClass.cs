using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarClass
{
    O,
    B,
    A,
    F,
    G,
    K,
    M
}

public static class StarClassUtil
{
    private static readonly Dictionary<StarClass, Color> _starColor = new Dictionary<StarClass, Color>
    {
        {StarClass.O, Color.blue},
        {StarClass.B, new Color(.7f, .85f, .9f)},
        {StarClass.A, Color.white},
        {StarClass.F, new Color(1, 1, .9f)},
        {StarClass.G, Color.yellow},
        {StarClass.K, new Color(1, .65f, 0)},
        {StarClass.M, Color.red}
    };

    public static Dictionary<StarClass, Color> StarColor
    {
        get
        {
            return _starColor;
        }
    }
}