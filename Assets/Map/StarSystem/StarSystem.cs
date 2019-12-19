using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StarSystem : MonoBehaviour
{
    // The position of this StarSystem relative to its parent cluster
    public Vector2 position;
    public string starSystemName;
    public GameObject StarPrefab, PlanetPrefab, RegionPrefab;
    public Star star;
    public List<Planet> planets;
    // The list contains sequencea of regions
    public List<Region[]> regions;
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

        regions = new List<Region[]>();

        // Planets are created from the inner orbit out until the systemSize is reached
        while (currentSystemSize < systemSize)
        {
            // Generate the planet's size
            int planetSize = (int)Mathf.Floor(Random.value * 99 + 1);

            // Figure orbital distance in light minutes
            float orbitalDistance = 2 + (Random.value * 2);
            // Adjust outward for existing planets
            if (planets.Count > 0)
            {
                orbitalDistance += planets[planets.Count - 1].orbitalDistance;
            }

            Planet newPlanet = Instantiate(PlanetPrefab).GetComponent<Planet>();
            newPlanet.transform.parent = transform;
            newPlanet.orbitalDistance = orbitalDistance;
            newPlanet.planetName = GeneratePlanetName();
            newPlanet.planetSize = planetSize;

            // Determine which region sequence to start looking in to place the planet
            int desiredRegionSequence = (int)Mathf.Floor(orbitalDistance / 6);
            // Create the region sequence if necessary
            while (regions.Count <= desiredRegionSequence)
            {
                AddRegionSequence();
            }
            // Generate the number of regions to skip from 0 to the number of regions in that sequence
            int regionsToSkip = (int)Mathf.Floor(regions[desiredRegionSequence].Length * Random.value);

            // Find a region in which to place the planet
            int skippedRegions = 0;
            bool regionFound = false;
            int foundRegion = 0;
            do
            {
                for (int i = 0; i < regions[desiredRegionSequence].Length; i++)
                {
                    if (regions[desiredRegionSequence][i].orbitalObject == null)
                    {
                        if (skippedRegions == regionsToSkip)
                        {
                            regions[desiredRegionSequence][i].orbitalObject = newPlanet;
                            regionFound = true;
                            foundRegion = i;
                            break;
                        }
                        else
                        {
                            skippedRegions++;
                        }
                    }
                }
            } while (skippedRegions != 0 && regionFound == false);

            // Create a new sequence if the desired one was full
            if (regionFound == false)
            {
                AddRegionSequence();

                desiredRegionSequence++;
                for (int i = 0; i < regions[desiredRegionSequence].Length; i++)
                {
                    if (regions[desiredRegionSequence][i].orbitalObject == null)
                    {
                        if (skippedRegions == regionsToSkip)
                        {
                            regions[desiredRegionSequence][i].orbitalObject = newPlanet;
                        }
                        else
                        {
                            skippedRegions++;
                        }
                    }
                }
            }

            // Set the planet's position
            Vector3 planetPosition = transform.Find("RegionMap").position;
            float regionAngle = 360f * foundRegion / (float)regions[desiredRegionSequence].Length;
            float r = (desiredRegionSequence * .2f) + .1f;
            planetPosition += Quaternion.AngleAxis(regionAngle, Vector3.back) * Vector3.up * r;
            planetPosition.z = 2;

            newPlanet.transform.position = planetPosition;
            planets.Add(newPlanet);

            // Generate planet resources
            Dictionary<Resource, float> planetResources = new Dictionary<Resource, float>();
            // Generate a random value for each resource
            for (int i = 0; i < 4; i++)
            {
                planetResources[(Resource)i] = Random.value;
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
                float mineralQuality = Random.value;
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
            float habitability = Random.value;

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

    private void AddRegionSequence()
    {
        int numRegions = (int)Mathf.Pow(2, regions.Count+1);
        Region[] newSequence = new Region[numRegions];
        Transform regionMap = transform.Find("RegionMap");
        float sequenceAngleOffset = 360f / numRegions / 2f;
        for (int i = 0; i < numRegions; i++)
        {
            Region newRegion = Instantiate(RegionPrefab).GetComponent<Region>();
            newRegion.transform.parent = regionMap;
            Vector3 regionPosition = transform.position;
            regionPosition.z = 3 + regions.Count;
            newRegion.transform.position = regionPosition;
            float regionAngle = 360f / numRegions;
            Quaternion regionRotation = Quaternion.AngleAxis(regionAngle * i + sequenceAngleOffset, Vector3.back);
            newRegion.transform.rotation = regionRotation;
            Mesh regionMesh = GenerateRegionMesh(regions.Count+1);
            newRegion.GetComponent<MeshFilter>().mesh = regionMesh;
            newRegion.GetComponent<MeshCollider>().sharedMesh = regionMesh;
            newSequence[i] = newRegion;
        }
        regions.Add(newSequence);
    }

    // Returns the region mesh for a region in the specified region sequence
    private Mesh GenerateRegionMesh(int regionSequence)
    {
        float r = 2 * regionSequence; // The outer curve radius
        int curveVertices = 20; // The number of vertices in the outer curve
        Mesh newMesh = new Mesh();
        Vector3[] newVertices = new Vector3[curveVertices + 2];
        Vector2[] newUV = new Vector2[curveVertices + 2];
        int[] newTriangles = new int[(curveVertices - 1) * 3];
        float angle = 360f / (float)(curveVertices - 1) / Mathf.Pow(2, regionSequence);

        // Create the outer curve
        for (int i = 0; i < curveVertices; i++)
        {
            newVertices[i] = Quaternion.AngleAxis(angle * i, Vector3.back) * Vector3.up * r;
            newUV[i] = new Vector2(i / (float)(curveVertices - 1), 1);
        }

        // Create the bottom vertex
        newVertices[curveVertices] = Vector3.zero;
        newUV[curveVertices] = new Vector2(.5f, 0);

        // Create the triangles
        for (int i = 0; i < curveVertices - 1; i++)
        {
            newTriangles[3 * i] = curveVertices; // The bottom vertex
            newTriangles[3 * i + 1] = i;
            newTriangles[3 * i + 2] = i + 1;
        }

        newMesh.vertices = newVertices;
        newMesh.uv = newUV;
        newMesh.triangles = newTriangles;
        return newMesh;
    }
}
