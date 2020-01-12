using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterSystemDevelopmentOverlay : MonoBehaviour
{
    public StarSystem starSystem;

    void OnEnable()
    {
        if (starSystem == null)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
