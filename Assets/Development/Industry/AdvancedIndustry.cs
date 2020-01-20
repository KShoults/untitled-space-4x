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

    public override Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        // Get the development capacity
        float developmentCapacity = CalculateDevelopmentCapacity(suppliers);

        // Search for a supplier of minerals for this new capacity
        float mineralShortage = developmentCapacity * MINERALTODEVRATIO;
        foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Minerals])
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

        return new Dictionary<Resource, float> {{resource, developmentCapacity * OUTPUTTODEVRATIO}};
    }

    public override Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        float newDevelopment = contractTerminal.capacity[resource];
        // Start with development costs
        float cost = CalculateDevelopmentCosts(suppliers, contractTerminal, newDevelopment);

        // Add mineral costs
        foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
        {
            cost += c.cost * c.amount;
        }
        // Convert from development to input minerals
        float inputCapacity = newDevelopment / MINERALTODEVRATIO;
        // Add future mineral costs
        foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Minerals])
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

        return new Dictionary<Resource, float> {{resource, cost}};
    }

    public override Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        Dictionary<Resource, float> importDemand = CalculateDevelopmentDemand(contractTerminal.boughtCapacity[resource] / OUTPUTTODEVRATIO);

        // Add mineral demand
        importDemand.Add(Resource.Minerals, (totalDevelopment + contractTerminal.boughtCapacity[resource] / OUTPUTTODEVRATIO) * MINERALTODEVRATIO);

        return importDemand;
    }

    public override float GenerateOutput()
    {
        float newDevelopment = contractTerminal.boughtCapacity[resource] / OUTPUTTODEVRATIO;

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
        float cost = CalculateDevelopmentCosts(null, contractTerminal, 0);

        // Add mineral costs
        foreach (Contract c in contractTerminal.importContracts[Resource.Minerals])
        {
            cost += c.cost * c.amount;
        }

        if (totalDevelopment > 0)
        {
            return new Dictionary<Resource, float>() {{resource, cost / (totalDevelopment * OUTPUTTODEVRATIO)}};
        }
        else
        {
            return new Dictionary<Resource, float>() {{resource, cost}};
        }
    }

    protected override List<Resource> GetImportResources()
    {
        List<Resource> importResources = base.GetImportResources();
        
        importResources.Add(Resource.Minerals);

        return importResources;
    }
}
