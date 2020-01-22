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
    public Dictionary<Resource, float> stockpileRatios;
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
            float x = totalDevelopment * TRANSPORTTODEVRATIO * .005f;
            return x > .005f ? x : .005f;
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

    // Estimates the available amount of resources that this TransportHub can produce
    public Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the development capacity
        // This tells us as a transport hub how much more resources total we can output
        // Suppliers is empty at this point because we pull resources from our own stockpile
        float developmentCapacity = EstimateDevelopmentCapacity(contractTerminal.suppliers);
        Dictionary<Resource, float> resourceCapacity = new Dictionary<Resource, float>();

        // Update the stockpile ratios
        UpdateStockpileRatios();

        // Determine whether we want to export based on our stockpile ratios
        foreach (Resource r in contractTerminal.importResources)
        {
            if (stockpileRatios[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                resourceCapacity.Add(r, 0);
            }
            else if (stockpileRatios[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                resourceCapacity.Add(r, 0);
            }
            else if (stockpileRatios[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                resourceCapacity.Add(r, developmentCapacity);
            }
            else
            {
                // We are at surplus stockpile level
                resourceCapacity.Add(r, developmentCapacity);
            }
        }

        // Add the transport capacity
        resourceCapacity.Add(Resource.TransportCapacity, developmentCapacity);

        return resourceCapacity;
    }

    public Dictionary<Resource, float> EstimateCost()
    {
        Dictionary<Resource, float> costs = new Dictionary<Resource, float>();
        Dictionary<Resource, float> resourceOutputs = new Dictionary<Resource, float>();
        float totalOutput = 0;
        // Add the import costs for each resource
        foreach (Resource r in contractTerminal.importResources)
        {
            if (contractTerminal.importContracts.Count > 0)
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
            else
            {
                resourceOutputs.Add(r, 0);
            }
        }

        // Add the base cost to each resource proportionally by output
        float baseCost = CalculateDevelopmentCost(contractTerminal.suppliers, contractTerminal, 0);
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

        UpdateStockpileRatios();

        Resource[] keys = new Resource[importDemand.Count];
        importDemand.Keys.CopyTo(keys, 0);
        foreach (Resource r in keys)
        {
            // The minimum export level to consider
            float effectiveExportLevel = totalExports[r] > minimumExportLevel ? totalExports[r] : minimumExportLevel;
            if (stockpileRatios[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                // Target trend is 3
                importDemand[r] = 3 * effectiveExportLevel + effectiveExportLevel;
            }
            else if (stockpileRatios[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                // Target trend is 1
                importDemand[r] = 1 * effectiveExportLevel + effectiveExportLevel;
            }
            else if (stockpileRatios[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                // Target trend is 0
                importDemand[r] = totalExports[r];
            }
            else
            {
                // We are at surplus stockpile level
                // Target trend is -.05
                importDemand[r] = -.05f * effectiveExportLevel + effectiveExportLevel;
            }
        }

        return importDemand;
    }

    // Grows the development and returns the new transport capacity
    public float GenerateOutput()
    {
        float newDevelopment = contractTerminal.boughtResourceCapacity[Resource.TransportCapacity] / TRANSPORTTODEVRATIO;

        Grow(newDevelopment, contractTerminal);

        return totalDevelopment * TRANSPORTTODEVRATIO;
    }

    public Dictionary<Resource, float> CalculatePrice()
    {
        float developmentCost = CalculateDevelopmentCost(null, contractTerminal, 0);
        // The exports of each resource
        Dictionary<Resource, float> totalExports = new Dictionary<Resource, float>();
        // The exports of all resources
        float totalOutput = 0;
        // The price per unit of this hub's exports
        Dictionary<Resource, float> prices = new Dictionary<Resource, float>();

        foreach (Resource r in contractTerminal.importResources)
        {
            if (contractTerminal.importContracts.Count > 0)
            {
                // The total cost of this resource's inputs
                float resourceCost = 0;
                // The total amount of this resource's inputs
                float resourceAmount = 0;
                foreach (Contract c in contractTerminal.importContracts[r])
                {
                    resourceCost += c.cost * c.amount;
                    resourceAmount += c.amount;
                }
                prices.Add(r, resourceCost / resourceAmount);
                float resourceExports = 0;
                foreach (Contract c in contractTerminal.exportContracts[r])
                {
                    resourceExports += c.amount;
                }
                totalExports.Add(r, resourceExports);
                totalOutput += resourceExports;
            }
            else
            {
                totalExports.Add(r, 0);
                prices.Add(r, contractTerminal.suppliers[r].Min.Item3);
            }
        }

        foreach (Resource r in contractTerminal.importResources)
        {
            if (totalOutput > 0)
            {
                prices[r] += developmentCost * totalExports[r] / totalOutput;
            }
        }

        return prices;
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
                // Add import amounts
                stockpile[r] += c.amount;
            }

            // Calculate hub's usage of this resource
            float resourceUsage = 0;
            if (r == Resource.Energy)
            {
                resourceUsage = totalDevelopment * ENERGYTODEVRATIO;
            }
            else if (r == Resource.Water)
            {
                resourceUsage = totalDevelopment * WATERTOPOPRATIO * POPTODEVRATIO;
            }
            else if (r == Resource.Food)
            {
                resourceUsage = totalDevelopment * FOODTOPOPRATIO * POPTODEVRATIO;
            }
            // Later we can add usage for ship parts here

            // Subtract hub resource usage
            stockpile[r] -= resourceUsage;

            // Never go below 0 stockpile amount
            stockpile[r] = stockpile[r] > 0 ? stockpile[r] : 0;
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
    protected override float EstimateDevelopmentCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
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
        stockpileRatios = new Dictionary<Resource, float>();
        stockpileTrend = new Dictionary<Resource, float>();

        foreach (Resource r in contractTerminal.importResources)
        {
            stockpile.Add(r, 0);
            totalImports.Add(r, 0);
            totalExports.Add(r, 0);
            stockpileRatios.Add(r, 0);
            stockpileTrend.Add(r, 0);
        }
    }

    private void UpdateStockpileRatios()
    {
        // Update each stockpile
        foreach (Resource r in stockpile.Keys)
        {
            // Add up the exports
            totalExports[r] = 0;
            foreach (Contract c in contractTerminal.exportContracts[r])
            {
                totalExports[r] += c.amount;
            }

            // Add up the imports
            totalImports[r] = 0;
            foreach (Contract c in contractTerminal.importContracts[r])
            {
                totalImports[r] += c.amount;
            }

            // Limit by the minimum export level
            float effectiveExportLevel = totalExports[r] > minimumExportLevel ? totalExports[r] : minimumExportLevel;

            // Calculate the stockpile ratio
            stockpileRatios[r] = stockpile[r] / effectiveExportLevel;

            // Calculate the stockpile trend
            stockpileTrend[r] = totalExports[r] > 0 ? (totalImports[r] - totalExports[r]) / totalExports[r] : 0;
        }
    }
}
