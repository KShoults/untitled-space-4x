using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    // orbital distance is measured in light minutes
    public float orbitalDistance;
    // Planet size is an integer from 1 - 100;
    public int planetSize;
    // The units for resources are the same as planet size.
    public Dictionary<Resource, float> resources;
    // The mineral quality can be common:0, uncommon:1, rare:2
    public int mineralQuality;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    // Alert the InputManager whe this is clicked on

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            Camera.main.GetComponent<InputManager>().SelectPlanet(this);
        }
    }
}
