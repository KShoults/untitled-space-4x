using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry
{
    public Resource resource;
    // The tiles that are allocated to this industry
    public List<Tile> tiles;


    public Industry()
    {
        tiles = new List<Tile>();
    }

    public Industry(Resource resource) : this()
    {
        this.resource = resource;
    }

    public Industry(List<Tile> tiles)
    {
        this.tiles = tiles;
    }
}
