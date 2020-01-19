using System;
using System.Collections;
using System.Collections.Generic;

// Defines a development that creates and exports a basic resource from tile yields
public class BasicIndustry : Industry
{
    // The ratio of resources produced per point of tile development
    // Recalculated during capacity calculation
    protected float outputPerDevelopment;

    public BasicIndustry(Resource resource) : base(resource)
    { }

    public override Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        // Get the development capacity
        float developmentCapacity = CalculateDevelopmentCapacity(suppliers);

        // Convert from development to resource output
        UpdateOutputPerDevelopment();
        return new Dictionary<Resource, float> {{resource, developmentCapacity * outputPerDevelopment}};
    }

    public override Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        float newDevelopment = contractTerminal.capacity[resource] / outputPerDevelopment;
        // Start with development costs
        float cost = CalculateDevelopmentCosts(suppliers, contractTerminal, newDevelopment);

        // Divide by total output
        if (totalDevelopment > 0 && outputPerDevelopment > 0)
        {
            cost = cost / (totalDevelopment * outputPerDevelopment);  
        }

        return new Dictionary<Resource, float> {{resource, cost}};
    }

    public override float GenerateOutput()
    {
        // Sort the tiles by the order they should be developed
        // We also need to convert boughtCapacity into development
        SortTiles();

        
        float output = base.GenerateOutput();
        // Convert from development to resource output
        UpdateOutputPerDevelopment();
        return output * outputPerDevelopment;
    }

    private void UpdateOutputPerDevelopment()
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
                outputPerDevelopment = totalResources / totalDevelopment;
            }
            else
            {
                outputPerDevelopment = 0;
            }
        }
        else
        {
            outputPerDevelopment = .01f;
        }
    }

    protected void SortTiles()
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

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();

        if (importResources.Contains(resource))
        {
            importResources.Remove(resource);
        }
        
        return importResources;
    }
}
