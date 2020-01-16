using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemPlanetDevelopmentOverlay : MonoBehaviour
{
    public Planet planet;
    public SystemTile[] SystemTiles;
    public TextMeshProUGUI PlanetNameText;

    void OnEnable()
    {
        if (planet == null)
        {
            gameObject.SetActive(false);
            return;
        }

        PlanetNameText.text = planet.planetShortName;
        if (planet.developments.Count > 0)
        {
            PlanetNameText.color = Color.red;
        }
        else
        {
            PlanetNameText.color = Color.white;
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
