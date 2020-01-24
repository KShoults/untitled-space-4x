using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines an object which occupies tiles on a planet
public abstract class TileOccupier
{
    // The tiles that are occupied by this TileOccupier
    public List<Tile> tiles;

    public TileOccupier()
    {
        tiles = new List<Tile>();
    }
}
