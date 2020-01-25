using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a producer that employs a population
public abstract class Employer : Producer
{
    // The ratio of population to a point of development
    public const int POPTODEVRATIO = 1000;
    // The amount of food needed to support one person
    public const float FOODTOPOPRATIO = .00001f;
    // The amount of water needed to support one person
    public const float WATERTOPOPRATIO = .00001f;
    // This is the max population that can be moved into this development in a turn
    public const int MAXPOPTOMOVE = 1000;
    // Population employed by the development
    public ulong population;



    public Employer(Resource resource) : base(resource)
    { }

    /**************************************************************
        IContractEndpoint Member Overrides
    **************************************************************/

    // Estimates the available amount of resources that this IContractEndpoint can produce
    // In this override we additionaly limit capacity by population movement and water/food requirements
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Get the capacity from producer
        Dictionary<Resource, float> resourceCapacity = base.EstimateResourceCapacity();

        // Determine how much population we can move
        // It will eventually be based on transport costs
        float populationOutputCapacity = CalculateOutputAtDevelopment((population + MAXPOPTOMOVE) / (float)POPTODEVRATIO);

        // Limit by available population
        resourceCapacity[producedResource] = resourceCapacity[producedResource] < populationOutputCapacity ? resourceCapacity[producedResource] : populationOutputCapacity;

        // Limit by available water imports
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Water))
        {
            // Find water suppliers for development
            float waterDemand = (CalculateDevelopmentAtOutput(resourceCapacity[producedResource])) * WATERTOPOPRATIO * POPTODEVRATIO;
            float waterShortage = contractTerminal.CheckForSuppliers(Resource.Water, waterDemand);

            // Reduce by water shortage
            resourceCapacity[producedResource] -= CalculateOutputAtDevelopment(waterShortage / WATERTOPOPRATIO / POPTODEVRATIO);
        }

        // Limit by available food imports
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Food))
        {
            // Find food suppliers for development
            float foodDemand = (CalculateDevelopmentAtOutput(resourceCapacity[producedResource])) * FOODTOPOPRATIO * POPTODEVRATIO;
            float foodShortage = contractTerminal.CheckForSuppliers(Resource.Food, foodDemand);

            // Reduce by food shortage
            resourceCapacity[producedResource] -= CalculateOutputAtDevelopment(foodShortage / FOODTOPOPRATIO / POPTODEVRATIO);
        }

        // Return resource capacity
        return resourceCapacity;
    }

    // Estimates the cost per unit of buying the resources that this IContractEndpoint produces
    // In this override we add in the water and food costs
    public override Dictionary<Resource, float> EstimateCost(float targetResourceCapacity)
    {
        Dictionary<Resource, float> cost = base.EstimateCost(targetResourceCapacity);

        if (targetResourceCapacity > 0)
        {
            // Calculate the development at target resource capacity
            float developmentAtOutput = CalculateDevelopmentAtOutput(targetResourceCapacity);

            // Estimate the water cost
            float waterCost = 0;
            if (!contractTerminal.exportContracts.ContainsKey(Resource.Water))
            {
                waterCost = contractTerminal.EstimateImportCost(Resource.Water, developmentAtOutput * WATERTOPOPRATIO * POPTODEVRATIO);
            }

            // Add the water cost
            cost[producedResource] += waterCost / targetResourceCapacity;

            // Estimate the food cost
            float foodCost = 0;
            if (!contractTerminal.exportContracts.ContainsKey(Resource.Food))
            {
                foodCost = contractTerminal.EstimateImportCost(Resource.Food, developmentAtOutput * FOODTOPOPRATIO * POPTODEVRATIO);
            }

            // Add the water cost
            cost[producedResource] += foodCost / targetResourceCapacity;
        }

        return cost;
    }

    // Calculates the need for each resource to determine contract reevaluation
    // In this override we add demand for water and food
    public override Dictionary<Resource, float> CalculateImportDemand(float targetResourceCapacity)
    {
        Dictionary<Resource, float> importDemand = base.CalculateImportDemand(targetResourceCapacity);

        // Calculate how much development we need
        float development = CalculateDevelopmentAtOutput(targetResourceCapacity);

        // Add water demand
        if (producedResource != Resource.Water)
        {
            float waterNeed = development * WATERTOPOPRATIO * POPTODEVRATIO;
            importDemand.Add(Resource.Water, waterNeed);
        }

        // Add food demand
        if (producedResource != Resource.Food)
        {
            float foodNeed = development * FOODTOPOPRATIO * POPTODEVRATIO;
            importDemand.Add(Resource.Food, foodNeed);
        }

        return importDemand;
    }

    // Grows into its bought capacity and return how much resources it generated this turn
    // In this override we first limit by population and water/food imports
    public override float GenerateOutput(float boughtResourceCapacity)
    {
        // Convert boughtCapacity into development
        float boughtDevelopment = CalculateDevelopmentAtOutput(boughtResourceCapacity);

        // Identify the population source
        // For now we create it from nothing
        
        // Calculate the maximum population we can add to this development
        // Later on we will base this on transport costs
        int populationToAdd = MAXPOPTOMOVE;

        // Limit by the maximum population for this development
        ulong maxPop = (ulong)tiles.Count * 100 * POPTODEVRATIO;
        populationToAdd = (int)(maxPop - population) >= populationToAdd ? populationToAdd : (int)(maxPop - population);

        // Add this to our current population to get the total population we can have
        ulong totalPopulation = population + (ulong)populationToAdd;

        // Calculate how much total development the population will give us
        float populationDevelopment = totalPopulation / (float)POPTODEVRATIO;

        // Limit by the population
        float targetDevelopment = boughtDevelopment < populationDevelopment ? boughtDevelopment : populationDevelopment;

        // Calculate how much total development our water imports can support
        float waterDevelopment = contractTerminal.CalculateTotalImports(Resource.Water) / WATERTOPOPRATIO / POPTODEVRATIO;

        // Limit by our water imports
        targetDevelopment = targetDevelopment < waterDevelopment ? targetDevelopment : waterDevelopment;

        // Calculate how much total development our food imports can support
        float foodDevelopment = contractTerminal.CalculateTotalImports(Resource.Food) / FOODTOPOPRATIO / POPTODEVRATIO;

        // Limit by our food imports
        targetDevelopment = targetDevelopment < foodDevelopment ? targetDevelopment : foodDevelopment;
        
        // Add the new population to the development
        int newPopulation = (int)((targetDevelopment - totalDevelopment) * POPTODEVRATIO);
        if (newPopulation > 0)
        {
            population += (uint)newPopulation;
        }
        else
        {
            population -= (uint)newPopulation;
        }

        return base.GenerateOutput(CalculateOutputAtDevelopment(targetDevelopment));
    }

    // Determines the final sale price per unit of this IContractEndpoint's exports
    // In this override we add the water/food costs to our price
    public override Dictionary<Resource, float> CalculatePrice()
    {
        // Get our current costs
        Dictionary<Resource, float> price = base.CalculatePrice();

        // Calculate our water costs
        float waterCost = contractTerminal.CalculateImportCosts(Resource.Water);

        // Calculate our food costs
        float foodCost = contractTerminal.CalculateImportCosts(Resource.Food);

        // Add our new costs
        if (totalDevelopment > 0)
        {
            float outputAtDevelopment = CalculateOutputAtDevelopment(totalDevelopment);
            price[producedResource] += waterCost / outputAtDevelopment;
            price[producedResource] += foodCost / outputAtDevelopment;
        }

        // Return our new output price
        return price;
    }

    /**************************************************************
        Producer Member Overrides
    **************************************************************/

    // Returns the resources we will need to import
    // In this override we add water and food to the import resources
    protected override List<Resource> GetImportResources()
    {
        // Get our current imports
        List<Resource> importResources = base.GetImportResources();

        // Don't import water if we use produce it
        if (producedResource != Resource.Water)
        {
            importResources.Add(Resource.Water);
        }

        // Don't import food if we use produce it
        if (producedResource != Resource.Food)
        {
            importResources.Add(Resource.Food);
        }

        return importResources;
    }

    /**************************************************************
        Personal Members
    **************************************************************/
}
