using System;
using System.Collections;
using System.Collections.Generic;

// Defines an employer that creates and exports an advanced resource from basic resources
public class AdvancedIndustry : Employer
{
    // The amount of resources created by 1 unity of development
    public const float OUTPUTTODEVRATIO = .01f;
    // The number of minerals used to run 1 development
    public const float MINERALTODEVRATIO = .1f;

    public AdvancedIndustry(Resource resource) : base(resource)
    { }

    /**************************************************************
        IContractEndpoint Member Overrides
    **************************************************************/

    // Estimates the available amount of resources that this IContractEndpoint can produce
    // In this override we additionaly limit capacity by the mineral requirements 
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the resource capacity
        Dictionary<Resource, float> resourceCapacity = base.EstimateResourceCapacity();

        // Find mineral suppliers for the new capacity
        float mineralDemand = (CalculateDevelopmentAtOutput(resourceCapacity[producedResource])) * MINERALTODEVRATIO;
        float mineralShortage = contractTerminal.CheckForSuppliers(Resource.Minerals, mineralDemand);

        // Reduce by mineral shortage
        resourceCapacity[producedResource] -= CalculateOutputAtDevelopment(mineralShortage / MINERALTODEVRATIO);

        // Return the resourceCapacity
        return resourceCapacity;
    }

    // Estimates the cost per unit of buying the resources that this IContractEndpoint produces
    // In this override we add in the mineral costs
    public override Dictionary<Resource, float> EstimateCost(float targetResourceCapacity)
    {
        Dictionary<Resource, float> cost = base.EstimateCost(targetResourceCapacity);

        if (targetResourceCapacity > 0)
        {
            // Calculate the development at target resource capacity
            float developmentAtOutput = CalculateDevelopmentAtOutput(targetResourceCapacity);

            // Estimate the mineral cost
            float mineralCost = contractTerminal.EstimateImportCost(Resource.Minerals, developmentAtOutput * MINERALTODEVRATIO);

            // Add the mineral cost
            cost[producedResource] += mineralCost / targetResourceCapacity;
        }

        return cost;
    }

    // Calculates the need for each resource to determine contract reevaluation
    // In this override we add demand for minerals
    public override Dictionary<Resource, float> CalculateImportDemand(float targetResourceCapacity)
    {
        Dictionary<Resource, float> importDemand = base.CalculateImportDemand(targetResourceCapacity);

        // Calculate how much development we need
        float development = CalculateDevelopmentAtOutput(targetResourceCapacity);

        // Add mineral demand
        float mineralNeed = development * MINERALTODEVRATIO;
        importDemand.Add(Resource.Minerals, mineralNeed);

        return importDemand;
    }

    // Grows into its bought capacity and return how much resources it generated this turn
    // In this override we first limit by mineral imports
    public override float GenerateOutput(float boughtResourceCapacity)
    {
        // Convert boughtCapacity into development
        float boughtDevelopment = CalculateDevelopmentAtOutput(boughtResourceCapacity);

        // Calculate how much total development our mineral imports can support
        float mineralDevelopment = contractTerminal.CalculateTotalImports(Resource.Minerals) / MINERALTODEVRATIO;

        // Limit by our mineral imports
        float targetDevelopment = boughtDevelopment < mineralDevelopment ? boughtDevelopment : mineralDevelopment;

        return base.GenerateOutput(CalculateOutputAtDevelopment(targetDevelopment));
    }

    // Determines the final sale price per unit of this IContractEndpoint's exports
    // In this override we add the mineral costs to our price
    public override Dictionary<Resource, float> CalculatePrice()
    {
        // Get our current costs
        Dictionary<Resource, float> price = base.CalculatePrice();

        // Calculate our mineral costs
        float mineralCost = contractTerminal.CalculateImportCosts(Resource.Minerals);

        // Add our new costs
        if (totalDevelopment > 0)
        {
            float outputAtDevelopment = CalculateOutputAtDevelopment(totalDevelopment);
            price[producedResource] += mineralCost / outputAtDevelopment;
        }

        // Return our new output price
        return price;
    }

    /**************************************************************
        Producer Member Overrides
    **************************************************************/

    // Returns the contract terminal for this producer
    protected override ContractTerminal CreateContractTerminal()
    {
        return new ContractTerminal(this, producedResource, GetImportResources());
    }


    // Returns the resources we will need to import
    // In this override we add minerals to the import resources
    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        
        importResources.Add(Resource.Minerals);

        return importResources;
    }

    // Returns the amount of output produced at the target development
    protected override float CalculateOutputAtDevelopment(float targetDevelopment)
    {
        return targetDevelopment * OUTPUTTODEVRATIO;
    }

    // Returns the amount of development required to meet the target output
    protected override float CalculateDevelopmentAtOutput(float targetOutput)
    {
        return targetOutput / OUTPUTTODEVRATIO;
    }

    /**************************************************************
        Personal Members
    **************************************************************/
}
