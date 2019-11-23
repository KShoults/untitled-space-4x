using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StarSystem : MonoBehaviour
{
    // The position of this StarSystem relative to its parent cluster
    public Vector2 position;
    public string starSystemName;
    public GameObject StarPrefab, PlanetPrefab;
    public Star star;
    public List<Planet> planets;
    private List<string> planetNames;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateStarSystem(int sizePlanetsAvg, int sizePlanetsVar)
    {
        // Create the star
        star = Instantiate(StarPrefab).GetComponent<Star>();
        star.transform.parent = transform;
        star.transform.position = transform.position;
        star.starName = starSystemName;
        star.starClass = GenerateStarClass();
        star.GetComponent<SpriteRenderer>().color = StarClassUtil.StarColor[star.starClass];
        GetComponent<SpriteRenderer>().color = StarClassUtil.StarColor[star.starClass];

        // Determine the total size of planets to add to the system
        int systemSize = sizePlanetsAvg + (int) Mathf.Round((Random.value * sizePlanetsVar * 2) - sizePlanetsVar);

        // Create and add the planets
        planets = new List<Planet>();
        int currentSystemSize = 0;
        // Planets are created from the inner orbit out until the systemSize is reached
        while (currentSystemSize < systemSize)
        {
            // Figure orbital distance in light minutes
            float orbitalDistance = 2 + (Random.value * 2);
            // Adjust outward for existing planets
            if (planets.Count > 0)
            {
                orbitalDistance = orbitalDistance + planets[planets.Count - 1].orbitalDistance;
            }

            Planet newPlanet = Instantiate(PlanetPrefab).GetComponent<Planet>();
            newPlanet.transform.parent = transform;
            newPlanet.orbitalDistance = orbitalDistance;
            newPlanet.planetName = GeneratePlanetName();
            // Figure out where to show the planet in its orbit
            Vector3 orbitalDirection = new Vector3(Random.value - .5f, Random.value - .5f, 0);
            orbitalDirection = orbitalDirection.normalized;
            // Remember to convert back to light years/Unity units
            newPlanet.transform.position = transform.position + (orbitalDirection * orbitalDistance / 20);
            planets.Add(newPlanet);
            currentSystemSize += 50;
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
            Camera.main.GetComponent<CameraController>().SetCameraTargetSmooth(2, this);
        }
    }

    private StarClass GenerateStarClass()
    {
        float r = Random.value * 7;
        if (r < 1)
        {
            return StarClass.O;
        }
        else if (r < 2)
        {
            return StarClass.B;
        }
        else if (r < 3)
        {
            return StarClass.A;
        }
        else if (r < 4)
        {
            return StarClass.F;
        }
        else if (r < 5)
        {
            return StarClass.G;
        }
        else if (r < 6)
        {
            return StarClass.K;
        }
        else
        {
            return StarClass.M;
        }
    }

    private string GeneratePlanetName()
    {
        // Create or refill the list if it is empty
        if (planetNames == null || planetNames.Count == 0)
        {
            planetNames = GenerateNameList();
        }

        string name = starSystemName + " " + planetNames[0];
        planetNames.RemoveAt(0);
        return name;
    }

    private List<string> GenerateNameList()
    {
        List<string> newNameList = new List<string>();
        using (StreamReader sr = File.OpenText("Assets/Map/NameLists/PlanetList.txt"))
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
