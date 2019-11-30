using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StarSystem : MonoBehaviour
{
    // The position of this StarSystem relative to its parent cluster
    public Vector2 position;
    public string starSystemName;
    public GameObject StarPrefab, PlanetPrefab, OrbitalRegionPrefab, PlanetaryRegionPrefab;
    public Star star;
    public List<Planet> planets;
    // The list contains a sequence of orbital regions
    public List<OrbitalRegion[]> orbitalRegions;
    public List<PlanetaryRegion> planetaryRegions;
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

        // Create the innermost orbital region sequence
        orbitalRegions = new List<OrbitalRegion[]>();
        AddOrbitalSequence(ref orbitalRegions);
        planetaryRegions = new List<PlanetaryRegion>();

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

            // Create planetary region
            PlanetaryRegion planetaryRegion = Instantiate(PlanetaryRegionPrefab).GetComponent<PlanetaryRegion>();
            planetaryRegion.transform.parent = transform.Find("RegionMap");
            // Determine which orbital region to start looking in to place the planetary region
            int desiredOrbitalSequence = (int)Mathf.Floor(orbitalDistance / 6);
            // Create the orbital sequence if necessary
            while (orbitalRegions.Count <= desiredOrbitalSequence)
            {
                AddOrbitalSequence(ref orbitalRegions);
            }
            // Generate the number of orbital regions to skip from 0 to the number of regions in that sequence
            int regionsToSkip = (int)Mathf.Floor(orbitalRegions[desiredOrbitalSequence].Length * Random.value);

            // Find an orbital region in which to place the planetary region
            int skippedRegions = 0;
            bool regionFound = false;
            int foundRegion = 0;
            do
            {
                for (int i = 0; i < orbitalRegions[desiredOrbitalSequence].Length; i++)
                {
                    if (orbitalRegions[desiredOrbitalSequence][i].planetaryRegion == null)
                    {
                        if (skippedRegions == regionsToSkip)
                        {
                            orbitalRegions[desiredOrbitalSequence][i].planetaryRegion = planetaryRegion;
                            regionFound = true;
                            foundRegion = i;
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
                AddOrbitalSequence(ref orbitalRegions);

                desiredOrbitalSequence++;
                for (int i = 0; i < orbitalRegions[desiredOrbitalSequence].Length; i++)
                {
                    if (orbitalRegions[desiredOrbitalSequence][i].planetaryRegion == null)
                    {
                        if (skippedRegions == regionsToSkip)
                        {
                            orbitalRegions[desiredOrbitalSequence][i].planetaryRegion = planetaryRegion;
                        }
                        else
                        {
                            skippedRegions++;
                        }
                    }
                }
            }

            // Generate the planetary region's mesh
            Mesh planetaryRegionMesh = GeneratePlanetaryRegionMesh();
            planetaryRegion.GetComponent<MeshFilter>().mesh = planetaryRegionMesh;
            planetaryRegion.GetComponent<MeshCollider>().sharedMesh = planetaryRegionMesh;

            // Set the region's position
            Vector3 regionPosition = transform.Find("RegionMap").position;
            float regionAngle = 360f * foundRegion / (float)orbitalRegions[desiredOrbitalSequence].Length;
            float r = (desiredOrbitalSequence * .2f) + .1f;
            regionPosition += Quaternion.AngleAxis(regionAngle, Vector3.back) * Vector3.up * r;
            regionPosition.z = 2;
            planetaryRegion.transform.position = regionPosition;

            planetaryRegions.Add(planetaryRegion);

            // Place the planet in its planetary region
            Vector3 planetPosition = regionPosition;
            planetPosition.z = 1;
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

    private void AddOrbitalSequence(ref List<OrbitalRegion[]> regions)
    {
        int numRegions = (int)Mathf.Pow(2, regions.Count+1);
        OrbitalRegion[] newSequence = new OrbitalRegion[numRegions];
        Transform regionMap = transform.Find("RegionMap");
        float sequenceAngleOffset = 360f / numRegions / 2f;
        for (int i = 0; i < numRegions; i++)
        {
            OrbitalRegion newRegion = Instantiate(OrbitalRegionPrefab).GetComponent<OrbitalRegion>();
            newRegion.transform.parent = regionMap;
            Vector3 regionPosition = transform.position;
            regionPosition.z = 3 + regions.Count;
            newRegion.transform.position = regionPosition;
            float regionAngle = 360f / numRegions;
            Quaternion regionRotation = Quaternion.AngleAxis(regionAngle * i + sequenceAngleOffset, Vector3.back);
            newRegion.transform.rotation = regionRotation;
            Mesh regionMesh = GenerateOrbitalRegionMesh(regions.Count+1);
            newRegion.GetComponent<MeshFilter>().mesh = regionMesh;
            newRegion.GetComponent<MeshCollider>().sharedMesh = regionMesh;
            newSequence[i] = newRegion;
        }
        regions.Add(newSequence);
    }

    // Returns the region mesh for a region in the specified orbital sequence
    private Mesh GenerateOrbitalRegionMesh(int orbitalSequence)
    {
        float r = 2 * orbitalSequence; // The outer curve radius
        int curveVertices = 20; // The number of vertices in the outer curve
        Mesh newMesh = new Mesh();
        Vector3[] newVertices = new Vector3[curveVertices + 2];
        Vector2[] newUV = new Vector2[curveVertices + 2];
        int[] newTriangles = new int[(curveVertices - 1) * 3];
        float angle = 360f / (float)(curveVertices - 1) / Mathf.Pow(2, orbitalSequence);

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

    private Mesh GeneratePlanetaryRegionMesh()
    {
        int curveVertices = 20;
        Mesh newMesh = new Mesh();
        Vector3[] newVertices = new Vector3[curveVertices + 1];
        Vector2[] newUV = new Vector2[curveVertices + 1];
        int[] newTriangles = new int[3 * curveVertices];
        float angle = 360f / (float)(curveVertices - 1);
        float r = .1f;

        // Create a circular mesh
        for (int i = 0; i < curveVertices; i++)
        {
            Vector3 vertex = Quaternion.AngleAxis(angle * i, Vector3.back) * Vector3.up;
            newVertices[i] = vertex * r;

            newUV[i] = new Vector2((vertex.x + 1 ) / 2f, (vertex.y + 1 ) / 2f);
        }

        // Create the center vertex
        newVertices[curveVertices] = Vector3.zero;
        newUV[curveVertices] = new Vector2(.5f, .5f);

        // Create the triangles
        for (int i = 0; i < curveVertices - 1; i++)
        {
            newTriangles[3 * i] = curveVertices; // The center vertex
            newTriangles[3 * i + 1] = i;
            newTriangles[3 * i + 2] = i + 1;
        }

        newMesh.vertices = newVertices;
        newMesh.uv = newUV;
        newMesh.triangles = newTriangles;
        return newMesh;
    }
}
