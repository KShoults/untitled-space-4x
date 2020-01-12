using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorDevelopmentOverlay : OverlayObject
{
    public Sector sector;
    // This is a prefab for the overlay over each cluster in the sector
    public SectorClusterDevelopmentOverlay ClusterDevelopmentOverlayPrefab;
    private List<SectorClusterDevelopmentOverlay> clusterDevelopmentOverlays;


    public override void Initialize(MonoBehaviour viewObject)
    {
        if (viewObject == null)
        {
            sector = GameManager.gameManager.sector;
            base.Initialize(viewObject);
        }
    }

    void OnEnable()
    {
        // The first time this runs we need to initialize a list
        if (clusterDevelopmentOverlays == null)
        {
            clusterDevelopmentOverlays = new List<SectorClusterDevelopmentOverlay>();
        }

        // Add more clusterDevelopmentOverlays if necessary
        while (clusterDevelopmentOverlays.Count < sector.clusters.Count)
        {
            SectorClusterDevelopmentOverlay newOverlay = Instantiate(ClusterDevelopmentOverlayPrefab);
            newOverlay.transform.SetParent(transform);
            newOverlay.GetComponent<Canvas>().worldCamera = Camera.main;
            clusterDevelopmentOverlays.Add(newOverlay);
        }

        // Assign clusters to clusterDevelopmentOverlays
        for (int i = 0; i < clusterDevelopmentOverlays.Count; i++)
        {
            if (i < sector.clusters.Count)
            {
                clusterDevelopmentOverlays[i].cluster = sector.clusters[i];
                clusterDevelopmentOverlays[i].transform.position = sector.clusters[i].transform.position + new Vector3(0,0,-1);
                clusterDevelopmentOverlays[i].gameObject.SetActive(true);
            }
            // Disable any overlays we don't need
            else
            {
                clusterDevelopmentOverlays[i].gameObject.SetActive(false);
            }
        }
    }
}
