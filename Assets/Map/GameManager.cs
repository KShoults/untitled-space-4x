using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    // Galaxy generation default settings
    public int numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar, clusterSpacing;
    public Sector sector;

    void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManager = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create the sector using perlin noise maps
        sector.GenerateSector(numClusters, clusterSpacing, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar);
        // Set the camera position
        Camera.main.GetComponent<CameraController>().SetCameraPosition(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
