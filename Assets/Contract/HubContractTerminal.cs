using System;
using System.Collections;
using System.Collections.Generic;

// This is a special type of contract terminal that transport hubs use
public class HubContractTerminal : ContractTerminal
{
    // The transport hub that owns this contract terminal
    public new TransportHub owner;
    public HubContractTerminal(TransportHub owner, Resource resource, List<Resource> importResources) : base(owner, resource, importResources)
    {
        this.owner = owner;

    }

    public override void EstimateResourceCapacity()
    {
        // Clean up outdated fields
        boughtResourceCapacity.Clear();
        suppliers.Clear();
        owner.completedFirstStageContractEvaluation = false;
        owner.completedFirstStageContractFulfillment = false;
        foreach (Resource r in importResources)
        {
            boughtResourceCapacity.Add(r, 0);
        }
        boughtResourceCapacity.Add(Resource.TransportCapacity, 0);

        // Get the estimateCapacity
        resourceCapacity = owner.EstimateResourceCapacity();

        // Get the estimate cost
        cost = owner.EstimateCost(resourceCapacity[producedResource]);
    }

    public override void EvaluateContracts()
    {
        if (!owner.completedFirstStageContractEvaluation)
        {
            // Now transport hubs find suppliers
            foreach (Resource r in importResources)
            {
                suppliers.Add(r, contractSystem.FindSuppliers(r));
            }
        }
        base.EvaluateContracts();
        owner.completedFirstStageContractEvaluation = true;
    }

    // In this override we selectively increase capacity in the case of industries creating an essential resource we have a shortage of
    // This makes sure we try to develop those industries if at all possible when we need to
    // We are also 
    // This simulates the reduced transport capacity due to each contract

    public override Contract RequestContract(Resource r, float amount, ContractTerminal importer)
    {
        // If this is a energy producing resource and we have a shortage of energy, make sure we provide it what it needs
        if (importer.producedResource == Resource.Energy && owner.stockpileRatios[Resource.Energy] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Water && resourceCapacity[Resource.Water] - boughtResourceCapacity[Resource.Water] < amount)
            {
                resourceCapacity[Resource.Water] = amount + boughtResourceCapacity[Resource.Water];
            }
            if (r == Resource.Food && resourceCapacity[Resource.Food] - boughtResourceCapacity[Resource.Food] < amount)
            {
                resourceCapacity[Resource.Food] = amount + boughtResourceCapacity[Resource.Food];
            }
        }
        // If this is a water producing resource and we have a shortage of water, make sure we provide it what it needs
        if (importer.producedResource == Resource.Water && owner.stockpileRatios[Resource.Water] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Energy && resourceCapacity[Resource.Energy] - boughtResourceCapacity[Resource.Energy] < amount)
            {
                resourceCapacity[Resource.Energy] = amount + boughtResourceCapacity[Resource.Energy];
            }
            if (r == Resource.Food && resourceCapacity[Resource.Food] - boughtResourceCapacity[Resource.Food] < amount)
            {
                resourceCapacity[Resource.Food] = amount + boughtResourceCapacity[Resource.Food];
            }
        }
        // If this is a food producing resource and we have a shortage of food, make sure we provide it what it needs
        if (importer.producedResource == Resource.Food && owner.stockpileRatios[Resource.Food] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Energy && resourceCapacity[Resource.Energy] - boughtResourceCapacity[Resource.Energy] < amount)
            {
                resourceCapacity[Resource.Energy] = amount + boughtResourceCapacity[Resource.Energy];
            }
            if (r == Resource.Water && resourceCapacity[Resource.Water] - boughtResourceCapacity[Resource.Water] < amount)
            {
                resourceCapacity[Resource.Water] = amount + boughtResourceCapacity[Resource.Water];
            }
        }

        Contract newContract = base.RequestContract(r, amount, importer);

        if (newContract.amount > 0)
        {
            boughtResourceCapacity[Resource.TransportCapacity] += newContract.amount;

            foreach (Resource res in importResources)
            {
                // If there isn't enough transport capacity to fulfill the remaining capacity of this resource
                // then we want to increase the bought capacity so that the difference can't be bought
                float transportCapacityLeft = resourceCapacity[Resource.TransportCapacity] - boughtResourceCapacity[Resource.TransportCapacity];
                float capacityLeft = resourceCapacity[res] - boughtResourceCapacity[res];
                boughtResourceCapacity[res] = capacityLeft >  transportCapacityLeft ? boughtResourceCapacity[res] : resourceCapacity[res] - transportCapacityLeft;
            }
        }

        return newContract;
    }

    public override void FulfillContracts()
    {
        if (!owner.completedFirstStageContractFulfillment)
        {
            // Grows into its bought capacity and returns how much transport capacity it has this turn.
            float output = owner.GenerateOutput(boughtResourceCapacity[producedResource]);
            // Determines the final price per unit of its exports
            Dictionary<Resource, float> price = owner.CalculatePrice();

            // Add all of the contracts to a sorted set to sort by age and resource
            SortedSet<Contract> sortedOutputContracts = new SortedSet<Contract>(new ByAgeThenResource()); 

            foreach (KeyValuePair<Resource, List<Contract>> kvp in exportContracts)
            {
                foreach (Contract c in kvp.Value)
                {
                    sortedOutputContracts.Add(c);
                }
            }

            // Limit the amount of any contracts that we can't fulfill
            foreach (Contract c in sortedOutputContracts)
            {
                c.cost = price[c.resource];
                if (output < c.amount)
                {
                    c.amount = output;
                }
                if (owner.stockpile[c.resource] < c.amount)
                {
                    c.amount = owner.stockpile[c.resource];
                }

                output -= c.amount;
                owner.stockpile[c.resource] -= c.amount;

                // Cancel any contracts that are reduced to 0
                if (c.amount == 0)
                {
                    c.importer.importContracts[c.resource].Remove(c);
                    exportContracts[c.resource].Remove(c);
                }
            }
        }
        else
        {
            owner.GrowStockpile();
        }

        owner.completedFirstStageContractFulfillment = true;
    }

    // Calculates the total amount of resources exported
    public float CalculateTotalOutput()
    {
        float total = 0;
        foreach (List<Contract> l in exportContracts.Values)
        {
            foreach (Contract c in l)
            {
                total += c.amount;
            }
        }
        return total;
    }

    protected override void InitializeExportContracts()
    {
        exportContracts = new Dictionary<Resource, List<Contract>>();
        exportContracts.Add(Resource.Energy, new List<Contract>());
        exportContracts.Add(Resource.Water, new List<Contract>());
        exportContracts.Add(Resource.Food, new List<Contract>());
        exportContracts.Add(Resource.Minerals, new List<Contract>());
        exportContracts.Add(Resource.CivilianGoods, new List<Contract>());
        exportContracts.Add(Resource.MilitaryGoods, new List<Contract>());
        exportContracts.Add(Resource.ShipParts, new List<Contract>());
    }

    // Defines a comparer to create a sorted set of contracts
    // that is sorted by the age of the contract then by the resource
    private class ByAgeThenResource : IComparer<Contract>
    {
        public int Compare(Contract x, Contract y)
        {
            if (x.startDate == y.startDate)
            {
                if (x.resource == y.resource)
                {
                    return 0;
                }
                if ((int)x.resource < (int)y.resource)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (x.startDate < y.startDate)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
