using System;
using System.Collections;
using System.Collections.Generic;

// Defines a TileOccupier that creates development on its tiles
public abstract class Development : TileOccupier
{
    // The development per tile
    public Dictionary<Tile, float> tileDevelopments;
    // The total amount of development
    public float totalDevelopment
    {
        get
        {
            float total = 0;
            foreach (KeyValuePair<Tile, float> kvp in tileDevelopments)
            {
                total += kvp.Value;
            }
            return total;
        }
    }



    public Development()
    {
        tileDevelopments = new Dictionary<Tile, float>();
    }



    // Calculate the total development capacity for this development
    protected float CalculateDevelopmentCapacity()
    {

        // Add up the development of undeveloped tiles
        float developmentCapacity = 100 * tiles.Count;

        // Return the development capacity
        return developmentCapacity;
    }

    // Grow the development as much as it can limited by targetDevelopment
    public virtual void Grow(float targetDevelopment)
    {
        float developmentToAdd = targetDevelopment;

        // Reduce the developmentToAdd by the existing tile development
        foreach (Tile t in tiles)
        {
            // If it's already developed then reduce developmentToAdd by its development
            if (tileDevelopments.ContainsKey(t))
            {
                developmentToAdd -= tileDevelopments[t];
            }

            // Reduce development if we went over targetDevelopment
            if (developmentToAdd < 0)
            {
                tileDevelopments[t] -= developmentToAdd;
                developmentToAdd = 0;
            }
        }

        // Allocate the desired development to the highest priority tile
        foreach (Tile t in tiles)
        {
            // If it's already developed then develop whatever room's left
            if (tileDevelopments.ContainsKey(t))
            {
                tileDevelopments[t] += developmentToAdd;
            }
            // Else add up to 100 development
            else
            {
                tileDevelopments.Add(t, 100 < developmentToAdd ? 100 : developmentToAdd);
            }

            // If we went over 100 development then the extra
            // can be added in the next loop to the next tile
            if (tileDevelopments[t] > 100)
            {
                developmentToAdd = tileDevelopments[t] - 100;
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
