using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public NameManager nameManager;
    public Text TurnCounterText;
    public int turnCounter;
    // Galaxy generation default settings
    public int numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar;
    public Sector sector;
    public float nameSeed = 0;

    void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManager = this;
            if (nameSeed == 0f)
            {
                float nameSeed = Random.value;
            }
            nameManager = new NameManager(nameSeed);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create the sector using perlin noise maps
        sector.GenerateSector(numClusters, numSystemsAvg, numSystemsVar, sizePlanetsAvg, sizePlanetsVar);
        TurnCounterText.text = "Turn: " + turnCounter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndTurn()
    {
        turnCounter++;
        TurnCounterText.text = "Turn: " + turnCounter;
    }
}
