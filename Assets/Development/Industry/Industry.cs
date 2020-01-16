using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry : Development
{
    public Industry(Resource resource) : base()
    {
        this.resource = resource;
    }

    public override void Grow()
    {
        // Sort the tiles by the order they should be developed
        // We only need to sort for basic industries
        if (resource == Resource.Energy ||
            resource == Resource.Water ||
            resource == Resource.Food ||
            resource == Resource.Minerals)
        {
            SortTiles();
        }

        base.Grow();
    }

    private void SortTiles()
    {
        tiles.Sort(delegate(Tile x, Tile y)
        {
            if ((int)x.resources[resource] > (int)y.resources[resource])
            {
                return -1;
            }
            else
            {
                return 1;
            }
        });
    }
}
