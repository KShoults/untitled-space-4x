using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Galaxy generation default settings
    public int numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar, clusterSpacing;
    public Sector sector;

    // Start is called before the first frame update
    void Start()
    {
        // Create the sector using perlin noise maps
        sector.GenerateSector(numClusters, clusterSpacing, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
