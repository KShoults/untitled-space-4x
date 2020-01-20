﻿using System;
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

    // In this override we just want to additionally increase every resource's boughtCapacity by the same amount
    // This simulates the reduced transport capacity due to each contract

    public override Contract RequestContract(Resource resource, float amount, ContractTerminal importer)
    {
        Contract newContract = base.RequestContract(resource, amount, importer);

        if (newContract.amount > 0)
        {
            foreach (Resource r in importResources)
            {
                boughtCapacity[r] += newContract.amount;
            }
            boughtCapacity[Resource.TransportCapacity] += newContract.amount;
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
