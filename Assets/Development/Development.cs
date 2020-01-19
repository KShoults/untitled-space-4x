using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Development
{
    // The resource produced by this development
    public Resource resource;
    // The amount of energy needed to run a point of development
    public const float ENERGYTODEVRATIO = .01f;
    // The ratio of population to a point of development
    public const int POPTODEVRATIO = 1000;
    // The amount of food needed to support one person
    public const float FOODTOPOPRATIO = .00001f;
    // The amount of water needed to support one person
    public const float WATERTOPOPRATIO = .00001f;
    // This is the max population that can be moved into this development in a turn
    public const int MAXPOPTOMOVE = 10000;
    // The tiles that are allocated to this development
    public List<Tile> tiles;
    // The development per tile
    public Dictionary<Tile, float> tileDevelopments;
    // The total amount of development in this industry
    public float totalDevelopment
    {
        get
        {
            float total = 0;
            foreach (KeyValuePair<Tile, float> kvp in tileDevelopments)
            {
                total += kvp.Value;
            }
            return total;
        }
    }
    // Population employed by the development
    public ulong population;

    public Development()
    {
        tiles = new List<Tile>();
        tileDevelopments = new Dictionary<Tile, float>();
    }
    public Development(Resource resource) : this()
    {
        this.resource = resource;
    }

    // Calculate the amount of available room for this development
    protected virtual float CalculateDevelopmentCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
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
        developmentCapacity = developmentCapacity <= MAXPOPTOMOVE / POPTODEVRATIO ? developmentCapacity : MAXPOPTOMOVE / POPTODEVRATIO;

        // Check for suppliers for new development and limit our development by what can be procured
        
        // Check for energy
        if (resource != Resource.Energy)
        {
            float energyShortage = developmentCapacity * ENERGYTODEVRATIO;
            foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Energy])
            {
                energyShortage -= s.Item2;
                if (energyShortage < 0)
                {
                    // We've found enough energy for our new development
                    energyShortage = 0;
                    break;
                }
            }
            developmentCapacity -= energyShortage;
        }

        // Check for water
        if (resource != Resource.Water)
        {
            float waterShortage = developmentCapacity * POPTODEVRATIO * WATERTOPOPRATIO;
            foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Water])
            {
                waterShortage -= s.Item2;
                if (waterShortage < 0)
                {
                    // We've found enough energy for our new development
                    waterShortage = 0;
                    break;
                }
            }
            developmentCapacity -= waterShortage;
        }

        // Check for food
        if (resource != Resource.Food)
        {
            float foodShortage = developmentCapacity * POPTODEVRATIO * FOODTOPOPRATIO;
            foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Food])
            {
                foodShortage -= s.Item2;
                if (foodShortage < 0)
                {
                    // We've found enough energy for our new development
                    foodShortage = 0;
                    break;
                }
            }
            developmentCapacity -= foodShortage;
        }

        return developmentCapacity;
    }

    protected float CalculateDevelopmentCosts(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers,
                                              ContractTerminal contractTerminal, float newDevelopment)
    {
        float cost = 0;
        
        // Add energy costs
        if (resource != Resource.Energy)
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Energy])
            {
                cost += c.cost * c.amount;
            }
        }
        
        // Add water costs
        if (resource != Resource.Water)
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Water])
            {
                cost += c.cost * c.amount;
            }
        }
        
        // Add food costs
        if (resource != Resource.Food)
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Food])
            {
                cost += c.cost * c.amount;
            }
        }

        if (newDevelopment > 0)
        {
            // Add future energy costs
            if (resource != Resource.Energy)
            {
                float energyCapacity = newDevelopment * ENERGYTODEVRATIO;
                foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Energy])
                {
                    float capacityToUse = s.Item3 < energyCapacity ? s.Item3 : energyCapacity;
                    cost += s.Item2 * capacityToUse;
                    energyCapacity -= capacityToUse;
                }
            }

            // Add future water costs
            if (resource != Resource.Water)
            {
                float waterCapacity = newDevelopment * POPTODEVRATIO * WATERTOPOPRATIO;
                foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Water])
                {
                    float capacityToUse = s.Item3 < waterCapacity ? s.Item3 : waterCapacity;
                    cost += s.Item2 * capacityToUse;
                    waterCapacity -= capacityToUse;
                }
            }

            // Add future food costs
            if (resource != Resource.Food)
            {
                float foodCapacity = newDevelopment * POPTODEVRATIO * FOODTOPOPRATIO;
                foreach (Tuple<ContractTerminal, float, float> s in suppliers[Resource.Food])
                {
                    float capacityToUse = s.Item3 < foodCapacity ? s.Item3 : foodCapacity;
                    cost += s.Item2 * capacityToUse;
                    foodCapacity -= capacityToUse;
                }
            }
        }

        return cost;
    }

    // Calculates the import demand of this development
    public Dictionary<Resource, float> CalculateDevelopmentDemand(float boughtCapacity)
    {
        Dictionary<Resource, float> developmentDemand = new Dictionary<Resource, float>();

        if (resource != Resource.Energy)
        {
            float energyNeed = (totalDevelopment + boughtCapacity) * ENERGYTODEVRATIO;
            developmentDemand.Add(Resource.Energy, energyNeed);
        }

        if (resource != Resource.Water)
        {
            float waterNeed = (totalDevelopment + boughtCapacity) * WATERTOPOPRATIO * POPTODEVRATIO;
            developmentDemand.Add(Resource.Water, waterNeed);
        }

        if (resource != Resource.Food)
        {
            float foodNeed = (totalDevelopment + boughtCapacity) * FOODTOPOPRATIO * POPTODEVRATIO;
            developmentDemand.Add(Resource.Food, foodNeed);
        }

        return developmentDemand;
    }

    // Grow the development as much as it can limited by desiredDevelopment
    // Returns how much it ended up growing
    public virtual float Grow(float desiredDevelopment, ContractTerminal contractTerminal)
    {
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

        // Calculate how much total development this will give us
        float populationDevelopment = totalPopulation / (float)POPTODEVRATIO;

        // Calculate total energy, water, and food imports
        float totalEnergy = 0, totalWater = 0, totalFood = 0;
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Energy))
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Energy])
            {
                totalEnergy += c.amount;
            }
        }
        else
        {
            totalEnergy = populationDevelopment * ENERGYTODEVRATIO;
        }
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Water))
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Water])
            {
                totalWater += c.amount;
            }
        }
        else
        {
            totalWater = populationDevelopment * WATERTOPOPRATIO * POPTODEVRATIO;
        }
        if (!contractTerminal.exportContracts.ContainsKey(Resource.Food))
        {
            foreach (Contract c in contractTerminal.importContracts[Resource.Food])
            {
                totalFood += c.amount;
            }
        }
        else
        {
            totalFood = populationDevelopment * FOODTOPOPRATIO * POPTODEVRATIO;
        }

        // Calculate how much total development these imports can support
        float energyDevelopment = totalEnergy / ENERGYTODEVRATIO;
        float waterDevelopment = totalWater / WATERTOPOPRATIO / POPTODEVRATIO;
        float foodDevelopment = totalFood / FOODTOPOPRATIO / POPTODEVRATIO;

        // Limit to the most restrictive
        float resourceDevelopment = energyDevelopment < waterDevelopment ? energyDevelopment < foodDevelopment ? energyDevelopment : foodDevelopment : waterDevelopment < foodDevelopment ? waterDevelopment : foodDevelopment;
        // Limit to the populationDevelopment
        float development = populationDevelopment < resourceDevelopment ? populationDevelopment : resourceDevelopment;
        // Subtract totalDevelopment to get just new development
        float newDevelopment = development - totalDevelopment;
        // Limit by desired development
        newDevelopment = newDevelopment < desiredDevelopment ? newDevelopment : desiredDevelopment;
        
        // Add the new population to the development
        population += (uint)(newDevelopment * POPTODEVRATIO);

        // Allocate the new development to the highest priority tile
        float developmentToAdd = newDevelopment;
        foreach (Tile t in tiles)
        {
            // If it isn't developed yet then create it with the new development
            if (!tileDevelopments.ContainsKey(t))
            {
                tileDevelopments.Add(t, developmentToAdd);
            }
            else
            {
                tileDevelopments[t] += developmentToAdd;
            }

            // If we went over 100 development then the extra
            // can be added in the next loop to the next tile
            if (tileDevelopments[t] > 100)
            {
                developmentToAdd = tileDevelopments[t] - 100;
                tileDevelopments[t] = 100;
            }
            else
            {
                // If we didn't go over 100 development then we're done
                break;
            }
        }
        return newDevelopment;
    }

    protected virtual List<Resource> GetImportResources()
    {
        return new List<Resource>
        {
            Resource.Energy,
            Resource.Water,
            Resource.Food
        };
    }
}
