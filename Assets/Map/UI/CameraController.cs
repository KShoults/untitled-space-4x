using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float TransitionSpeed;
    private int numClusters, clusterSpacing;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private float targetSize;
    private int targetLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        numClusters = GameManager.gameManager.numClusters;
        clusterSpacing = GameManager.gameManager.clusterSpacing;
        mainCamera = GetComponent<Camera>();
        // Set the starting camera position
        SetCameraPosition(0, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) >= .1f ||
            mainCamera.orthographicSize - targetSize >= .1f)
        {
            MoveTowardTargetPosition();
        }
    }

    /* Set the camera to one of three preset positions without a transition
    /  position: preset position to move the camera to; 0 = Sector View, 1 = Cluster View, 2 = System View
    */ 
    public void SetCameraPosition(int position, Vector3 objectPosition)
    {
        Vector3 newPosition = Vector3.zero;
        newPosition.z = -10;
        float newSize = 0;
        int newLayerMask = 0;

        switch (position)
        {
            default:
            case 0:     // Sector View
                newPosition.x = numClusters * clusterSpacing;
                newPosition.y = numClusters * clusterSpacing;
                newSize = numClusters * clusterSpacing * 1.1f;
                newLayerMask = 1 << 9;
                break;

            case 1:     // Cluster View
                newPosition.x = objectPosition.x;
                newPosition.y = objectPosition.y;
                newSize = 5;
                newLayerMask = 1 << 8;
                break;

            case 2:     // System View
                break;
        }

        newLayerMask = ~newLayerMask;
        mainCamera.cullingMask = newLayerMask;
        transform.position = newPosition;
        targetPosition = newPosition;
        mainCamera.orthographicSize = newSize;
        targetSize = newSize;
    }

    /* Set the camera target position to one of three preset positions.
    /  The camera will move smoothly to the target position.
    /  position: preset position to move the camera to; 0 = Sector View, 1 = Cluster View, 2 = System View
    */ 
    public void SetCameraTargetPosition(int position, Vector3 objectPosition)
    {
        targetPosition.z = -10;

        switch (position)
        {
            default:
            case 0:     // Sector View
                targetPosition.x = numClusters * clusterSpacing;
                targetPosition.y = numClusters * clusterSpacing;
                targetSize = numClusters * clusterSpacing * 1.1f;
                targetLayerMask = 1 << 9;
                break;

            case 1:     // Cluster View
                targetPosition.x = objectPosition.x;
                targetPosition.y = objectPosition.y;
                targetSize = 5;
                targetLayerMask = 1 << 8;
                break;

            case 2:     // System View
                break;
        }

        targetLayerMask = ~targetLayerMask;

        // When zooming in the mask should be applied immediately
        if (targetSize < mainCamera.orthographicSize)
        {
            mainCamera.cullingMask = targetLayerMask;
        }
    }

    private void MoveTowardTargetPosition()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, TransitionSpeed / 100f);
        transform.position = newPosition;
        float newSize =  mainCamera.orthographicSize + ((targetSize - mainCamera.orthographicSize) * (TransitionSpeed / 100f));
        mainCamera.orthographicSize = newSize;
        
        // When zooming out the mask should be applied when finished.
        if (targetSize - mainCamera.orthographicSize < 1)
        {
            mainCamera.cullingMask = targetLayerMask;
        }
    }
}
