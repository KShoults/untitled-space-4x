using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum View
{
    Sector,
    Cluster,
    System,
    Region
}

public class ViewController : MonoBehaviour
{
    public float TransitionSpeed;
    public View view;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private float targetSize;
    private int targetLayerMask;
    private MonoBehaviour viewObject;

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
        if (Vector3.Distance(transform.position, targetPosition) >= .01f ||
            Mathf.Abs(mainCamera.orthographicSize - targetSize) >= .01f * targetSize)
        {
            MoveTowardTargetPosition();
        }
    }

    /* Set the camera to one of three preset positions without a transition
    /  view: which view to move the camera to
    */ 
    public void SetCameraTarget(View newView, MonoBehaviour o)
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

        switch (newView)
        {
            default:
            case View.Sector:   // Sector View
                newPosition.x = 0;
                newPosition.y = 0;
                newSize = 550;
                newLayerMask = 1 << 8;
                break;

            case View.Cluster:  // Cluster View
                objectPosition = o.transform.position;
                newPosition.x = objectPosition.x;
                newPosition.y = objectPosition.y;
                newSize = 50;
                newLayerMask = 1 << 9;
                break;

            case View.System:   // System View
                objectPosition = o.transform.position;
                newPosition.x = objectPosition.x;
                newPosition.y = objectPosition.y;
                newSize = .75f;
                newLayerMask = 1 << 10;
                break;

            case View.Region:   // The Region View
                objectPosition = o.transform.position;
                newPosition.x = objectPosition.x - .04f;
                newPosition.y = objectPosition.y;
                newSize = .02f;
                newLayerMask = 1 << 11;
                break;
        }
        mainCamera.cullingMask = newLayerMask;
        transform.position = newPosition;
        targetPosition = newPosition;
        mainCamera.orthographicSize = newSize;
        targetSize = newSize;
        GetComponent<InputManager>().ChangeView(newView, o);
        view = newView;
        viewObject = o;
    }

    /* Set the camera target position to one of three preset positions.
    /  The camera will move smoothly to the target position.
    /  view: which view to move the camera to
    */ 
    public void SetCameraTargetSmooth(View newView, MonoBehaviour o)
    {
        Vector3 objectPosition;
        targetPosition.z = -10;

        switch (newView)
        {
            default:
            case View.Sector:   // Sector View
                targetPosition.x = 0;
                targetPosition.y = 0;
                targetSize = 550;
                targetLayerMask = 1 << 8;
                break;

            case View.Cluster:  // Cluster View
                objectPosition = o.transform.position;
                targetPosition.x = objectPosition.x;
                targetPosition.y = objectPosition.y;
                targetSize = 50;
                targetLayerMask = 1 << 9;
                break;

            case View.System:   // System View
                objectPosition = o.transform.position;
                targetPosition.x = objectPosition.x;
                targetPosition.y = objectPosition.y;
                targetSize = .75f;
                targetLayerMask = 1 << 10;
                break;

            case View.Region:   // The Region View
                objectPosition = o.transform.position;
                targetPosition.x = objectPosition.x - .04f;
                targetPosition.y = objectPosition.y;
                targetSize = .02f;
                targetLayerMask = 1 << 11;
                break;
        }

        // When zooming in the mask should be applied immediately
        if (targetSize < mainCamera.orthographicSize)
        {
            mainCamera.cullingMask = targetLayerMask;
            GetComponent<InputManager>().ChangeView(newView, o);
        }

        view = newView;
        viewObject = o;
    }

    private void MoveTowardTargetPosition()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, TransitionSpeed / 100f);
        transform.position = newPosition;
        float newSize =  mainCamera.orthographicSize + ((targetSize - mainCamera.orthographicSize) * (TransitionSpeed / 100f));
        mainCamera.orthographicSize = newSize;
        
        // When zooming out the mask should be applied when finished.
        if (targetSize - mainCamera.orthographicSize > 0 &&
            targetSize - mainCamera.orthographicSize < .1f * targetSize)
        {
            mainCamera.cullingMask = targetLayerMask;
            GetComponent<InputManager>().ChangeView(view, viewObject);
        }
    }
}
