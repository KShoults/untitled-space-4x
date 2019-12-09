using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float TransitionSpeed;
    // Refers to Sector:0, Cluster:1, StarSystem:2
    public int viewType;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private float targetSize;
    private int targetLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) >= .1f ||
            mainCamera.orthographicSize - targetSize >= .1f * targetSize)
        {
            MoveTowardTargetPosition();
        }
    }

    /* Set the camera to one of three preset positions without a transition
    /  position: preset position to move the camera to; 0 = Sector View, 1 = Cluster View, 2 = System View
    */ 
    public void SetCameraTarget(int position, MonoBehaviour o)
    {
        // It's possible for this to be called before start by the GameManager
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        Vector3 objectPosition;
        Vector3 newPosition = Vector3.zero;
        newPosition.z = -10;
        float newSize = 0;
        int newLayerMask = 0;

        switch (position)
        {
            default:
            case 0:     // Sector View
                newPosition.x = 0;
                newPosition.y = 0;
                newSize = 550;
                newLayerMask = 1 << 8;
                break;

            case 1:     // Cluster View
                objectPosition = o.transform.position;
                newPosition.x = objectPosition.x;
                newPosition.y = objectPosition.y;
                newSize = 50;
                newLayerMask = 1 << 9;
                break;

            case 2:     // System View
                objectPosition = o.transform.position;
                newPosition.x = objectPosition.x;
                newPosition.y = objectPosition.y;
                newSize = 1;
                newLayerMask = 1 << 10;
                break;
        }
        mainCamera.cullingMask = newLayerMask;
        transform.position = newPosition;
        targetPosition = newPosition;
        mainCamera.orthographicSize = newSize;
        targetSize = newSize;
        GetComponent<InputManager>().ChangeView(position, o);
        viewType = position;
    }

    /* Set the camera target position to one of three preset positions.
    /  The camera will move smoothly to the target position.
    /  position: preset position to move the camera to; 0 = Sector View, 1 = Cluster View, 2 = System View
    */ 
    public void SetCameraTargetSmooth(int position, MonoBehaviour o)
    {
        Vector3 objectPosition;
        targetPosition.z = -10;

        switch (position)
        {
            default:
            case 0:     // Sector View
                targetPosition.x = 0;
                targetPosition.y = 0;
                targetSize = 550;
                targetLayerMask = 1 << 8;
                break;

            case 1:     // Cluster View
                objectPosition = o.transform.position;
                targetPosition.x = objectPosition.x;
                targetPosition.y = objectPosition.y;
                targetSize = 50;
                targetLayerMask = 1 << 9;
                break;

            case 2:     // System View
                objectPosition = o.transform.position;
                targetPosition.x = objectPosition.x;
                targetPosition.y = objectPosition.y;
                targetSize = 1;
                targetLayerMask = 1 << 10;
                break;
        }

        // When zooming in the mask should be applied immediately
        if (targetSize < mainCamera.orthographicSize)
        {
            mainCamera.cullingMask = targetLayerMask;
        }

        GetComponent<InputManager>().ChangeView(position, o);
        viewType = position;
    }

    private void MoveTowardTargetPosition()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, TransitionSpeed / 100f);
        transform.position = newPosition;
        float newSize =  mainCamera.orthographicSize + ((targetSize - mainCamera.orthographicSize) * (TransitionSpeed / 100f));
        mainCamera.orthographicSize = newSize;
        
        // When zooming out the mask should be applied when finished.
        if (Mathf.Abs(targetSize - mainCamera.orthographicSize) < .25f * targetSize)
        {
            mainCamera.cullingMask = targetLayerMask;
        }
    }
}
