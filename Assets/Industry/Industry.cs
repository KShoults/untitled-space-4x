using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry
{
    // The ratio of population to a point of development
    public const int POPTODEVRATIO = 1000;
    public Resource resource;
    // The tiles that are allocated to this industry
    public List<Tile> tiles;
    // Population employed by the industry
    public ulong population;
    public Dictionary<Tile, float> tileDevelopments;


    public Industry()
    {
        tiles = new List<Tile>();
        tileDevelopments = new Dictionary<Tile, float>();
    }

    public Industry(Resource resource) : this()
    {
        this.resource = resource;
    }

    public void Grow()
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
        
        // Identify the population source
        // For now we create it from nothing
        
        // Calculate the maximum population we can add to this industry
        // Later on we will base this on transport costs
        int populationToAdd = 10000;

        // Limit by the maximum population for this industry
        ulong maxPop = (ulong)tiles.Count * 100 * POPTODEVRATIO;
        populationToAdd = (int)(maxPop - population) >= populationToAdd ? populationToAdd : (int)(maxPop - population);

        // Calculate how much new development this will give us
        float newDevelopment = (float)populationToAdd / POPTODEVRATIO;

        // Allocate the new development to the highest priority tile
        foreach (Tile t in tiles)
        {
            // If it isn't developed yet then create it with the new development
            if (!tileDevelopments.ContainsKey(t))
            {
                tileDevelopments.Add(t, newDevelopment);
            }
            else
            {
                tileDevelopments[t] += newDevelopment;
            }

            // If we went over 100 development then the extra
            // can be added in the next loop to the next tile
            if (tileDevelopments[t] > 100)
            {
                newDevelopment = tileDevelopments[t] - 100;
                tileDevelopments[t] = 100;
            }
            else
            {
                // If we didn't go over 100 development then we're done
                break;
            }
        }
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
