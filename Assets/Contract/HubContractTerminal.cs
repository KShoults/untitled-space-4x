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
    public HubContractTerminal(TransportHub owner, Resource resource, List<Resource> importResources) : base(owner, resource, importResources)
    {
        hubOwner = owner;

    }

    public override void CalculateCapacity()
    {
        completedFirstStageContractEvaluation = false;
        boughtCapacity.Clear();
        // Find our suppliers
        suppliers.Clear();
        foreach (Resource r in importResources)
        {
            suppliers.Add(r, contractSystem.FindSuppliers(r));
            boughtCapacity.Add(r, 0);
        }
        capacity = hubOwner.CalculateCapacity(suppliers);
        cost = hubOwner.CalculateCost(suppliers);
    }

    public override void EvaluateContracts()
    {
        base.EvaluateContracts();
        completedFirstStageContractEvaluation = true;
    }

    // In this override we just want to additionally increase every resource's boughtCapacity by the same amount
    public override Contract RequestContract(Resource resource, float amount, ContractTerminal importer)
    {
        amount = amount < capacity[resource] - boughtCapacity[resource] ? amount : capacity[resource] - boughtCapacity[resource];
        Contract newContract = new Contract(resource, amount, cost[resource], this, importer);
        if (amount > 0)
        {
            exportContracts[resource].Add(newContract);
            foreach (Resource r in importResources)
            {
                boughtCapacity[r] += amount;
            }
        }
        return newContract;
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
}
