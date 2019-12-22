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

    // Determines the possible resources for a specific yield on a planet around a star of a specified class
    private static readonly Dictionary<StarClass, Dictionary<Yield, Resource[]>> _starTiles = new Dictionary<StarClass, Dictionary<Yield, Resource[]>>
    {
        {StarClass.O, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[2] {Resource.Minerals, Resource.Energy}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Energy}},
            {Yield.Uncommon, new Resource[2] {Resource.Minerals, Resource.Energy}},
            {Yield.Rare, new Resource[2] {Resource.Minerals, Resource.Energy}}
        }},

        {StarClass.B, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[3] {Resource.Minerals, Resource.Energy, Resource.Food}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Energy}},
            {Yield.Uncommon, new Resource[1] {Resource.Minerals}},
            {Yield.Rare, new Resource[1] {Resource.Minerals}}
        }},

        {StarClass.A, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[3] {Resource.Minerals, Resource.Energy, Resource.Food}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Food}},
            {Yield.Uncommon, new Resource[1] {Resource.Minerals}},
            {Yield.Rare, new Resource[1] {Resource.Minerals}}
        }},

        {StarClass.F, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[2] {Resource.Minerals, Resource.Food}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Food}},
            {Yield.Uncommon, new Resource[2] {Resource.Minerals, Resource.Food}},
            {Yield.Rare, new Resource[2] {Resource.Minerals, Resource.Food}}
        }},

        {StarClass.G, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[3] {Resource.Minerals, Resource.Food, Resource.Water}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Food}},
            {Yield.Uncommon, new Resource[1] {Resource.Minerals}},
            {Yield.Rare, new Resource[1] {Resource.Minerals}}
        }},

        {StarClass.K, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[3] {Resource.Minerals, Resource.Food, Resource.Water}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Water}},
            {Yield.Uncommon, new Resource[1] {Resource.Minerals}},
            {Yield.Rare, new Resource[1] {Resource.Minerals}}
        }},

        {StarClass.M, new Dictionary<Yield, Resource[]>
        {
            {Yield.Low, new Resource[4] {Resource.Minerals, Resource.Energy, Resource.Water, Resource.Food}},
            {Yield.Medium, new Resource[2] {Resource.Minerals, Resource.Water}},
            {Yield.High, new Resource[2] {Resource.Minerals, Resource.Water}},
            {Yield.Uncommon, new Resource[2] {Resource.Minerals, Resource.Water}},
            {Yield.Rare, new Resource[2] {Resource.Minerals, Resource.Water}}
        }}
    };

    public static Dictionary<StarClass, Dictionary<Yield, Resource[]>> StarTiles
    {
        get
        {
            return _starTiles;
        }
    }

    private static readonly Dictionary<StarClass, int> _starHabitability = new Dictionary<StarClass, int>
    {
        {StarClass.O, 1},
        {StarClass.B, 2},
        {StarClass.A, 4},
        {StarClass.F, 6},
        {StarClass.G, 8},
        {StarClass.K, 6},
        {StarClass.M, 4}
    };

    public static Dictionary<StarClass, int> StarHabitability
    {
        get
        {
            return _starHabitability;
        }
    }
}