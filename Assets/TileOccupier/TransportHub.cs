using System;
using System.Collections;
using System.Collections.Generic;

public class TransportHub : Employer
{
    // This transport hub's resource stockpiles
    public Dictionary<Resource, float> stockpile;
    // The current ratio of the stockpile
    // All stockpile ratios are ratios of the current amount in the stockpile compared to the total amount in export contracts
    public Dictionary<Resource, float> stockpileRatios;
    // How much the stockpile is trending upwards or downwards as a ratio
    public Dictionary<Resource, float> stockpileTrends;
    // The minimum ratio at which a stockpile is marked as having a shortage
    public const float SHORTAGERATIO = 1;
    // The minimum ratio at which a stockpile is marked as being ideal
    public const float IDEALRATIO = 3;
    // The minimum ratio at which a stockpile is marked as having a surplus
    public const float SURPLUSRATIO = 5;
    // The ratio of transportation capacity to development
    public const float TRANSPORTTODEVRATIO = 10;
    // Determines if the first stage of contract evaluations has occured
    // Transport hubs evaluate their contracts in two waves
    public bool completedFirstStageContractEvaluation;
    // Determines if the first stage of contract fulfillment has occured
    // Transport hubs fulfill their export contracts in the first wave
    // They then build their stockpiles with import contracts in the second wave
    public bool completedFirstStageContractFulfillment;

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
        InitializeStockpile();
    }

    /**************************************************************
        IContractEndpoint Member Overrides
    **************************************************************/

    // Estimates the available amount of resources that this IContractEndpoint can produce
    // In this override we additionaly calculate how much of every other resource we can produce
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the resource capacity
        // This tells us as a transport hub how much resources total we can output
        Dictionary<Resource, float> resourceCapacity = base.EstimateResourceCapacity();

        // Update the stockpile ratios
        UpdateStockpileRatios();

        // Determine whether we want to export based on our stockpile ratios
        foreach (Resource r in contractTerminal.importResources)
        {
            float totalExports = contractTerminal.CalculateTotalExports(r);

            if (stockpileRatios[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                resourceCapacity.Add(r, totalExports);
            }
            else if (stockpileRatios[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                resourceCapacity.Add(r, totalExports);
            }
            else if (stockpileRatios[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                resourceCapacity.Add(r, resourceCapacity[producedResource]);
            }
            else
            {
                // We are at surplus stockpile level
                resourceCapacity.Add(r, resourceCapacity[producedResource]);
            }
        }

        return resourceCapacity;
    }

    // Estimates the cost per unit of buying the resources that this IContractEndpoint produces
    // In this override we instead calculate the cost of other resources
    public override Dictionary<Resource, float> EstimateCost(float targetResourceCapacity)
    {
        // Add the import costs for each resource
        Dictionary<Resource, float> costs = new Dictionary<Resource, float>();
        foreach (Resource r in contractTerminal.importResources)
        {
            if (contractTerminal.importContracts[r].Count > 0)
            {
                // Find the total import cost
                float resourceImportCost = contractTerminal.CalculateImportCosts(r);
                float totalImports = contractTerminal.CalculateTotalImports(r);

                // Add the resource import cost
                costs.Add(r, resourceImportCost / totalImports);
            }
            else
            {
                costs.Add(r, 0);
            }
        }

        return costs;
    }

    // Calculates the need for each resource to determine contract reevaluation
    // In this override we add demand for all of the other resources
    public override Dictionary<Resource, float> CalculateImportDemand(float targetResourceCapacity)
    {
        // Calculate different demand depending on whether we're in the first round or second round of evaluation
        Dictionary<Resource, float> importDemand;
        if (!completedFirstStageContractEvaluation)
        {
            importDemand = new Dictionary<Resource, float>();
            importDemand.Add(Resource.MilitaryGoods, 0);
            importDemand.Add(Resource.ShipParts, 0);
            importDemand.Add(Resource.CivilianGoods, 0);
        }
        else
        {
            // Find the producer and employer demand for energy/food/water
            importDemand = base.CalculateImportDemand(targetResourceCapacity);
            importDemand.Add(Resource.Minerals, 0);
        }

        // Update the stockpile ratios
        UpdateStockpileRatios();

        // Calculate the import demand for each resource
        Resource[] keys = new Resource[importDemand.Count];
        importDemand.Keys.CopyTo(keys, 0);
        foreach (Resource r in keys)
        {
            // Find the minimum export level to consider
            float totalResourceExports = contractTerminal.CalculateTotalExports(r);
            float effectiveExportLevel = totalResourceExports > minimumExportLevel ? totalResourceExports : minimumExportLevel;

            if (stockpileRatios[r] < SHORTAGERATIO)
            {
                // We are at critical stockpile level
                // Target trend is 3
                importDemand[r] += 3 * effectiveExportLevel + effectiveExportLevel;
            }
            else if (stockpileRatios[r] < IDEALRATIO)
            {
                // We are at shortage stockpile level
                // Target trend is 1
                importDemand[r] += 1 * effectiveExportLevel + effectiveExportLevel;
            }
            else if (stockpileRatios[r] < SURPLUSRATIO)
            {
                // We are at ideal stockpile level
                // Target trend is 0
                importDemand[r] += totalResourceExports;
            }
            else
            {
                // We are at surplus stockpile level
                // Target trend is -.05
                importDemand[r] += -.05f * totalResourceExports + totalResourceExports;
            }
        }

        return importDemand;
    }

    // Determines the final sale price per unit of this IContractEndpoint's exports
    // In this override we calculate the price of every resource
    public override Dictionary<Resource, float> CalculatePrice()
    {
        // Get our base cost
        Dictionary<Resource, float> costs = base.CalculatePrice();

        // Add the import costs for each resource
        foreach (Resource r in contractTerminal.importResources)
        {
            if (contractTerminal.importContracts[r].Count > 0)
            {
                // Find the total import cost
                float resourceImportCost = contractTerminal.CalculateImportCosts(r);
                float totalImports = contractTerminal.CalculateTotalImports(r);

                // Add the resource import cost
                costs.Add(r, resourceImportCost / totalImports);
            }
            else
            {
                costs.Add(r, 0);
            }
        }

        return costs;
    }

    /**************************************************************
        Producer Member Overrides
    **************************************************************/

    // Returns the contract terminal for this producer
    protected override ContractTerminal CreateContractTerminal()
    {
        return new HubContractTerminal(this, producedResource, GetImportResources());
    }


    // Returns the resources we will need to import
    // In this override we add all of the other tangible resources to import resources
    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        importResources.Add(Resource.Minerals);
        importResources.Add(Resource.CivilianGoods);
        importResources.Add(Resource.MilitaryGoods);
        importResources.Add(Resource.ShipParts);
        return importResources;
    }

    // Returns the amount of output produced at the target development
    protected override float CalculateOutputAtDevelopment(float targetDevelopment)
    {
        return targetDevelopment * TRANSPORTTODEVRATIO;
    }

    // Returns the amount of development required to meet the target output
    protected override float CalculateDevelopmentAtOutput(float targetOutput)
    {
        return targetOutput / TRANSPORTTODEVRATIO;
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    private void InitializeStockpile()
    {
        stockpile = new Dictionary<Resource, float>();
        stockpileRatios = new Dictionary<Resource, float>();
        stockpileTrends = new Dictionary<Resource, float>();

        foreach (Resource r in contractTerminal.importResources)
        {
            stockpile.Add(r, 0);
            stockpileRatios.Add(r, 0);
            stockpileTrends.Add(r, 0);
        }
    }

    private void UpdateStockpileRatios()
    {
        // Update each stockpile
        foreach (Resource r in stockpile.Keys)
        {
            // Add up the exports
            float totalExports = contractTerminal.CalculateTotalExports(r);

            // Add up the imports
            float totalImports = contractTerminal.CalculateTotalImports(r);

            // Limit by the minimum export level
            float effectiveExportLevel = totalExports > minimumExportLevel ? totalExports : minimumExportLevel;

            // Calculate the stockpile ratio
            stockpileRatios[r] = stockpile[r] / effectiveExportLevel;

            // Calculate the stockpile trend
            stockpileTrends[r] = totalExports > 0 ? (totalImports - totalExports) / totalExports : 0;
        }
    }

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
}
