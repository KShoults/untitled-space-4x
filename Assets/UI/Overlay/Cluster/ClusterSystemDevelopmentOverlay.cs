using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClusterSystemDevelopmentOverlay : MonoBehaviour
{
    public StarSystem starSystem;
    public TextMeshProUGUI SystemNameText;

    void OnEnable()
    {
        if (starSystem == null)
        {
            gameObject.SetActive(false);
            return;
        }

        SystemNameText.text = starSystem.starSystemShortName;

        bool isDeveloped = false;
        // Check for player allocation in this system
        foreach (Planet p in starSystem.planets)
        {
            if (p.producers.Count > 0)
            {
                isDeveloped = true;
                break;
            }
        }
        if (isDeveloped)
        {
            SystemNameText.color = Color.red;
        }
        else
        {
            SystemNameText.color = Color.white;
        }
    }
}
