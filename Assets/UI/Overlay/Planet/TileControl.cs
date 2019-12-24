﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileControl : MonoBehaviour
{
    // The buttons on this TileControl.
    // 0: Yield Button 1, 1: Yield Button 2, 2: Clear Development Button,
    // 3: Advanced Industry Button, 4: Other Development Button
    public Image[] buttons;
    public PlanetDevelopmentControls planetDevelopmentControls;

    public void OnYieldButtonClick(int yieldButton)
    {
        planetDevelopmentControls.OnYieldButtonClick(this, yieldButton);
    }

    public void OnCancelButtonClick()
    {
        planetDevelopmentControls.OnCancelButtonClick(this);
    }
}
