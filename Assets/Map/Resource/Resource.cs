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
    ShipParts
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
        {Resource.ShipParts, "Shipyards"}
    };

    public static Dictionary<Resource, string> ResourceString
    {
        get
        {
            return _resourceString;
        }
    }
}
