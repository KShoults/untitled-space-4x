using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a development that produces a resource
public abstract class Producer : Development, IContractEndpoint
{
    // The resource produced by this development
    public Resource producedResource;
    // Our connection to the contract system
    public ContractTerminal contractTerminal;
    // The amount of energy needed to run a point of development
    public const float ENERGYTODEVRATIO = .01f;



    public Producer(Resource resource) : base()
    {
        this.producedResource = resource;
        this.contractTerminal = CreateContractTerminal();
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    // Estimates the available amount of resources that this IContractEndpoint can produce
    public virtual Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the capacity from development
        float developmentCapacity = base.CalculateDevelopmentCapacity();
        float resourceCapacity = CalculateOutputAtDevelopment(developmentCapacity);

        // Find energy suppliers for development
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Energy))
        {
            float energyDemand = CalculateDevelopmentAtOutput(resourceCapacity) * ENERGYTODEVRATIO;
            float energyShortage = contractTerminal.CheckForSuppliers(Resource.Energy, energyDemand);

            // Reduce by energy shortage
            resourceCapacity -= CalculateOutputAtDevelopment(energyShortage / ENERGYTODEVRATIO);
        }

        // Return resource capacity
        return new Dictionary<Resource, float>() {{producedResource, resourceCapacity}};
    }

    // Estimates the cost per unit of buying the resources that this IContractEndpoint produces
    public virtual Dictionary<Resource, float> EstimateCost(float targetResourceCapacity)
    {

        if (targetResourceCapacity > 0)
        {
            // Estimate the energy cost
            float energyCost = 0;
            if (!contractTerminal.exportContracts.ContainsKey(Resource.Energy))
            {
                energyCost = contractTerminal.EstimateImportCost(Resource.Energy, CalculateDevelopmentAtOutput(targetResourceCapacity) * ENERGYTODEVRATIO);
            }
            return new Dictionary<Resource, float>() {{producedResource, energyCost / targetResourceCapacity}};
        }
        else
        {
            return new Dictionary<Resource, float>() {{producedResource, 0}};
        }
    }

    // Calculates the need for each resource to determine contract reevaluation
    public virtual Dictionary<Resource, float> CalculateImportDemand(float targetResourceCapacity)
    {
        Dictionary<Resource, float> importDemand = new Dictionary<Resource, float>();

        // Calculate how much development we need
        float development = CalculateDevelopmentAtOutput(targetResourceCapacity);

        // Add energy demand
        if (producedResource != Resource.Energy)
        {
            float energyNeed = development * ENERGYTODEVRATIO;
            importDemand.Add(Resource.Energy, energyNeed);
        }

        return importDemand;
    }

    // Grows into its bought capacity and returns how much resources it generated this turn
    public virtual float GenerateOutput(float boughtCapacity)
    {
        // Convert boughtCapacity into development
        float outputPerDevelopment = CalculateOutputAtDevelopment(boughtCapacity);
        float boughtDevelopment = boughtCapacity / outputPerDevelopment;

        // Calculate how much total development our energy imports can support
        float energyDevelopment = contractTerminal.CalculateTotalImports(Resource.Energy) / ENERGYTODEVRATIO;
        
        // Limit by the energy imports
        float targetDevelopment = boughtCapacity < energyDevelopment ? boughtCapacity : energyDevelopment;

        // Grow the new development
        Grow(targetDevelopment);

        // Convert from development to resource output
        return CalculateOutputAtDevelopment(totalDevelopment);
    }

    // Determines the final sale price per unit of this IContractEndpoint's exports
    public virtual Dictionary<Resource, float> CalculatePrice()
    {
        // Calculate our energy costs
        float energyCost = contractTerminal.CalculateImportCosts(Resource.Energy);

        // Calculate the output price
        if (totalDevelopment > 0)
        {
            return new Dictionary<Resource, float>() {{producedResource, energyCost / (CalculateOutputAtDevelopment(totalDevelopment))}};
        }
        else
        {
            return new Dictionary<Resource, float>() {{producedResource, 0}};
        }
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    // Returns the contract terminal for this producer
    protected abstract ContractTerminal CreateContractTerminal();

    // Returns the resources we will need to import
    protected virtual List<Resource> GetImportResources()
    {
        // Don't import energy if we use produce it
        if (producedResource != Resource.Energy)
        {
            return new List<Resource>
            {
                Resource.Energy
            };
        }
        else
        {
            return new List<Resource>
            {
                Resource.Energy
            };
        }
    }

    // Returns the amount of output produced at the target development
    // Doesn't actually add any development to tiles
    // If there isn't enough room for the target development it calculates based on full development
    protected abstract float CalculateOutputAtDevelopment(float targetDevelopment);

    // Returns the amount of development required to meet the target output
    // Doesn't actually add any development to tiles
    // If there isn't enough room for the target output it calculates based on full development
    protected abstract float CalculateDevelopmentAtOutput(float targetOutput);
}
