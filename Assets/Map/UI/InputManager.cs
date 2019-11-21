using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Text SelectedObjectNameText;

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
            Camera.main.GetComponent<CameraController>().SetCameraTargetPosition(0, Vector3.zero);
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
}
