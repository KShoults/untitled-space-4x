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
        GrowIndustries();

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

        // Assign those tiles to industries
        homeworld.industries.Add(Resource.Energy, new Industry(Resource.Energy));
        Industry energyIndustry = homeworld.industries[Resource.Energy];
        energyIndustry.tiles.Add(tiles[0]);
        tiles[0].industry = energyIndustry;

        homeworld.industries.Add(Resource.Water, new Industry(Resource.Water));
        Industry waterIndustry = homeworld.industries[Resource.Water];
        waterIndustry.tiles.Add(tiles[1]);
        tiles[1].industry = waterIndustry;

        homeworld.industries.Add(Resource.Food, new Industry(Resource.Food));
        Industry foodIndustry = homeworld.industries[Resource.Food];
        foodIndustry.tiles.Add(tiles[2]);
        tiles[2].industry = foodIndustry;

        homeworld.industries.Add(Resource.Minerals, new Industry(Resource.Minerals));
        Industry mineralsIndustry = homeworld.industries[Resource.Minerals];
        mineralsIndustry.tiles.Add(tiles[3]);
        tiles[3].industry = mineralsIndustry;

        homeworld.industries.Add(Resource.CivilianGoods, new Industry(Resource.CivilianGoods));
        Industry civilianIndustry = homeworld.industries[Resource.CivilianGoods];
        civilianIndustry.tiles.Add(tiles[4]);
        tiles[4].industry = civilianIndustry;

        homeworld.industries.Add(Resource.ShipParts, new Industry(Resource.ShipParts));
        Industry shipyard = homeworld.industries[Resource.ShipParts];
        shipyard.tiles.Add(tiles[5]);
        tiles[5].industry = shipyard;

        // Add a palace for dev purposes
        homeworld.palace = new Palace();

        // Center the view on the home system
        Camera.main.GetComponent<ViewController>().SetCameraTarget(View.System, homeSystem);
    }

    private void GrowIndustries()
    {
        foreach (Cluster c in sector.clusters)
        {
            foreach (StarSystem s in c.starSystems)
            {
                foreach (Planet p in s.planets)
                {
                    foreach (KeyValuePair<Resource, Industry> kvp in p.industries)
                    {
                        kvp.Value.Grow();
                    }
                }
            }
        }
    }
}
