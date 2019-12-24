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
        // Find a random planet in the home system to designate as the home planet
        Planet homeworld = homeSystem.planets[(int)Mathf.Floor(Random.value * homeSystem.planets.Count)];

        // Overwrite its size, habitability, and resources
        homeworld.planetSize = 5;
        homeworld.habitability = 90;

        // Set the homeworld's tiles
        Tile[] tiles = new Tile[5];
        tiles[0] = new Tile(Resource.Minerals, Yield.Medium, Resource.Energy, Yield.Medium);
        tiles[1] = new Tile(Resource.Food, Yield.Medium, Resource.Water, Yield.Medium);
        tiles[2] = new Tile(Resource.Energy, Yield.Low);
        tiles[3] = new Tile(Resource.Water, Yield.Low);
        tiles[4] = new Tile(Resource.Food, Yield.Low);

        homeworld.tiles = tiles;

        // Center the view on the home system
        Camera.main.GetComponent<ViewController>().SetCameraTarget(View.System, homeSystem);
    }
}
