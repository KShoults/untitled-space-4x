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

        if (tile.tileOccupier != null && tile.tileOccupier is Producer)
        {
            GetComponent<Image>().color = ResourceUtil.ResourceColors[((Producer)tile.tileOccupier).producedResource];
        }
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}
