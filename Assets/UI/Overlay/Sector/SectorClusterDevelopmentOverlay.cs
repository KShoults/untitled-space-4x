using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SectorClusterDevelopmentOverlay : MonoBehaviour
{
    public Cluster cluster;

    public TextMeshProUGUI ClusterNameText;

    void OnEnable()
    {
        if (cluster == null)
        {
            gameObject.SetActive(false);
            return;
        }

        ClusterNameText.text = cluster.clusterName;
        
        bool isDeveloped = false;
        // Check for player allocation in this system
        foreach (StarSystem s in cluster.starSystems)
        {
            foreach (Planet p in s.planets)
            {
                if (p.producers.Count > 0)
                {
                    isDeveloped = true;
                    break;
                }
            }
        }
        if (isDeveloped)
        {
            ClusterNameText.color = Color.red;
        }
        else
        {
            ClusterNameText.color = Color.white;
        }
    }
}
