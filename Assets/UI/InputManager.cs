using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Text SelectedObjectNameText, LabelViewText, LabelViewSubText;
    public Dictionary<View, Dictionary<Overlay, List<OverlayObject>>> overlayLists;
    private Overlay activeOverlay;
    private View activeView;
    private ViewController viewController;
    private MonoBehaviour viewObject;

    void Awake()
    {
        InitializeOverlayLists();
    }

    // Start is called before the first frame update
    void Start()
    {
        viewController = GetComponent<ViewController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeOverlay == Overlay.None)
        {
            SetOverlay(activeView, Overlay.Development);
        }
        HandleMouseButtons();
    }

    public void HandleMouseButtons()
    {
        if (Input.GetMouseButtonDown(1))
        {
            switch (activeView)
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
                LabelViewText.gameObject.SetActive(false);
                LabelViewSubText.gameObject.SetActive(false);
                break;

            case View.Cluster:
                Cluster cluster = o as Cluster;
                LabelViewText.text = cluster.clusterName;
                LabelViewText.gameObject.SetActive(true);
                LabelViewSubText.gameObject.SetActive(false);
                break;

            case View.System:
                StarSystem starSystem = o as StarSystem;
                LabelViewText.text = starSystem.starSystemName;
                LabelViewText.gameObject.SetActive(true);
                LabelViewSubText.gameObject.SetActive(false);
                break;

            case View.Region:
                Region region = o as Region;
                if (region.orbitalObject != null)
                {
                    LabelViewText.text = ((Planet)region.orbitalObject).planetName;
                }
                else
                {
                    LabelViewText.text = region.GetComponentInParent<StarSystem>().starSystemName;
                }
                LabelViewText.gameObject.SetActive(true);
                LabelViewSubText.text = "(" + region.q + ", " + region.r + ", " + region.s + ")";
                LabelViewSubText.gameObject.SetActive(true);
                break;
        }
        viewObject = o;
        SetOverlay(newView, activeOverlay);
    }

    private void InitializeOverlayLists()
    {
        overlayLists = new Dictionary<View, Dictionary<Overlay, List<OverlayObject>>>();
        foreach(View view in Enum.GetValues(typeof(View)))
        {
            overlayLists.Add(view, new Dictionary<Overlay, List<OverlayObject>>());
            foreach(Overlay overlay in Enum.GetValues(typeof(Overlay)))
            {
                overlayLists[view].Add(overlay, new List<OverlayObject>());
            }
        }
    }
    
    private void SetOverlay(View view, Overlay overlay)
    {
        if (activeOverlay != Overlay.None)
        {
            // Deactivate any OverlayObjects in the old overlay
            foreach(OverlayObject o in overlayLists[activeView][activeOverlay])
            {
                o.gameObject.SetActive(false);
            }
        }

        if (overlay != Overlay.None)
        {
            // Activate any OverlayObjects in the new overlay
            foreach(OverlayObject o in overlayLists[view][overlay])
            {
                o.Initialize(viewObject);
            }
        }

        activeOverlay = overlay;
        activeView = view;
    }
}
