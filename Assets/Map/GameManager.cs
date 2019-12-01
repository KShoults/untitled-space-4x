﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public NameManager nameManager;
    public Text TurnCounterText;
    public int turnCounter;
    // Galaxy generation default settings
    public int numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar;
    public Sector sector;
    public float nameSeed = 0;

    void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManager = this;
            if (nameSeed == 0f)
            {
                float nameSeed = Random.value;
            }
            nameManager = new NameManager(nameSeed);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create the sector using perlin noise maps
        sector.GenerateSector(numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar);
        CreateHomeworld();
        TurnCounterText.text = "Turn: " + turnCounter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndTurn()
    {
        turnCounter++;
        TurnCounterText.text = "Turn: " + turnCounter;
    }

    private void CreateHomeworld()
    {
        // Find a random planet to designate the home planet
        int homeCluster = (int)Mathf.Floor(Random.value * numClusters);
        int homeSystem = (int)Mathf.Floor(Random.value * sector.clusters[homeCluster].starSystems.Count);
        int homePlanet = (int)Mathf.Floor(Random.value * sector.clusters[homeCluster].starSystems[homeSystem].planets.Count);

        Planet homeworld = sector.clusters[homeCluster].starSystems[homeSystem].planets[homePlanet];

        // Overwrite its size, habitability, and resources
        homeworld.planetSize = 50;
        homeworld.habitability = 90;

        Dictionary<Resource, float> resources = new Dictionary<Resource, float>();
        resources.Add(Resource.Energy, 10f);
        resources.Add(Resource.Water, 20f);
        resources.Add(Resource.Food, 20f);
        resources.Add(Resource.Minerals, 20f);

        homeworld.resources = resources;
        homeworld.mineralQuality = 0;

        Debug.Log("Homeworld: " + homeworld.planetName);
    }
}
