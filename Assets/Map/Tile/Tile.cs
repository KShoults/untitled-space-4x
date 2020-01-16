using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Yield
{
    Low = 2,        // 2
    Medium = 6,     // 6
    High = 10,      // 10
    Uncommon = 15,  // 15
    Rare = 20       // 20
}

public class Tile
{
    // The resource yields on this tile
    public Dictionary<Resource, Yield> resources;
    // The development that this tile is allocated to
    public Development development;

    public Tile()
    {
        resources = new Dictionary<Resource, Yield>();
    }

    public Tile(Resource primaryResource, Yield primaryYield) : this()
    {
        resources.Add(primaryResource, primaryYield);
    }

    public Tile(Resource primaryResource, Yield primaryYield,
                Resource secondaryResource, Yield secondaryYield) : this(primaryResource, primaryYield)
    {
        resources.Add(secondaryResource, secondaryYield);
    }

    public Tile(Resource primaryResource, Yield primaryYield,
                Resource secondaryResource, Yield secondaryYield,
                Resource tertiaryResource, Yield tertiaryYield) : this(primaryResource, primaryYield, secondaryResource, secondaryYield)
    {
        resources.Add(tertiaryResource, tertiaryYield);
    }
}
