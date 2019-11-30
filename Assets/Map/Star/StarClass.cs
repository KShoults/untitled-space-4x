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

    private static readonly Dictionary<StarClass, Dictionary<Resource, int>> _starResources = new Dictionary<StarClass, Dictionary<Resource, int>>
    {
        {StarClass.O, new Dictionary<Resource, int> {{Resource.Energy, 10}, {Resource.Water, 0}, {Resource.Food, 4}}},
        {StarClass.B, new Dictionary<Resource, int> {{Resource.Energy, 8}, {Resource.Water, 1}, {Resource.Food, 6}}},
        {StarClass.A, new Dictionary<Resource, int> {{Resource.Energy, 6}, {Resource.Water, 2}, {Resource.Food, 8}}},
        {StarClass.F, new Dictionary<Resource, int> {{Resource.Energy, 4}, {Resource.Water, 4}, {Resource.Food, 6}}},
        {StarClass.G, new Dictionary<Resource, int> {{Resource.Energy, 2}, {Resource.Water, 6}, {Resource.Food, 4}}},
        {StarClass.K, new Dictionary<Resource, int> {{Resource.Energy, 1}, {Resource.Water, 8}, {Resource.Food, 2}}},
        {StarClass.M, new Dictionary<Resource, int> {{Resource.Energy, 0}, {Resource.Water, 10}, {Resource.Food, 1}}}
    };

    public static Dictionary<StarClass, Dictionary<Resource, int>> StarResources
    {
        get
        {
            return _starResources;
        }
    }
}