using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    None,
    Energy,
    Water,
    Food,
    Minerals,
    CivilianGoods,
    MilitaryGoods,
    ShipParts,
    Economy,
    MilitaryCapacity,
    TransportCapacity
}

public static class ResourceUtil
{
    private static readonly Dictionary<Resource, string> _resourceString = new Dictionary<Resource, string>
    {
        {Resource.Energy, "Energy"},
        {Resource.Water, "Water"},
        {Resource.Food, "Food"},
        {Resource.Minerals, "Minerals"},
        {Resource.CivilianGoods, "Civilian Industry"},
        {Resource.MilitaryGoods, "Military Industry"},
        {Resource.ShipParts, "Shipyards"},
        {Resource.Economy, "Economy"},
        {Resource.MilitaryCapacity, "HQ"},
        {Resource.TransportCapacity, "TransportHub"}
    };

    public static Dictionary<Resource, string> ResourceString
    {
        get
        {
            return _resourceString;
        }
    }

    private static readonly Dictionary<Resource, Color> _resourceColors = new Dictionary<Resource, Color>
    {
        {Resource.Energy, Color.yellow},
        {Resource.Water, Color.blue},
        {Resource.Food, Color.green},
        {Resource.Minerals, Color.red},
        {Resource.CivilianGoods, new Color(.55f, .27f, .07f)},
        {Resource.MilitaryGoods, new Color(1, .65f, 0)},
        {Resource.ShipParts, new Color(.5f, 0, .5f)},
        {Resource.Economy, new Color(1, .85f, 0)},
        {Resource.MilitaryCapacity, Color.black},
        {Resource.TransportCapacity, new Color(.5f, .5f, .5f)}
    };

    public static Dictionary<Resource, Color> ResourceColors
    {
        get
        {
            return _resourceColors;
        }
    }
}
