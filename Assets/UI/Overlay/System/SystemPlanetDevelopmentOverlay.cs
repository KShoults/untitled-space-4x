using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemPlanetDevelopmentOverlay : MonoBehaviour
{
    public Planet planet;
    public SystemTile[] SystemTiles;

    void OnEnable()
    {
        if (planet == null)
        {
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < SystemTiles.Length; i++)
        {
            if (i >= planet.planetSize)
            {
                SystemTiles[i].gameObject.SetActive(false);
            }
            else
            {
                SystemTiles[i].tile = planet.tiles[i];
                SystemTiles[i].gameObject.SetActive(true);
            }
        }
    }
}
