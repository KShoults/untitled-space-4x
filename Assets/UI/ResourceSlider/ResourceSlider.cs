using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSlider : MonoBehaviour
{
    public PlanetPanel planetPanel;
    public Slider Slider;
    public RectTransform FillArea, HandleArea;
    // The resource this slider is tracking
    public Resource resource;
    private float targetSize, resourceSize, availableSize, planetSize;

    // Called when a planet is selected and all of the sliders values are set up
    public void InitializeSlider(float targetSize, float resourceSize, float availableSize, int planetSize)
    {
        this.targetSize = targetSize;
        this.resourceSize = resourceSize;
        this.availableSize = availableSize;
        this.planetSize = planetSize;
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Set the width of the slider determined by the available resources
        rectTransform.offsetMax = new Vector2(160f * resourceSize / planetSize, rectTransform.offsetMax.y);

        Slider.value = targetSize;
        
        UpdateWidth();
    }

    // Called by the planetPanel to update the available size
    public void UpdateSlider(float availableSize)
    {
        this.availableSize = availableSize;

        UpdateWidth();
    }

    public void OnValueChanged(float value)
    {
        if (value != targetSize)
        {
            targetSize = value;
            planetPanel.SetIndustryTarget(resource, value);
        }
    }

    // Updates the width of the fill area and handle area based on available size
    private void UpdateWidth()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float totalWidth = rectTransform.offsetMax.x - 10;
        float areaWidth = (availableSize + targetSize) / resourceSize;  // Percentage of the width that should be available
        areaWidth = areaWidth > 1 ? 1 : areaWidth;
        FillArea.offsetMax = new Vector2(-(totalWidth - totalWidth * areaWidth + 5), FillArea.offsetMax.y);
        HandleArea.offsetMax = new Vector2(-(totalWidth - totalWidth * areaWidth + 5), HandleArea.offsetMax.y);

        // Set the maxValue as well
        Slider.maxValue = resourceSize > availableSize + targetSize ? availableSize + targetSize : resourceSize;
    }
}
