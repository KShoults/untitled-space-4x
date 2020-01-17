using System;
using System.Collections;
using System.Collections.Generic;

// This is a special type of contract terminal that transport hubs use
public class HubContractTerminal : ContractTerminal
{
    // The transport hub that owns this contract terminal
    TransportHub hubOwner;
    public HubContractTerminal(TransportHub owner, Resource resource, List<Resource> importResources) : base(owner, resource, importResources)
    {
        hubOwner = owner;

    }

    public override void CalculateCapacity()
    {
        capacity = hubOwner.CalculateCapacity(suppliers);
        cost = hubOwner.CalculateCost(suppliers);
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
