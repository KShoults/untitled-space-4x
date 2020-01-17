using System;
using System.Collections;
using System.Collections.Generic;

public class Industry : Development, IContractEndpoint
{
    public ContractTerminal contractTerminal;
    // The ratio of resources produced per point of tile development
    // Recalculated during capacity calculation
    private float outputPerDevelopment;

    public Industry(Resource resource) : base()
    {
        this.resource = resource;
        contractTerminal = new ContractTerminal(this, resource, GetImportResources());
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        // Get the development capacity
        float developmentCapacity = CalculateDevelopmentCapacity(suppliers);

        // If we're an advanced industry
        if ((int)resource >= 100)
        {
            // Search for a supplier of minerals for this new capacity
            float mineralShortage = developmentCapacity;
            foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Minerals])
            {
                mineralShortage -= s.Item2;
                if (mineralShortage < 0)
                {
                    // We've found enough energy for our new development
                    mineralShortage = 0;
                    break;
                }
            }
            developmentCapacity -= mineralShortage;
        }

        // Convert from development to resource output
        UpdateOutputPerDevelopment();
        return new Dictionary<Resource, float> {{resource, developmentCapacity * outputPerDevelopment}};
    }

    public Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        float newDevelopment = contractTerminal.capacity[resource] / outputPerDevelopment;
        // Start with development costs
        float cost = CalculateDevelopmentCosts(suppliers, contractTerminal, newDevelopment);

        // Add other import costs
        // If we're an advanced industry
        if ((int)resource >= 100)
        {
            // Add mineral costs
            foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
            {
                cost += c.cost * c.amount;
            }
            // The input capacity for advanced industries is 10 times their output capacity
            float inputCapacity = newDevelopment;
            // Add future mineral costs
            foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Minerals])
            {
                float capacityToUse = s.Item2 < inputCapacity ? s.Item2 : inputCapacity;
                cost += s.Item3 * capacityToUse;
                inputCapacity -= capacityToUse;
            }
        }

        // Divide by total output
        if (totalDevelopment > 0 && outputPerDevelopment > 0)
        {
            cost = cost / (totalDevelopment * outputPerDevelopment);  
        }

        return new Dictionary<Resource, float> {{resource, cost}};
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

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        if (importResources.Contains(resource))
        {
            importResources.Remove(resource);
        }
        // If this is an advanced resource
        if ((int)resource  >= 100)
        {
            importResources.Add(Resource.Minerals);
        }
        return importResources;
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
            outputPerDevelopment = 1;
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
