using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Cluster : MonoBehaviour
{
    public string clusterName;
    public GameObject StarSystemPrefab, ClusterStarPrefab;
    // The star systems in this cluster
    public List<StarSystem> starSystems;
    // The SpriteRenderers used to show this cluster in the sector view
    private List<SpriteRenderer> clusterStars;
    // Ordered list of names used for additional starSystems
    private List<string> starSystemNames;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCluster(int numSystemsAvg, float numSystemsVar, int sizePlanetsAvg, int sizePlanetsVar)
    {
        // Determine the number of systems in this cluster
        int numSystems = numSystemsAvg + (int) Mathf.Round((Random.value * numSystemsVar * 2) - numSystemsVar);

        // Generate an unweighted noise map with a standard size
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



        starSystems = new List<StarSystem>();
        clusterStars = new List<SpriteRenderer>();
        // For each system, find the largest point in the noise map and place a system
        for (int system = 0; system < numSystems; system++)
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

            // Convert the noise map position to world coordinates
            float xPosition = transform.position.x + (bestX - mapWidth / 2f) / 100f;
            float yPosition = transform.position.y + (bestY - mapWidth / 2f) / 100f;

            // Create and add the system
            StarSystem newSystem = GameObject.Instantiate(StarSystemPrefab).GetComponent<StarSystem>();
            newSystem.transform.parent = transform;
            newSystem.transform.position = new Vector3(xPosition, yPosition, 0);
            newSystem.starSystemName = GenerateSystemName();
            newSystem.GenerateStarSystem(sizePlanetsAvg, sizePlanetsVar);
            starSystems.Add(newSystem);

            // Create and add a sprite to this cluster for the sector view
            xPosition = transform.position.x + (bestX - mapWidth / 2f) / 25f;
            yPosition = transform.position.y + (bestY - mapWidth / 2f) / 25f;

            SpriteRenderer newSprite = GameObject.Instantiate(ClusterStarPrefab).GetComponent<SpriteRenderer>();
            newSprite.transform.parent = transform;
            newSprite.transform.position = new Vector3(xPosition, yPosition, 0);
            clusterStars.Add(newSprite);

            // Prevent other systems from spawning too close
            int systemSpacing = (int) Mathf.Floor((float)mapWidth / (float)numSystems / 2f);
            for (int i = 0-systemSpacing; i < systemSpacing; i++)
            {
                for (int j = 0-systemSpacing; j < systemSpacing; j++)
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

    // Alert the InputManager when this is hovered over
    void OnMouseEnter()
    {
        Camera.main.GetComponent<InputManager>().HoverEnter(this);
    }

    void OnMouseExit()
    {
        Camera.main.GetComponent<InputManager>().HoverExit(this);
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            Camera.main.GetComponent<CameraController>().SetCameraTargetSmooth(1, this);
        }
    }

    private string GenerateSystemName()
    {
        // Create or refill the list if it is empty
        if (starSystemNames == null || starSystemNames.Count == 0)
        {
            starSystemNames = GenerateNameList();
        }

        string name = starSystemNames[0] + " " + clusterName;
        starSystemNames.RemoveAt(0);
        return name;
    }

    private List<string> GenerateNameList()
    {
        List<string> newNameList = new List<string>();
        using (StreamReader sr = File.OpenText("Assets/Map/NameLists/StarSystemList.txt"))
        {
            string name;
            while ((name = sr.ReadLine()) != null)
            {
                newNameList.Add(name);
            }
        }
        return newNameList;
    }
}
