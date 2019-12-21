using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOverlayObject : OverlayObject
{
    public Planet planet;

    // Start is called before the first frame update
    void Start()
    {
        RegisterOverlayObject(View.Region, Overlay.Development);
    }

    protected override bool ShouldBeActive(MonoBehaviour viewObject)
    {
        if (viewObject is Planet && (Planet)viewObject == planet)
        {
            return true;
        }
        return false;
    }
}
