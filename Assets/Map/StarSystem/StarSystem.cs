using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StarSystem : MonoBehaviour
{
    // The position of this StarSystem relative to its parent cluster
    public Vector2 position;
    public string starSystemName;
    public GameObject StarPrefab, PlanetPrefab, RegionPrefab;
    public Star star;
    public List<Planet> planets;
    // The region map in axial coordinates (q, r)
    public Region[,] regions;
    private List<string> planetNames;
    // A buffered region mesh to prevent generating the mesh extra times
    private static Mesh regionMesh;
    // Useful constants for axial coordinate conversion
    private Vector2 qVector = new Vector2(Mathf.Sqrt(3), 0);
    private Vector2 rVector = new Vector2(Mathf.Sqrt(3)/2f, 3f/2f);
    // Axial coordinate directions
    private Tuple<int,int>[] axialDirections = new Tuple<int, int>[6] {Tuple.Create(1, 0), Tuple.Create(1, -1), Tuple.Create(0, -1),
                                                Tuple.Create(-1, 0), Tuple.Create(-1, 1), Tuple.Create(0, 1)};

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
        int systemSize = sizePlanetsAvg + (int) Mathf.Round((UnityEngine.Random.value * sizePlanetsVar * 2) - sizePlanetsVar);

        // Create and add the planets
        planets = new List<Planet>();
        int currentSystemSize = 0;

        GenerateRegionMap();

        // Generate a noise map with zero weights where we don't have regions
        int mapWidth = regions.GetLength(0);
        float[,] weightMap = new float[mapWidth, mapWidth];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (regions[i,j] == null)
                {
                    weightMap[i,j] = 0;
                }
                else
                {
                    weightMap[i,j] = 1f;
                }
            }
        }
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(weightMap, 1);

        // Planets are created until the systemSize is reached
        while (currentSystemSize < systemSize)
        {
            // Find the largest point in the noise map to place a planet
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

            // Find the associated region
            Region targetRegion = regions[bestX, bestY];

            // Clear the target position and adjacent positions in the noise map
            noiseMap[bestX, bestY] = 0;
            for (int i = 0; i < axialDirections.Length; i++)
            {
                // Find the neighbor in region coordinates
                int qToClear = targetRegion.q + axialDirections[i].Item1;
                int rToClear = targetRegion.r + axialDirections[i].Item2;
                // Convert to noisemap coordinates
                int xToClear = qToClear + (mapWidth-1) / 2;
                int yToClear = rToClear + (mapWidth-1) / 2;

                if (xToClear >= 0 && xToClear < mapWidth &&
                    yToClear >= 0 && yToClear < mapWidth)
                {
                    noiseMap[xToClear, yToClear] = 0;
                }
            }

            // Generate the planet's size
            int planetSize = (int)Mathf.Floor(UnityEngine.Random.value * 99 + 1);

            Planet newPlanet = Instantiate(PlanetPrefab).GetComponent<Planet>();
            newPlanet.transform.parent = targetRegion.transform;
            newPlanet.planetName = GeneratePlanetName();
            newPlanet.planetSize = planetSize;
            newPlanet.parentRegion = targetRegion;

            Vector3 planetPosition = targetRegion.transform.position;
            planetPosition.z = 1;

            newPlanet.transform.position = planetPosition;
            planets.Add(newPlanet);

            // Generate planet resources
            Dictionary<Resource, float> planetResources = new Dictionary<Resource, float>();
            // Generate a random value for each resource
            for (int i = 0; i < 4; i++)
            {
                planetResources[(Resource)i] = UnityEngine.Random.value;
            }
            
            // Modify each resource by the star class
            Dictionary<Resource, int> classResources = StarClassUtil.StarResources[star.starClass];
            foreach (KeyValuePair<Resource, int> kvp in classResources)
            {
                planetResources[kvp.Key] *= kvp.Value;
            }
            // Increase the mineral amounts to bring it to the same level as the rest
            planetResources[Resource.Minerals] *= 10;

            // Zero out any resources that are below a certain threshold
            for (int i = 0; i < 4; i++)
            {
                if (planetResources[(Resource)i] < 2.5f)
                {
                    planetResources[(Resource)i] = 0;
                }
            }

            // Calculate the highest maximum possible size multiplier between all resources
            float maxMultiplier = planetSize / 8f;
            foreach (KeyValuePair<Resource, float> kvp in planetResources)
            {
                if (planetSize / kvp.Value < maxMultiplier)
                {
                    maxMultiplier = planetSize / kvp.Value;
                }
            }

            // Adjust resources for planet size
            for (int i = 0; i < 4; i++)
            {
                planetResources[(Resource)i] *= maxMultiplier;
                // Floor any values that are less than 1
                if (planetResources[(Resource)i] < 1)
                {
                    planetResources[(Resource)i] = Mathf.Floor(planetResources[(Resource)i]);
                } 
            }

            newPlanet.resources = planetResources;

            // Generate mineral quality
            if (planetResources[Resource.Minerals] > 0)
            {
                float mineralQuality = UnityEngine.Random.value;
                if (mineralQuality >= .5f)
                {
                    newPlanet.mineralQuality = 2;
                }
                else if (mineralQuality >= .15f)
                {
                    newPlanet.mineralQuality = 1;
                }
            }

            // Generate planet habitability
            float habitability = UnityEngine.Random.value;

            // Adjust for star class
            habitability *= StarClassUtil.StarHabitability[star.starClass];

            // Place this habitability on a curve that makes high habitability rare
            // The curve is the function (1.25x)^4 / 100
            habitability = Mathf.Pow(1.25f * habitability, 4) / 100f;

            // Minimum habitability should be 1
            if (habitability < 1)
            {
                habitability = 1;
            }

            newPlanet.habitability = (int)Mathf.Round(habitability);

            currentSystemSize += planetSize;
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
        float r = UnityEngine.Random.value * 7;
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

    // Generate the hexagonal region map
    private void GenerateRegionMap()
    {
        // The radius of the map in hexagonal tiles
        int mapSize = 7;
        regions = new Region[mapSize * 2 + 1, mapSize * 2 + 1];

        // We'll want to skip some regions to make the map the right shape
        List<Tuple<int, int>> regionsToSkip = new List<Tuple<int, int>>();

        // Leave the center region and every region adjacent null to give the star some room
        regionsToSkip.Add(Tuple.Create(0, 0));
        for (int i = 0; i < axialDirections.Length; i++)
        {
            regionsToSkip.Add(axialDirections[i]);
        }

        // Generate the map in axial coordinates
        for(int q = -mapSize; q <= mapSize; q++)
        {
            for (int r = -mapSize; r <= mapSize; r++)
            {
                if (!regionsToSkip.Contains(Tuple.Create(q, r)) &&
                    Mathf.Abs(q + r) <= mapSize) // Cut off the corners to make the map hexagonal
                {
                    Region newRegion = Instantiate(RegionPrefab).GetComponent<Region>();
                    newRegion.q = q;
                    newRegion.r = r;
                    newRegion.s = -q - r;

                    // Set the region's world position
                    newRegion.transform.parent = transform;
                    Vector3 regionPosition = transform.position;
                    regionPosition += (Vector3) (q * qVector) * .05f;
                    regionPosition += (Vector3) (r * rVector) * .05f;
                    regionPosition.z = 2;
                    newRegion.transform.position = regionPosition;

                    // Get the region's mesh
                    Mesh regionMesh = GenerateRegionMesh();
                    newRegion.GetComponent<MeshFilter>().mesh = regionMesh;
                    newRegion.GetComponent<MeshCollider>().sharedMesh = regionMesh;

                    regions[q + mapSize, r + mapSize] = newRegion;
                }
            }
        }
    }

    // Generate a hexagonal region mesh
    private Mesh GenerateRegionMesh()
    {
        // Check to see if the mesh has already been generated
        if (regionMesh == null)
        {
            Mesh newMesh = new Mesh();
            Vector3[] newVertices = new Vector3[7];
            Vector2[] newUV = new Vector2[7];
            int[] newTriangles = new int[6*3];

            float size = .5f;  // Half the width

            // Create the center vertex
            newVertices[0] = Vector3.zero;
            newUV[0] = new Vector2(.5f, .5f);

            // Create the hexagon
            for (int i = 0; i < 6; i++)
            {
                // Generate the vertex
                float angleDeg = 60 * i - 30;
                float angleRad = Mathf.PI / 180f * angleDeg;
                float xPos = size * Mathf.Cos(angleRad);
                float yPos = size * Mathf.Sin(angleRad);
                newVertices[i+1] = new Vector3(xPos, yPos, 0);

                // Generate the UV
                float uPos = .5f * Mathf.Cos(angleRad) + .5f;
                float vPos = .5f * Mathf.Sin(angleRad) + .5f;

                newUV[i+1] = new Vector2(uPos, vPos);
            }

            // Create the triangles
            for (int i = 0; i < 6; i++)
            {
                newTriangles[3 * i] = 0;
                newTriangles[3 * i + 1] = i + 2 < 7 ? i + 2 : 1; // Loop back around to 1
                newTriangles[3 * i + 2] = i + 1;
            }

            newMesh.vertices = newVertices;
            newMesh.uv = newUV;
            newMesh.triangles = newTriangles;
            regionMesh = newMesh;
        }
        
        return regionMesh;
    }
}
