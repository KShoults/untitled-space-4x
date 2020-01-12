using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterDevelopmentOverlay : OverlayObject
{
    public Cluster cluster;
    // This is a prefab for the overlay over each system in the cluster
    public ClusterSystemDevelopmentOverlay SystemDevelopmentOverlayPrefab;
    private List<ClusterSystemDevelopmentOverlay> systemDevelopmentOverlays;


    public override void Initialize(MonoBehaviour viewObject)
    {
        if (viewObject is Cluster)
        {
            cluster = (Cluster) viewObject;
            base.Initialize(viewObject);
        }
    }

    void OnEnable()
    {
        if (cluster == null)
        {
            return;
        }

        // The first time this runs we need to initialize a list
        if (systemDevelopmentOverlays == null)
        {
            systemDevelopmentOverlays = new List<ClusterSystemDevelopmentOverlay>();
        }

        // Add more systemDevelopmentOverlays if necessary
        while (systemDevelopmentOverlays.Count < cluster.starSystems.Count)
        {
            ClusterSystemDevelopmentOverlay newOverlay = Instantiate(SystemDevelopmentOverlayPrefab);
            newOverlay.transform.SetParent(transform);
            newOverlay.GetComponent<Canvas>().worldCamera = Camera.main;
            systemDevelopmentOverlays.Add(newOverlay);
        }

        // Assign systems to systemDevelopmentOverlays
        for (int i = 0; i < systemDevelopmentOverlays.Count; i++)
        {
            if (i < cluster.starSystems.Count)
            {
                systemDevelopmentOverlays[i].starSystem = cluster.starSystems[i];
                systemDevelopmentOverlays[i].transform.position = cluster.starSystems[i].transform.position + new Vector3(0,0,-1);
                systemDevelopmentOverlays[i].gameObject.SetActive(true);
            }
            // Disable any overlays we don't need
            else
            {
                systemDevelopmentOverlays[i].gameObject.SetActive(false);
            }
        }
    }
}
