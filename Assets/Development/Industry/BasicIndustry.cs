using System;
using System.Collections;
using System.Collections.Generic;

// Defines a development that creates and exports a basic resource from tile yields
public class BasicIndustry : Industry
{
    public BasicIndustry(Resource resource) : base(resource)
    { }

    // Estimates the available amount of resources that this BasicIndustry can produce
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the development capacity
        float developmentCapacity = EstimateDevelopmentCapacity(contractTerminal.suppliers);

        // Convert from development to resource output
        float resourceCapacity = developmentCapacity * CalculateOutputPerDevelopment(developmentCapacity);

        // Return the resourceCapacity
        return new Dictionary<Resource, float> {{producedResource, resourceCapacity}};
    }

    public override Dictionary<Resource, float> EstimateCost()
    {
        float newOutputPerDevelopment = CalculateOutputPerDevelopment(contractTerminal.resourceCapacity[producedResource]);
        float newDevelopment = contractTerminal.resourceCapacity[producedResource] / newOutputPerDevelopment;
        // Start with development costs
        float cost = CalculateDevelopmentCost(contractTerminal.suppliers, contractTerminal, newDevelopment);

        // Divide by total output
        if (totalDevelopment > 0)
        {
            cost = cost / (totalDevelopment * newOutputPerDevelopment);
        }

        return new Dictionary<Resource, float> {{producedResource, cost}};
    }

    public override Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        return CalculateDevelopmentDemand(CalculateDevelopmentPerCapacity(contractTerminal.boughtResourceCapacity[producedResource]));
    }


    public override float GenerateOutput()
    {
        // Sort the tiles by the order they should be developed
        SortTiles();

        // Convert boughtCapacity into development
        float newDevelopment = CalculateDevelopmentPerCapacity(contractTerminal.boughtResourceCapacity[producedResource]);

        // Grow the new development
        Grow(newDevelopment, contractTerminal);

        // Convert from development to resource output
        return totalDevelopment * CalculateOutputPerDevelopment(0);
    }

    public override Dictionary<Resource, float> CalculatePrice()
    {
        float cost = CalculateDevelopmentCost(null, contractTerminal, 0);

        if (totalDevelopment > 0)
        {
            return new Dictionary<Resource, float>() {{producedResource, cost / (totalDevelopment * CalculateOutputPerDevelopment(0))}};
        }
        else
        {
            return new Dictionary<Resource, float>() {{producedResource, cost}};
        }
    }

    protected void SortTiles()
    {
        tiles.Sort(delegate(Tile x, Tile y)
        {
            if ((int)x.resources[producedResource] > (int)y.resources[producedResource])
            {
                return -1;
            }
            else
            {
                return 1;
            }
        });
    }

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();

        if (importResources.Contains(producedResource))
        {
            importResources.Remove(producedResource);
        }

        return importResources;
    }

    // Calculates the projected output per development with the supplied amount of added development
    // Doesn't actually add any development to tiles
    // If there isn't enough room for the added development it calculates based on full development
    private float CalculateOutputPerDevelopment(float addedDevelopment)
    {
        float totalResources = 0;
        // Add up current output
        foreach (KeyValuePair<Tile, float> kvp in tileDevelopments)
        {
            totalResources += (int)kvp.Key.resources[producedResource] * kvp.Value / 100f;
        }

        // Add up projected output
        if (addedDevelopment > 0)
        {
            float developmentToAdd = addedDevelopment;
            foreach (Tile t in tiles)
            {
                if (!tileDevelopments.ContainsKey(t))
                {
                    totalResources += (int)t.resources[producedResource] * (developmentToAdd < 100 ? developmentToAdd : 100) / 100;
                    developmentToAdd -= developmentToAdd < 100 ? developmentToAdd : 100;
                }
                else if (tileDevelopments[t] < 100)
                {
                    float developmentRoomOnTile = 100 - tileDevelopments[t];
                    totalResources += (int)t.resources[producedResource] * (developmentToAdd < developmentRoomOnTile ? developmentToAdd : developmentRoomOnTile) / 100;
                    developmentToAdd -= developmentToAdd < developmentRoomOnTile ? developmentToAdd : developmentRoomOnTile;
                }

                if (developmentToAdd < 0)
                {
                    // We've added all of our new development
                    break;
                }
            }

            // Subtract development we couldn't add
            addedDevelopment -= developmentToAdd;
        }

        // Return the outputPerDevelopment
        if (totalDevelopment + addedDevelopment > 0)
        {
            return totalResources / (totalDevelopment + addedDevelopment);
        }
        else
        {
            return 0;
        }
    }

    // Calculates the amount of added development needed to fulfill the given output capacity
    // If there isn't enough room for development then it returns the amount of available development
    private float CalculateDevelopmentPerCapacity(float capacity)
    {
        // Add up projected output
        float developmentNeeded = 0;
        foreach (Tile t in tiles)
        {
            if (!tileDevelopments.ContainsKey(t))
            {
                float tileDevelopmentNeeded = capacity / (int)t.resources[producedResource] * 100f;
                float tileDevelopmentToAdd = tileDevelopmentNeeded < 100 ? tileDevelopmentNeeded : 100;
                developmentNeeded += tileDevelopmentToAdd;
                capacity -= tileDevelopmentToAdd * (int)t.resources[producedResource] / 100f;
            }
            else if (tileDevelopments[t] < 100)
            {
                float tileDevelopmentNeeded = capacity / (int)t.resources[producedResource] * 100f;
                float tileDevelopmentToAdd = tileDevelopmentNeeded < 100 - tileDevelopments[t] ? tileDevelopmentNeeded : 100 - tileDevelopments[t];
                developmentNeeded += tileDevelopmentToAdd;
                capacity -= tileDevelopmentToAdd * (int)t.resources[producedResource];
            }

            if (capacity < 0)
            {
                // We've added all of our new development
                break;
            }
        }

        return developmentNeeded;
    }
}
