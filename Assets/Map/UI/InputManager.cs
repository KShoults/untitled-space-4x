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
        
    }

    public void HoverEnter(MonoBehaviour o)
    {
        if (o is Cluster)
        {
            Cluster cluster = o as Cluster;
            SelectedObjectNameText.text = cluster.clusterName;
            Vector3 worldPosition = cluster.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            SelectedObjectNameText.transform.position = new Vector3 (screenPosition.x, screenPosition.y + 20, 0);
            SelectedObjectNameText.gameObject.SetActive(true);
        }
    }

    public void HoverExit(MonoBehaviour o)
    {
        if (o is Cluster)
        {
            SelectedObjectNameText.text = "";
            SelectedObjectNameText.gameObject.SetActive(false);
        }
    }
}
