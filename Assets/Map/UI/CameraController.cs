using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int numClusters, clusterSpacing;

    // Start is called before the first frame update
    void Start()
    {
        numClusters = GameManager.gameManager.numClusters;
        clusterSpacing = GameManager.gameManager.clusterSpacing;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Set the camera to one of three preset positions
    /  position: preset position to move the camera to; 0 = Sector View, 1 = Cluster View, 2 = System View
    */ 
    public void SetCameraPosition(int position)
    {
        Vector3 newPosition = Vector3.zero;
        newPosition.z = -10;
        int newSize = 0;

        switch (position)
        {
            default:
            case 0:     // Sector View
                newPosition.x = numClusters * clusterSpacing;
                newPosition.y = numClusters * clusterSpacing;
                newSize = numClusters * clusterSpacing;
                break;

            case 1:     // Cluster View
                break;

            case 2:     // System View
                break;
        }

        transform.position = newPosition;
        Camera.main.orthographicSize = newSize;
    }
}
