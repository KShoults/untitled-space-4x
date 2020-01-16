using System.Collections;
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
    public StarSystem homeSystem;

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
                nameSeed = Random.value;
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

    public void EndTurn()
    {
        GrowDevelopments();

        turnCounter++;
        TurnCounterText.text = "Turn: " + turnCounter;

        Camera.main.GetComponent<InputManager>().UpdateOverlay();
    }

    private void CreateHomeworld()
    {
        // Find a random planet in the home system to designate as the home planet
        Planet homeworld = homeSystem.planets[(int)Mathf.Floor(Random.value * homeSystem.planets.Count)];

        // Overwrite its size, habitability, and resources
        homeworld.planetSize = 8;
        homeworld.habitability = 90;

        // Set the homeworld's tiles
        Tile[] tiles = new Tile[8];
        tiles[0] = new Tile(Resource.Energy, Yield.Medium);
        tiles[1] = new Tile(Resource.Water, Yield.Medium);
        tiles[2] = new Tile(Resource.Food, Yield.Medium);
        tiles[3] = new Tile(Resource.Minerals, Yield.Medium);
        tiles[4] = new Tile(Resource.Energy, Yield.Low);
        tiles[5] = new Tile(Resource.Water, Yield.Low);
        tiles[6] = new Tile(Resource.Food, Yield.Low);
        tiles[7] = new Tile(Resource.Minerals, Yield.Low);

        homeworld.tiles = tiles;

        // Assign those tiles to developments
        Industry energyIndustry = new Industry(Resource.Energy);
        homeworld.developments.Add(Resource.Energy, energyIndustry);
        energyIndustry.tiles.Add(tiles[0]);
        tiles[0].development = energyIndustry;

        Industry waterIndustry = new Industry(Resource.Water);
        homeworld.developments.Add(Resource.Water, waterIndustry);
        waterIndustry.tiles.Add(tiles[1]);
        tiles[1].development = waterIndustry;

        Industry foodIndustry = new Industry(Resource.Food);
        homeworld.developments.Add(Resource.Food, foodIndustry);
        foodIndustry.tiles.Add(tiles[2]);
        tiles[2].development = foodIndustry;

        Industry mineralsIndustry = new Industry(Resource.Minerals);
        homeworld.developments.Add(Resource.Minerals, mineralsIndustry);
        mineralsIndustry.tiles.Add(tiles[3]);
        tiles[3].development = mineralsIndustry;

        Industry civilianIndustry = new Industry(Resource.CivilianGoods);
        homeworld.developments.Add(Resource.CivilianGoods, civilianIndustry);
        civilianIndustry.tiles.Add(tiles[4]);
        tiles[4].development = civilianIndustry;

        Industry shipyard = new Industry(Resource.ShipParts);
        homeworld.developments.Add(Resource.ShipParts, shipyard);
        shipyard.tiles.Add(tiles[5]);
        tiles[5].development = shipyard;

        // Add a palace for dev purposes
        homeworld.palace = new Palace();

        // Center the view on the home system
        Camera.main.GetComponent<ViewController>().SetCameraTarget(View.System, homeSystem);
    }

    private void GrowDevelopments()
    {
        foreach (Cluster c in sector.clusters)
        {
            foreach (StarSystem s in c.starSystems)
            {
                foreach (Planet p in s.planets)
                {
                    foreach (KeyValuePair<Resource, Development> kvp in p.developments)
                    {
                        kvp.Value.Grow();
                    }
                }
            }
        }
    }
}
