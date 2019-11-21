﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Text SelectedObjectNameText, LabelViewText;

    // Start is called before the first frame update
    void Start()
    {

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
            Camera.main.GetComponent<CameraController>().SetCameraTargetSmooth(0, null);
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
                break;

            case 2:
                break;
        }
    }
}
