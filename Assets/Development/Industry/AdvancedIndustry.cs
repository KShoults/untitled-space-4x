using System;
using System.Collections;
using System.Collections.Generic;

// Defines a development that creates and exports an advanced resource from basic resources
public class AdvancedIndustry : Industry
{
    // The amount of resources created by 1 unity of development
    public const float OUTPUTTODEVRATIO = .01f;
    // The number of minerals used to run 1 development
    public const float MINERALTODEVRATIO = .1f;

    public AdvancedIndustry(Resource resource) : base(resource)
    { }

    // Estimates the available amount of resources that this AdvancedIndustry can produce
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the development capacity
        float developmentCapacity = EstimateDevelopmentCapacity(contractTerminal.suppliers);

        // Determine the amount of minerals we need to run the new capacity
        float mineralShortage = developmentCapacity * MINERALTODEVRATIO;

        // Search for a supplier of minerals for this new capacity
        foreach (Tuple<ContractTerminal, float, float> s in contractTerminal.suppliers[Resource.Minerals])
        {
            mineralShortage -= s.Item2;
            if (mineralShortage < 0)
            {
                // We've found enough minerals for our new development
                mineralShortage = 0;
                break;
            }
        }
        // Limit by the available minerals
        developmentCapacity -= mineralShortage / MINERALTODEVRATIO;

        // Convert from development to resource output
        float resourceCapacity = developmentCapacity * OUTPUTTODEVRATIO;

        // Return the resourceCapacity
        return new Dictionary<Resource, float> {{producedResource, resourceCapacity}};
    }

    public override Dictionary<Resource, float> EstimateCost()
    {
        float newDevelopment = contractTerminal.resourceCapacity[producedResource];
        // Start with development costs
        float cost = CalculateDevelopmentCost(contractTerminal.suppliers, contractTerminal, newDevelopment);

        // Add mineral costs
        foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
        {
            cost += c.cost * c.amount;
        }
        // Convert from development to input minerals
        float inputCapacity = newDevelopment / MINERALTODEVRATIO;
        // Add future mineral costs
        foreach (Tuple<ContractTerminal, float, float> s in contractTerminal.suppliers[Resource.Minerals])
        {
            float capacityToUse = s.Item2 < inputCapacity ? s.Item2 : inputCapacity;
            cost += s.Item3 * capacityToUse;
            inputCapacity -= capacityToUse;
        }

        // Divide by total output
        if (totalDevelopment > 0)
        {
            cost = cost / (totalDevelopment * OUTPUTTODEVRATIO);
        }

        return new Dictionary<Resource, float> {{producedResource, cost}};
    }

    public override Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        Dictionary<Resource, float> importDemand = CalculateDevelopmentDemand(contractTerminal.boughtResourceCapacity[producedResource] / OUTPUTTODEVRATIO);

        // Add mineral demand
        importDemand.Add(Resource.Minerals, (totalDevelopment + contractTerminal.boughtResourceCapacity[producedResource] / OUTPUTTODEVRATIO) * MINERALTODEVRATIO);

        return importDemand;
    }

    public override float GenerateOutput()
    {
        float newDevelopment = contractTerminal.boughtResourceCapacity[producedResource] / OUTPUTTODEVRATIO;

        // Find the total amount of minerals imported
        float totalMinerals = 0;
        foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
        {
            totalMinerals += c.amount;
        }
        // Limit by the amount of minerals
        newDevelopment = newDevelopment < totalMinerals / MINERALTODEVRATIO ? newDevelopment : totalMinerals / MINERALTODEVRATIO;

        Grow(newDevelopment, contractTerminal);

        return totalDevelopment * OUTPUTTODEVRATIO;
    }

    public override Dictionary<Resource, float> CalculatePrice()
    {
        float cost = CalculateDevelopmentCost(null, contractTerminal, 0);

        // Add mineral costs
        foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
        {
            cost += c.cost * c.amount;
        }

        if (totalDevelopment > 0)
        {
            return new Dictionary<Resource, float>() {{producedResource, cost / (totalDevelopment * OUTPUTTODEVRATIO)}};
        }
        else
        {
            return new Dictionary<Resource, float>() {{producedResource, cost}};
        }
    }

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        
        importResources.Add(Resource.Minerals);

        return importResources;
    }
}
