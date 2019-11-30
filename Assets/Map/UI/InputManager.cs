using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Text SelectedObjectNameText, LabelViewText;
    // Planet Panel Texts
    public Text PlanetNameText, PlanetSizeText;
    // Resource Panel Texts
    public Text EnergyText, WaterText, FoodText, MineralText;
    // Dictionary for quick lookup of resourceTexts
    public Dictionary<Resource, Text> resourceTexts;
    public RectTransform PlanetPanel;
    private CameraController cameraController;
    private MonoBehaviour viewObject;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponent<CameraController>();
        InitializeResourceTexts();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseButtons();
    }

    public void HandleMouseButtons()
    {
        if (Input.GetMouseButtonDown(1))
        {

            switch (cameraController.viewType)
            {
                default:
                case 0:
                    break;
                case 1:
                    GetComponent<CameraController>().SetCameraTargetSmooth(0, null);
                    break;
                case 2:
                    if (viewObject != null)
                    {
                        GetComponent<CameraController>().SetCameraTargetSmooth(1, viewObject.GetComponentInParent<Cluster>());
                    }
                    break;
            }
        }
    }

    public void HoverEnter(MonoBehaviour o)
    {
        if (o is Cluster)
        {
            Cluster cluster = o as Cluster;
            SelectedObjectNameText.text = cluster.clusterName;
            Vector3 worldPosition = cluster.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            SelectedObjectNameText.transform.position = new Vector3 (screenPosition.x, screenPosition.y + 40, 0);
            SelectedObjectNameText.gameObject.SetActive(true);
        }

        else if (o is StarSystem)
        {
            StarSystem starSystem = o as StarSystem;
            SelectedObjectNameText.text = starSystem.starSystemName;
            Vector3 worldPosition = starSystem.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            SelectedObjectNameText.transform.position = new Vector3 (screenPosition.x, screenPosition.y + 20, 0);
            SelectedObjectNameText.gameObject.SetActive(true);
        }

        else if (o is Planet)
        {
            Planet planet = o as Planet;
            SelectedObjectNameText.text = planet.planetName;
            Vector3 worldPosition = planet.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            SelectedObjectNameText.transform.position = new Vector3 (screenPosition.x, screenPosition.y + 20, 0);
            SelectedObjectNameText.gameObject.SetActive(true);
        }
    }

    public void HoverExit(MonoBehaviour o)
    {
        SelectedObjectNameText.text = "";
        SelectedObjectNameText.gameObject.SetActive(false);
    }

    // Triggers all of the UI changes that should occur when the view changes
    // viewType refers to Sector/Cluster/System view.
    // o is the cluster or system we are viewing or null for sector view.
    public void ChangeView(int viewType, MonoBehaviour o)
    {
        switch (viewType)
        {
            default:
            case 0:
                LabelViewText.text = "";
                LabelViewText.gameObject.SetActive(false);
                break;

            case 1:
                Cluster cluster = o as Cluster;
                LabelViewText.text = cluster.clusterName;
                LabelViewText.gameObject.SetActive(true);
                PlanetPanel.gameObject.SetActive(false);
                break;

            case 2:
                StarSystem starSystem = o as StarSystem;
                LabelViewText.text = starSystem.starSystemName;
                LabelViewText.gameObject.SetActive(true);
                break;
        }
        viewObject = o;
    }

    // Triggers the UI changes that should occur when a planet is selected
    public void SelectPlanet(Planet planet)
    {
        PlanetPanel.gameObject.SetActive(true);
        PlanetNameText.text = planet.planetName;
        PlanetSizeText.text = "Size: " + planet.planetSize;
        // Set the resource panel texts
        Dictionary<Resource, string> resourceStrings = ResourceUtil.ResourceString;
        foreach (KeyValuePair<Resource, float> kvp in planet.resources)
        {
            if (kvp.Value == 0)
            {
                resourceTexts[kvp.Key].gameObject.SetActive(false);
            }
            else
            {
                resourceTexts[kvp.Key].gameObject.SetActive(true);
                resourceTexts[kvp.Key].text = resourceStrings[kvp.Key] + ": " + (int)Mathf.Floor(kvp.Value);
            }
        }
    }

    private void InitializeResourceTexts()
    {
        resourceTexts = new Dictionary<Resource, Text>
        {
            {Resource.Energy, EnergyText},
            {Resource.Water, WaterText},
            {Resource.Food, FoodText},
            {Resource.Minerals, MineralText}
        };
    }
}
