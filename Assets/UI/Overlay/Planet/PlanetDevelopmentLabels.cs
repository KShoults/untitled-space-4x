using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetDevelopmentLabels : OverlayObject
{
    public Planet planet;
    public TextMeshProUGUI SizeText, HabitabilityText,
        TotalDevelopmentText, EnergyDevelopmentText, WaterDevelopmentText, FoodDevelopmentText, MineralDevelopmentText,
        CivilianDevelopmentText, MilitaryDevelopmentText, ShipyardDevelopmentText;
    // Quick lookup for the industry development texts
    public Dictionary<Resource, TextMeshProUGUI> developmentTexts;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDevelopmentTexts();
        RegisterOverlayObject(View.Region, Overlay.Development);
    }

    void OnEnable()
    {
        // This skips if the planet hasn't been loaded
        if (planet.tiles == null)
        {
            return;
        }
        UpdateLabels();
    }

    protected override bool ShouldBeActive(MonoBehaviour viewObject)
    {
        if (viewObject is Region && viewObject == planet.parentRegion)
        {
            return true;
        }
        return false;
    }

    public void UpdateLabels()
    {
        SizeText.text = "Size: " + planet.planetSize;
        HabitabilityText.text = "Habitability: " + planet.habitability;
        
        int totalDevelopment = 0;
        // Loop through the Resource texts
        foreach (KeyValuePair<Resource, TextMeshProUGUI> kvp in developmentTexts)
        {
            // If the planet has this industry
            if (planet.industries.ContainsKey(kvp.Key))
            {
                // Loop through the industry's tiles to add up development
                int development = 0;
                foreach(Tile t in planet.industries[kvp.Key].tiles)
                {
                    development += (int)t.resources[kvp.Key];
                }
                // Write the label
                kvp.Value.text = kvp.Key + ": " + development;
                totalDevelopment += development;
            }
            else
            {
                kvp.Value.text = kvp.Key + ": 0";
            }
        }

        TotalDevelopmentText.text = "Total: " + totalDevelopment;
    }



    private void InitializeDevelopmentTexts()
    {
        developmentTexts = new Dictionary<Resource, TextMeshProUGUI>();
        // Basic industries
        developmentTexts.Add(Resource.Energy, EnergyDevelopmentText);
        developmentTexts.Add(Resource.Water, WaterDevelopmentText);
        developmentTexts.Add(Resource.Food, FoodDevelopmentText);
        developmentTexts.Add(Resource.Minerals, MineralDevelopmentText);
        // Advanced industries
        developmentTexts.Add(Resource.CivilianGoods, CivilianDevelopmentText);
        developmentTexts.Add(Resource.MilitaryGoods, MilitaryDevelopmentText);
        developmentTexts.Add(Resource.ShipParts, ShipyardDevelopmentText);
    }
}
