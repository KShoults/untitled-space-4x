using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public GameObject ClusterPrefab;
    public List<Cluster> clusters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /* Generate clusters and have them generate their systems
    /  numClusters: The guaranteed number of clusters in the sector
    /  numSystemsAvg: The average number of systems in each cluster
    /  numSystemsVar: The variance in the number of systems in each cluster
    /  sizePlanetsAvg: The average total aggregate size of planets in each system
    /  sizePlanetsVar: The variance in the total aggregate size of planets in each system
    */
    public void GenerateSector(int numClusters, int numSystemsAvg, float numSystemsVar, int sizePlanetsAvg, int sizePlanetsVar)
    {
        // Generate an unweighted noise map with a size based on the number of clusters
        int mapWidth = 500;
        float[,] weightMap = new float[mapWidth, mapWidth];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                weightMap[i,j] = 1f;
            }
        }
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(weightMap, 10f);

        // Log the noise map
        /*for (int i = 0; i < mapWidth; i++)
        {
            string row = "";
            for (int j = 0; j < mapWidth; j ++)
            {
                row += Mathf.Floor(noiseMap[i,j] * 10);
            }
            Debug.Log(row);
        }*/

        clusters = new List<Cluster>();
        // For each cluster, find the largest point in the noise map and place a cluster
        for (int cluster = 0; cluster < numClusters; cluster++)
        {
            float bestValue = 0;
            int bestX = 0, bestY = 0;

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (noiseMap[i,j] > bestValue)
                    {
                        bestX = i;
                        bestY = j;
                        bestValue = noiseMap[i,j];
                    }
                }
            }

            // Convert the noise map position to unity coordinates
            float unityXPosition = (bestX - mapWidth / 2f) * 2;
            float unityYPosition = (bestY - mapWidth / 2f) * 2;

            // Actual game coordinates of clusters are different than unity coordinates
            float xPosition = (bestX - mapWidth / 2f) * 1.5f;
            float yPosition = (bestY - mapWidth / 2f) * 1.5f;

            // Create and add the cluster
            Cluster newCluster = GameObject.Instantiate(ClusterPrefab).GetComponent<Cluster>();
            newCluster.transform.parent = transform;
            newCluster.transform.position = new Vector3(unityXPosition, unityYPosition, 0);
            newCluster.position = new Vector2(xPosition, yPosition);
            newCluster.clusterName = GenerateClusterName();
            newCluster.GenerateCluster(numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar);
            clusters.Add(newCluster);

            // Prevent other clusters from spawning too close
            int clusterSpacing = (int) Mathf.Floor((float)mapWidth / (float)numClusters / 2f) * 2;
            for (int i = 0-clusterSpacing; i < clusterSpacing; i++)
            {
                for (int j = 0-clusterSpacing; j < clusterSpacing; j++)
                {
                    int targetX = bestX + i, targetY = bestY + j;
                    if (targetX >= 0 && targetX < mapWidth &&
                        targetY >= 0 && targetY < mapWidth)
                    {
                        noiseMap[targetX, targetY] = 0;
                    }
                }
            }
        }
    }

    private string GenerateClusterName()
    {
        return GameManager.gameManager.nameManager.GetName();
    }
}
