using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverlayObject : MonoBehaviour
{
    // Activate this object if it passes a check based on the current view object.
    public virtual void Initialize(MonoBehaviour viewObject)
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
        gameObject.SetActive(true);
    }
}
