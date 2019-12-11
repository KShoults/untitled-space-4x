using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverlayObject : MonoBehaviour
{
    // Activate this object if it passes a check based on the current view object.
    public void Initialize(MonoBehaviour viewObject)
    {
        if (ShouldBeActive(viewObject))
        {
            gameObject.SetActive(true);
        }
    }

    // Registers this overlay object and then disables it so that it is ready to be called by input manager.
    protected void RegisterOverlayObject(View view, Overlay overlay)
    {
        Camera.main.GetComponent<InputManager>().overlayLists[view][overlay].Add(this);
        gameObject.SetActive(false);
    }

    protected abstract bool ShouldBeActive(MonoBehaviour viewObject);
}
