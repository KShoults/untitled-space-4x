using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileControl : MonoBehaviour
{
    // The buttons on this TileControl.
    // 0: Yield Button 1, 1: Yield Button 2, 2: Clear Development Button,
    // 3: Advanced Industry Button, 4: Other Development Button
    public Image[] buttons, advancedIndustryButtons;
    public PlanetDevelopmentControls planetDevelopmentControls;

    public void OnYieldButtonClick(int yieldButton)
    {
        planetDevelopmentControls.OnYieldButtonClick(this, yieldButton);
    }

    public void OnCancelButtonClick()
    {
        planetDevelopmentControls.OnCancelButtonClick(this);
    }

    public void OnAdvancedIndustriesButtonClick()
    {
        // If they're already active deactivate them
        if (advancedIndustryButtons[0].gameObject.activeSelf)
        {
            foreach (Image i in advancedIndustryButtons)
            {
                i.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Image i in advancedIndustryButtons)
            {
                i.gameObject.SetActive(true);
            }
            planetDevelopmentControls.OnAdvancedIndustriesButtonClick(this);
        }
    }

    public void OnCivilianIndustryButtonClick()
    {
        planetDevelopmentControls.OnCivilianIndustryButtonClick(this);
        ClosePopups();
    }

    public void OnMilitaryIndustryButtonClick()
    {
        planetDevelopmentControls.OnMilitaryIndustryButtonClick(this);
        ClosePopups();
    }

    public void OnShipyardIndustryButtonClick()
    {
        planetDevelopmentControls.OnShipyardIndustryButtonClick(this);
        ClosePopups();
    }

    public void ClosePopups()
    {
        foreach (Image i in advancedIndustryButtons)
        {
            i.gameObject.SetActive(false);
        }
    }
}
