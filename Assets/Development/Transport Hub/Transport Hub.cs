using System;
using System.Collections;
using System.Collections.Generic;

public class TransportHub : Development, IContractEndpoint
{
    public HubContractTerminal contractTerminal;
    public TransportHub(Resource resource) : base()
    {
        this.resource = resource;
        contractTerminal = new HubContractTerminal(this, resource, GetImportResources());
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        // Get the maximum development capacity
        // This tells us as a transport hub how much more resources total we can output
        float developmentCapacity = CalculateDevelopmentCapacity(suppliers);
        Dictionary<Resource, float> capacity = new Dictionary<Resource, float>();

        // Determine whether we want to export (and how much) based on our current stockpile
        // For now we just always export as much as possible
        foreach (Resource r in contractTerminal.importResources)
        {
            capacity.Add(r, developmentCapacity);
        }
        capacity.Add(resource, developmentCapacity);

        return capacity;
    }

    public Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        Dictionary<Resource, float> costs = new Dictionary<Resource, float>();
        Dictionary<Resource, float> resourceOutputs = new Dictionary<Resource, float>();
        float totalOutput = 0;
        // Add the import costs for each resource
        foreach (Resource r in contractTerminal.importResources)
        {
            float cost = 0;
            foreach (Contract c in contractTerminal.importContracts[r])
            {
                cost += c.cost * c.amount;
            }
            costs.Add(r, cost);
            float resourceOutput = 0;
            foreach (Contract c in contractTerminal.exportContracts[r])
            {
                resourceOutput += c.amount;
            }
            resourceOutputs.Add(r, resourceOutput);
            totalOutput += resourceOutput;
        }

        // Add the base cost to each resource proportionally by output
        float baseCost = CalculateDevelopmentCosts(suppliers, contractTerminal, 0);
        foreach (Resource r in contractTerminal.importResources)
        {
            if (totalOutput != 0)
            {
                costs[r] += baseCost * resourceOutputs[r] / totalOutput;
            }
        }

        return costs;
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        importResources.Add(Resource.Minerals);
        importResources.Add(Resource.CivilianGoods);
        importResources.Add(Resource.MilitaryGoods);
        importResources.Add(Resource.ShipParts);
        return importResources;
    }

    // Calculate the amount of available room for this development
    // In the case of transport hubs we're not checking for suppliers
    protected override float CalculateDevelopmentCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        float developmentCapacity = 0;

        foreach (Tile t in tiles)
        {
            if (tileDevelopments.ContainsKey(t))
            {
                // Add however much room there is in this tile
                developmentCapacity += 100 - tileDevelopments[t];
            }
            else
            {
                // If it isn't developed we just add an empty tile's worth
                developmentCapacity += 100;
            }
        }

        // Limit by the amount of population that can be moved
        // It will eventually be based on transport costs
        developmentCapacity = developmentCapacity <= MAXPOPTOMOVE ? developmentCapacity : MAXPOPTOMOVE;

        return developmentCapacity;
    }
}
