using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    public MonoBehaviour orbitalObject;
    public int q, r, s;
    
    // Alert the InputManager when this is clicked on
    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            Camera.main.GetComponent<ViewController>().SetCameraTargetSmooth(View.Region, this);
        }
    }
}
