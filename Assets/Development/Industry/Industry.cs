using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry : Development, IContractEndpoint
{
    public ContractTerminal contractTerminal;
    // The ratio of resources produced per point of tile development
    // Recalculated during capacity calculation
    private float resourcesPerDevelopment;

    public Industry(Resource resource) : base()
    {
        this.resource = resource;
        contractTerminal = new ContractTerminal(resource, this);
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public float CalculateCapacity()
    {
        // Get the maximum development capacity
        float developmentCapacity = CalculateDevelopmentCapacity();

        // Search for transport hubs to supply this new capacity
        // For now we don't need supplies

        // Convert from development to resource output
        UpdateResourcesPerDevelopment();
        return developmentCapacity * resourcesPerDevelopment;
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

    /**************************************************************
        Personal Members
    **************************************************************/

    private void UpdateResourcesPerDevelopment()
    {
        // If we produce a basic resource
        if ((int)resource < 100)
        {
            float totalResources = 0;
            float totalDevelopment = 0;
            foreach (Tile t in tiles)
            {
                if (tileDevelopments.ContainsKey(t))
                {
                    totalResources += (int)t.resources[resource] * tileDevelopments[t] / 100;
                    totalDevelopment += tileDevelopments[t];
                }
            }
            if (totalDevelopment > 0)
            {
                resourcesPerDevelopment = totalResources / totalDevelopment;
            }
            else
            {
                resourcesPerDevelopment = 0;
            }
        }
        else
        {
            resourcesPerDevelopment = 1;
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
