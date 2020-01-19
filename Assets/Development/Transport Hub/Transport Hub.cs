using System;
using System.Collections;
using System.Collections.Generic;

public class TransportHub : Development, IContractEndpoint
{
    public HubContractTerminal contractTerminal;
    // This transport hub's resource stockpiles
    public Dictionary<Resource, float> stockpile;
    // The total amount of resources being imported
    public Dictionary<Resource, float> totalImports;
    // The total amount of resources being exported
    public Dictionary<Resource, float> totalExports;
    // The current ratio of the stockpile
    // All stockpile ratios are ratios of the current amount in the stockpile compared to the total amount in export contracts
    public Dictionary<Resource, float> stockpileRatio;
    // How much the stockpile is trending upwards or downwards as a ratio
    public Dictionary<Resource, float> stockpileTrend;
    // The minimum ratio at which a stockpile is marked as having a shortage
    public const float SHORTAGERATIO = 1;
    // The minimum ratio at which a stockpile is marked as being ideal
    public const float IDEALRATIO = 3;
    // The minimum ratio at which a stockpile is marked as having a surplus
    public const float SURPLUSRATIO = 5;
    // The ratio of transportation capacity to development
    public const float TRANSPORTTODEVRATIO = 10;

    // The minimum amount of exports that the stockpile should be prepared for regardless of its current exports
    private float minimumExportLevel
    {
        get
        { 
            float x = totalDevelopment * TRANSPORTTODEVRATIO * .01f;
            return x > .01f ? x : .01f;
        }
    }

    public TransportHub() : base(Resource.TransportCapacity)
    {
        contractTerminal = new HubContractTerminal(this, Resource.TransportCapacity, GetImportResources());
        InitializeStockpile();
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

        CalculateStockpileRatios();

        // Determine whether we want to export (and how much) based on our current stockpile
        foreach (Resource r in contractTerminal.importResources)
        {
            if (stockpileRatio[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                capacity.Add(r, 0);
            }
            else if (stockpileRatio[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                capacity.Add(r, 0);
            }
            else if (stockpileRatio[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                capacity.Add(r, developmentCapacity);
            }
            else
            {
                // We are at surplus stockpile level
                capacity.Add(r, developmentCapacity);
            }
        }
        capacity.Add(Resource.TransportCapacity, developmentCapacity);

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

    public Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        Dictionary<Resource, float> importDemand = new Dictionary<Resource, float>();

        // Calculate different demand depending on whether we're in the first round or second round of evaluation
        if (!contractTerminal.completedFirstStageContractEvaluation)
        {
            importDemand.Add(Resource.MilitaryGoods, 0);
            importDemand.Add(Resource.ShipParts, 0);
            importDemand.Add(Resource.CivilianGoods, 0);
        }
        else
        {
            importDemand.Add(Resource.Energy, 0);
            importDemand.Add(Resource.Water, 0);
            importDemand.Add(Resource.Food, 0);
            importDemand.Add(Resource.Minerals, 0);
        }

        foreach (Resource r in contractTerminal.importResources)
        {
            // The minimum export level to consider
            float effectiveExportLevel = totalExports[r] > minimumExportLevel ? totalExports[r] : minimumExportLevel;
            if (stockpileRatio[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                // Target trend is .5
                importDemand[r] = .5f * effectiveExportLevel + effectiveExportLevel - totalImports[r];
            }
            else if (stockpileRatio[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                // Target trend is .1
                importDemand[r] = .1f * effectiveExportLevel + effectiveExportLevel - totalImports[r];
            }
            else if (stockpileRatio[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                // Target trend is .05
                importDemand[r] = .05f * effectiveExportLevel + effectiveExportLevel - totalImports[r];
            }
            else
            {
                // We are at surplus stockpile level
                // Target trend is -.05
                importDemand[r] = -.5f * effectiveExportLevel + effectiveExportLevel - totalImports[r];
            }
        }

        return importDemand;
    }

    // Grows the development and returns the new transport capacity
    public float GenerateOutput()
    {
        float newDevelopment = contractTerminal.boughtCapacity[Resource.TransportCapacity];

        Grow(newDevelopment, contractTerminal);

        return totalDevelopment * TRANSPORTTODEVRATIO;
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    public void GrowStockpile()
    {
        foreach (Resource r in contractTerminal.importResources)
        {
            foreach (Contract c in contractTerminal.importContracts[r])
            {
                stockpile[r] += c.amount;
            }
        }
    }

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

    private void InitializeStockpile()
    {
        stockpile = new Dictionary<Resource, float>();
        totalImports = new Dictionary<Resource, float>();
        totalExports = new Dictionary<Resource, float>();
        stockpileRatio = new Dictionary<Resource, float>();
        stockpileTrend = new Dictionary<Resource, float>();

        foreach (Resource r in contractTerminal.importResources)
        {
            stockpile.Add(r, 0);
            totalImports.Add(r, 0);
            totalExports.Add(r, 0);
            stockpileRatio.Add(r, 0);
            stockpileTrend.Add(r, 0);
        }
    }

    private void CalculateStockpileRatios()
    {
        // Add up the imports and exports
        foreach (Resource r in contractTerminal.importResources)
        {
            totalExports[r] = 0;
            totalImports[r] = 0;
            foreach (Contract c in contractTerminal.exportContracts[r])
            {
                totalExports[r] += c.amount;
            }
            foreach (Contract c in contractTerminal.importContracts[r])
            {
                totalImports[r] += c.amount;
            }

            float effectiveExportLevel = totalExports[r] > minimumExportLevel ? totalExports[r] : minimumExportLevel;
            stockpileRatio[r] = stockpile[r] / effectiveExportLevel;

            stockpileTrend[r] = totalExports[r] > 0 ? (totalImports[r] - totalExports[r]) / totalExports[r] : 0;
        }
    }
}
