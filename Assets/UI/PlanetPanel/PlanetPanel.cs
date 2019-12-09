using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetPanel : MonoBehaviour
{
    // Planet Panel Texts
    public Text PlanetNameText, PlanetSizeText, PlanetHabitabilityText;
    // Resource Panel Texts
    public Text EnergyText, WaterText, FoodText, MineralText;
    // Resource Panel Sliders
    public ResourceSlider EnergySlider, WaterSlider, FoodSlider, MineralSlider;
    // Dictionary for quick lookup of resourceTexts
    public Dictionary<Resource, Text> resourceTexts;
    // Dictionary for quick lookup of resourceSliders
    public Dictionary<Resource, ResourceSlider> resourceSliders;
    // The planet that is being displayed
    private Planet planet;

    void Awake()
    {
        InitializeResourceTexts();
        InitializeResourceSliders();
    }

    public void SelectPlanet(Planet planet)
    {
        this.planet = planet;
        PlanetNameText.text = planet.planetName;
        PlanetSizeText.text = "Size: " + planet.planetSize;
        PlanetHabitabilityText.text = "Hab: " + planet.habitability;

        Dictionary<Resource, string> resourceStrings = ResourceUtil.ResourceString;
        // Calculate the total available size on the planet
        float availableSize = planet.planetSize;
        foreach (KeyValuePair<Resource, Industry> kvp in planet.industries)
        {
            availableSize -= kvp.Value.targetDevelopment;
        }
        // Set the resource panel texts and resource sliders
        foreach (KeyValuePair<Resource, float> kvp in planet.resources)
        {
            if (kvp.Value == 0)
            {
                resourceTexts[kvp.Key].gameObject.SetActive(false);
                resourceSliders[kvp.Key].gameObject.SetActive(false);
            }
            else
            {
                resourceTexts[kvp.Key].gameObject.SetActive(true);
                resourceTexts[kvp.Key].text = resourceStrings[kvp.Key] + ": " + (int)Mathf.Floor(kvp.Value);
                resourceSliders[kvp.Key].gameObject.SetActive(true);
                float targetDevelopment = planet.industries.ContainsKey(kvp.Key) ? planet.industries[kvp.Key].targetDevelopment : 0;

                resourceSliders[kvp.Key].InitializeSlider(targetDevelopment, kvp.Value, availableSize, planet.planetSize);
            }
        }
    }

    // Called by the resource sliders to indicate a new target development for a resource
    public void SetIndustryTarget(Resource resource, float target)
    {
        // Set the target development if the industry exists
        if (planet.industries.ContainsKey(resource))
        {
            planet.industries[resource].targetDevelopment = target;
        }
        // Create the industry if it doesn't exist
        else if (target > 0)
        {
            planet.industries.Add(resource, new Industry(target));
        }

        // Calculate the total available size on the planet
        float availableSize = planet.planetSize;
        foreach (KeyValuePair<Resource, Industry> kvp in planet.industries)
        {
            availableSize -= kvp.Value.targetDevelopment;
        }

        // Update the sliders with the new available size
        foreach (KeyValuePair<Resource, float> kvp in planet.resources)
        {
            resourceSliders[kvp.Key].UpdateSlider(availableSize);
        }
    }



    private void InitializeResourceTexts()
    {
        resourceTexts = new Dictionary<Resource, Text>
        {
            {Resource.Energy, EnergyText},
            {Resource.Water, WaterText},
            {Resource.Food, FoodText},
            {Resource.Minerals, MineralText}
        };
    }

    private void InitializeResourceSliders()
    {
        resourceSliders = new Dictionary<Resource, ResourceSlider>
        {
            {Resource.Energy, EnergySlider},
            {Resource.Water, WaterSlider},
            {Resource.Food, FoodSlider},
            {Resource.Minerals, MineralSlider}
        };
    }
}
