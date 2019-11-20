using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    public string clusterName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Alert the InputManager when this is hovered over
    void OnMouseEnter ()
    {
        Camera.main.GetComponent<InputManager>().HoverEnter(this);
    }

    void OnMouseExit ()
    {
        Camera.main.GetComponent<InputManager>().HoverExit(this);
    }
}
