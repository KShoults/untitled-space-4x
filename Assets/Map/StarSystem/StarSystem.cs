using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public string starSystemName;
    public GameObject StarPrefab;
    public Star star;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateStarSystem(int sizePlanetsAvg, int sizePlanetsVar)
    {
        // Create the star
        star = Instantiate(StarPrefab).GetComponent<Star>();
        star.transform.parent = transform;
        star.transform.position = transform.position;
        star.starName = starSystemName;
        star.starClass = GenerateStarClass();
        star.GetComponent<SpriteRenderer>().color = StarClassUtil.StarColor[star.starClass];
        GetComponent<SpriteRenderer>().color = StarClassUtil.StarColor[star.starClass];
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

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            Camera.main.GetComponent<CameraController>().SetCameraTargetSmooth(2, this);
        }
    }

    private StarClass GenerateStarClass()
    {
        float r = Random.value * 7;
        if (r < 1)
        {
            return StarClass.O;
        }
        else if (r < 2)
        {
            return StarClass.B;
        }
        else if (r < 3)
        {
            return StarClass.A;
        }
        else if (r < 4)
        {
            return StarClass.F;
        }
        else if (r < 5)
        {
            return StarClass.G;
        }
        else if (r < 6)
        {
            return StarClass.K;
        }
        else
        {
            return StarClass.M;
        }
    }
}
