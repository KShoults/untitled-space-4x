using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    // The region that contains this planet
    public Region parentRegion;
    // Planet size is an integer from 1 - 10;
    public int planetSize;
    // An array of the tiles on the planet
    public Tile[] tiles;
    // The habitability is from 1 - 100
    public int habitability;
    public Dictionary<Resource, Industry> industries;

    void Awake()
    {
        industries = new Dictionary<Resource, Industry>();
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
}
