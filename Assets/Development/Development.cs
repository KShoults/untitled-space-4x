﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Development
{
    // The resource produced by this development
    public Resource resource;
    // The ratio of population to a point of development
    public const int POPTODEVRATIO = 1000;
    // The tiles that are allocated to this development
    public List<Tile> tiles;
    // The development per tile
    public Dictionary<Tile, float> tileDevelopments;
    // Population employed by the development
    public ulong population;

    public Development()
    {
        tiles = new List<Tile>();
        tileDevelopments = new Dictionary<Tile, float>();
    }
    public Development(Resource resource) : this()
    {
        this.resource = resource;
    }

    public virtual void Grow()
    {
        
        // Identify the population source
        // For now we create it from nothing
        
        // Calculate the maximum population we can add to this development
        // Later on we will base this on transport costs
        int populationToAdd = 10000;

        // Limit by the maximum population for this development
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
}