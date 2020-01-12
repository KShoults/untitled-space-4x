using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemDevelopmentOverlay : OverlayObject
{
    public StarSystem starSystem;
    // This is a prefab for the overlay over each planet in the system
    public SystemPlanetDevelopmentOverlay PlanetDevelopmentOverlayPrefab;
    private List<SystemPlanetDevelopmentOverlay> planetDevelopmentOverlays;

    public override void Initialize(MonoBehaviour viewObject)
    {
        if (viewObject is StarSystem)
        {
            starSystem = (StarSystem) viewObject;
            base.Initialize(viewObject);
        }
    }

    void OnEnable()
    {
        if (starSystem == null)
        {
            return;
        }

        // The first time this runs we need to initialize a list
        if (planetDevelopmentOverlays == null)
        {
            planetDevelopmentOverlays = new List<SystemPlanetDevelopmentOverlay>();
        }

        // Add more planetDevelopmentOverlays if necessary
        while (planetDevelopmentOverlays.Count < starSystem.planets.Count)
        {
            SystemPlanetDevelopmentOverlay newOverlay = Instantiate(PlanetDevelopmentOverlayPrefab);
            newOverlay.transform.SetParent(transform);
            newOverlay.GetComponent<Canvas>().worldCamera = Camera.main;
            planetDevelopmentOverlays.Add(newOverlay);
        }

        // Assign planets to planetDevelopmentOverlays
        for (int i = 0; i < planetDevelopmentOverlays.Count; i++)
        {
            if (i < starSystem.planets.Count)
            {
                planetDevelopmentOverlays[i].planet = starSystem.planets[i];
                planetDevelopmentOverlays[i].transform.position = starSystem.planets[i].transform.position + new Vector3(0,0,-1);
                planetDevelopmentOverlays[i].gameObject.SetActive(true);
            }
            // Disable any overlays we don't need
            else
            {
                planetDevelopmentOverlays[i].gameObject.SetActive(false);
            }
        }
    }
}
