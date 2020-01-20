using System;
using System.Collections;
using System.Collections.Generic;

// This is a special type of contract terminal that transport hubs use
public class HubContractTerminal : ContractTerminal
{
    // The transport hub that owns this contract terminal
    public TransportHub hubOwner;
    // Determines if the first stage of contract evaluations has occured
    // Transport hubs evaluate their contracts in two waves
    public bool completedFirstStageContractEvaluation;
    // Determines if the first stage of contract fulfillment has occured
    // Transport hubs fulfill their export contracts in the first wave
    // They then build their stockpiles with import contracts in the second wave
    public bool completedFirstStageContractFulfillment;
    public HubContractTerminal(TransportHub owner, Resource resource, List<Resource> importResources) : base(owner, resource, importResources)
    {
        hubOwner = owner;

    }

    public override void CalculateCapacity()
    {
        // A couple of flags that need to be reset at the beginning of end turn calculations
        completedFirstStageContractEvaluation = false;
        completedFirstStageContractFulfillment = false;

        boughtCapacity.Clear();
        suppliers.Clear();
        foreach (Resource r in importResources)
        {
            boughtCapacity.Add(r, 0);
        }
        boughtCapacity.Add(Resource.TransportCapacity, 0);
        capacity = hubOwner.CalculateCapacity(suppliers);
        cost = hubOwner.CalculateCost(suppliers);
    }

    public override void EvaluateContracts()
    {
        if (!completedFirstStageContractEvaluation)
        {
            // Now transport hubs find suppliers
            foreach (Resource r in importResources)
            {
                suppliers.Add(r, contractSystem.FindSuppliers(r));
            }
        }
        base.EvaluateContracts();
        completedFirstStageContractEvaluation = true;
    }

    // In this override we selectively increase capacity in the case of industries creating an essential resource we have a shortage of
    // This makes sure we try to develop those industries if at all possible when we need to
    // We are also 
    // This simulates the reduced transport capacity due to each contract

    public override Contract RequestContract(Resource r, float amount, ContractTerminal importer)
    {
        // If this is a energy producing resource and we have a shortage of energy, make sure we provide it what it needs
        if (importer.resource == Resource.Energy && hubOwner.stockpileRatio[Resource.Energy] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Water && capacity[Resource.Water] - boughtCapacity[Resource.Water] < amount)
            {
                capacity[Resource.Water] = amount + boughtCapacity[Resource.Water];
            }
            if (r == Resource.Food && capacity[Resource.Food] - boughtCapacity[Resource.Food] < amount)
            {
                capacity[Resource.Food] = amount + boughtCapacity[Resource.Food];
            }
        }
        // If this is a water producing resource and we have a shortage of water, make sure we provide it what it needs
        if (importer.resource == Resource.Water && hubOwner.stockpileRatio[Resource.Water] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Energy && capacity[Resource.Energy] - boughtCapacity[Resource.Energy] < amount)
            {
                capacity[Resource.Energy] = amount + boughtCapacity[Resource.Energy];
            }
            if (r == Resource.Food && capacity[Resource.Food] - boughtCapacity[Resource.Food] < amount)
            {
                capacity[Resource.Food] = amount + boughtCapacity[Resource.Food];
            }
        }
        // If this is a food producing resource and we have a shortage of food, make sure we provide it what it needs
        if (importer.resource == Resource.Food && hubOwner.stockpileRatio[Resource.Food] < TransportHub.IDEALRATIO)
        {
            if (r == Resource.Energy && capacity[Resource.Energy] - boughtCapacity[Resource.Energy] < amount)
            {
                capacity[Resource.Energy] = amount + boughtCapacity[Resource.Energy];
            }
            if (r == Resource.Water && capacity[Resource.Water] - boughtCapacity[Resource.Water] < amount)
            {
                capacity[Resource.Water] = amount + boughtCapacity[Resource.Water];
            }
        }

        Contract newContract = base.RequestContract(r, amount, importer);

        if (newContract.amount > 0)
        {
            boughtCapacity[Resource.TransportCapacity] += newContract.amount;

            foreach (Resource res in importResources)
            {
                // If there isn't enough transport capacity to fulfill the remaining capacity of this resource
                // then we want to increase the bought capacity so that the difference can't be bought
                float transportCapacityLeft = capacity[Resource.TransportCapacity] - boughtCapacity[Resource.TransportCapacity];
                float capacityLeft = capacity[res] - boughtCapacity[res];
                boughtCapacity[res] = capacityLeft >  transportCapacityLeft ? boughtCapacity[res] : capacity[res] - transportCapacityLeft;
            }
        }

        return newContract;
    }

    public override void FulfillContracts()
    {
        if (!completedFirstStageContractFulfillment)
        {
            // Grows into its bought capacity and returns how much transport capacity it has this turn.
            float output = owner.GenerateOutput();
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
                if (hubOwner.stockpile[c.resource] < c.amount)
                {
                    c.amount = hubOwner.stockpile[c.resource];
                }

                output -= c.amount;
                hubOwner.stockpile[c.resource] -= c.amount;

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
            hubOwner.GrowStockpile();
        }

        completedFirstStageContractFulfillment = true;
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
