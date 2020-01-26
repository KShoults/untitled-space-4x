using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static ContractSystem contractSystem;
    public static int turnCounter = 1;
    public NameManager nameManager;
    public Text TurnCounterText;
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
            contractSystem = new ContractSystem();
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
        contractSystem.EvaluateContractSystem();

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
        BasicIndustry energyIndustry = new BasicIndustry(Resource.Energy);
        homeworld.producers.Add(Resource.Energy, energyIndustry);
        energyIndustry.tiles.Add(tiles[0]);
        tiles[0].tileOccupier = energyIndustry;
        energyIndustry.tileDevelopments.Add(tiles[0], 3);
        energyIndustry.population += 3 * Employer.POPTODEVRATIO;

        BasicIndustry waterIndustry = new BasicIndustry(Resource.Water);
        homeworld.producers.Add(Resource.Water, waterIndustry);
        waterIndustry.tiles.Add(tiles[1]);
        tiles[1].tileOccupier = waterIndustry;
        waterIndustry.tileDevelopments.Add(tiles[1], 3);
        waterIndustry.population += 3 * Employer.POPTODEVRATIO;

        BasicIndustry foodIndustry = new BasicIndustry(Resource.Food);
        homeworld.producers.Add(Resource.Food, foodIndustry);
        foodIndustry.tiles.Add(tiles[2]);
        tiles[2].tileOccupier = foodIndustry;
        foodIndustry.tileDevelopments.Add(tiles[2], 3);
        foodIndustry.population += 3 * Employer.POPTODEVRATIO;

        BasicIndustry mineralsIndustry = new BasicIndustry(Resource.Minerals);
        homeworld.producers.Add(Resource.Minerals, mineralsIndustry);
        mineralsIndustry.tiles.Add(tiles[3]);
        tiles[3].tileOccupier = mineralsIndustry;
        mineralsIndustry.tileDevelopments.Add(tiles[3], 34);
        mineralsIndustry.population += 34 * Employer.POPTODEVRATIO;

        AdvancedIndustry civilianIndustry = new AdvancedIndustry(Resource.CivilianGoods);
        homeworld.producers.Add(Resource.CivilianGoods, civilianIndustry);
        civilianIndustry.tiles.Add(tiles[4]);
        tiles[4].tileOccupier = civilianIndustry;
        civilianIndustry.tileDevelopments.Add(tiles[4], 20);
        civilianIndustry.population += 20 * Employer.POPTODEVRATIO;

        AdvancedIndustry shipyard = new AdvancedIndustry(Resource.ShipParts);
        homeworld.producers.Add(Resource.ShipParts, shipyard);
        shipyard.tiles.Add(tiles[5]);
        tiles[5].tileOccupier = shipyard;

        TransportHub transportHub = new TransportHub();
        homeworld.producers.Add(Resource.TransportCapacity, transportHub);
        transportHub.tiles.Add(tiles[6]);
        tiles[6].tileOccupier = transportHub;
        transportHub.tileDevelopments.Add(tiles[6], 5);
        transportHub.population += 5 * Employer.POPTODEVRATIO;

        // Add starting stockpiles to the transport hub
        transportHub.stockpile[Resource.Energy] = 1;
        transportHub.stockpile[Resource.Water] = 1;
        transportHub.stockpile[Resource.Food] = 1;
        transportHub.stockpile[Resource.Minerals] = 8;
        transportHub.stockpile[Resource.CivilianGoods] = 1;
        transportHub.stockpile[Resource.ShipParts] = 1;

        /*TransportHub economy = new Economy();
        homeworld.developments.Add(Resource.Economy, economy);
        economy.tiles.Add(tiles[7]);
        tiles[7].development = economy;
        economy.tileDevelopments.Add(tiles[7], 20);
        economy.population += 20 * Development.POPTODEVRATIO;*/

        // Add a palace for dev purposes
        homeworld.palace = new Palace();

        // Center the view on the home system
        Camera.main.GetComponent<ViewController>().SetCameraTarget(View.System, homeSystem);
    }
}
