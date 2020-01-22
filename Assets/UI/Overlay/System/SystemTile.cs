using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemTile : MonoBehaviour
{
    public Tile tile;
    /*public Sprite EnergyLowYieldSprite, EnergyMediumYieldSprite, EnergyHighYieldSprite, EnergyUncommonYieldSprite, EnergyRareYieldSprite,
                    WaterLowYieldSprite, WaterMediumYieldSprite, WaterHighYieldSprite, WaterUncommonYieldSprite, WaterRareYieldSprite,
                    FoodLowYieldSprite, FoodMediumYieldSprite, FoodHighYieldSprite, FoodUncommonYieldSprite, FoodRareYieldSprite,
                    MineralsLowYieldSprite, MineralsMediumYieldSprite, MineralsHighYieldSprite, MineralsUncommonYieldSprite, MineralsRareYieldSprite;*/

    void OnEnable ()
    {
        if (tile == null)
        {
            return;
        }

        if (tile.development == null)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = ResourceUtil.ResourceColors[tile.development.producedResource];
        }
    }
}
