using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Text SelectedObjectNameText, LabelViewText;
    public PlanetPanel planetPanel;
    private ViewController viewController;
    private MonoBehaviour viewObject;

    // Start is called before the first frame update
    void Start()
    {
        viewController = GetComponent<ViewController>();
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

            switch (viewController.view)
            {
                default:
                case View.Sector:
                    break;
                case View.Cluster:
                    viewController.SetCameraTargetSmooth(View.Sector, null);
                    break;
                case View.System:
                    if (viewObject != null)
                    {
                        viewController.SetCameraTargetSmooth(View.Cluster, viewObject.GetComponentInParent<Cluster>());
                    }
                    break;
                case View.Region:
                case View.Planet:
                    if (viewObject != null)
                    {
                        viewController.SetCameraTargetSmooth(View.System, viewObject.GetComponentInParent<StarSystem>());
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
    public void ChangeView(View newView, MonoBehaviour o)
    {
        switch (newView)
        {
            default:
            case View.Sector:
                LabelViewText.text = "";
                LabelViewText.gameObject.SetActive(false);
                break;

            case View.Cluster:
                Cluster cluster = o as Cluster;
                LabelViewText.text = cluster.clusterName;
                LabelViewText.gameObject.SetActive(true);
                //planetPanel.gameObject.SetActive(false);
                break;

            case View.System:
                StarSystem starSystem = o as StarSystem;
                LabelViewText.text = starSystem.starSystemName;
                LabelViewText.gameObject.SetActive(true);
                break;

            case View.Region:
                Region region = o as Region;
                //LabelViewText.text = region.regionName;
                LabelViewText.gameObject.SetActive(false);
                break;
            
            case View.Planet:
                Planet planet = o as Planet;
                LabelViewText.text = planet.planetName;
                LabelViewText.gameObject.SetActive(true);
                break;
        }
        viewObject = o;
    }

    // Triggers the UI changes that should occur when a planet is selected
    public void SelectPlanet(Planet planet)
    {
        //planetPanel.gameObject.SetActive(true);
        //planetPanel.SelectPlanet(planet);
    }
}
