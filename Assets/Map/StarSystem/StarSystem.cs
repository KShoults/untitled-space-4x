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
    // The list contains a sequence of orbital regions
    public List<Region[]> orbitalRegions;
    public List<Region> planetaryRegions;
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
        orbitalRegions = new List<Region[]>();
        AddOrbitalSequence(ref orbitalRegions);
        AddOrbitalSequence(ref orbitalRegions);
        AddOrbitalSequence(ref orbitalRegions);
        /*AddOrbitalSequence(ref orbitalRegions);
        AddOrbitalSequence(ref orbitalRegions);
        AddOrbitalSequence(ref orbitalRegions);*/

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

            // Remember to convert back to Unity units
            newPlanet.transform.position = transform.position + (orbitalDirection * orbitalDistance / 20);
            planets.Add(newPlanet);

            // Add an orbital region
            //Region orbitalRegion = Instantiate(RegionPrefab).GetComponent<Region>();
            //orbitalRegion.transform.parent = transform;
            //orbitalRegion.transform.position = transform.position;
            //orbitalRegion.transform.localScale = new Vector3(orbitalDistance / 10, orbitalDistance / 10, orbitalDistance / 10);

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

    private void AddOrbitalSequence(ref List<Region[]> regions)
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

    // Returns the region mesh for a region in the specified orbital sequence
    private Mesh GenerateRegionMesh(int orbitalSequence)
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
}
