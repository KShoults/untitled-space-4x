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

    void OnEnable()
    {
        // This skips if the planet hasn't been selected
        if (planet == null)
        {
            return;
        }
        
        // The first time this runs we need to initialize some dictionaries
        if (developmentTexts == null)
        {
            InitializeDevelopmentTexts();
        }
        UpdateLabels();
    }

    public override void Initialize(MonoBehaviour viewObject)
    {
        if (viewObject is Region)
        {
            planet = (Planet)((Region)viewObject).orbitalObject;
            base.Initialize(viewObject);
        }
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
            if (planet.developments.ContainsKey(kvp.Key))
            {
                // Loop through the industry's tiles to add up development
                int development = 0;
                foreach(Tile t in planet.developments[kvp.Key].tiles)
                {
                    // Is this tile developed yet
                    if (t.development.tileDevelopments.ContainsKey(t))
                    {
                        development += (int)Mathf.Round(t.development.tileDevelopments[t]);
                    }
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
