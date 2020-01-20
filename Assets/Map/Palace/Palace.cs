using System;
using System.Collections;
using System.Collections.Generic;

public class Palace : IContractEndpoint
{
    // The artificial demand for goods at this palace
    public float civilianDemand, militaryDemand, shipDemand,
        // The total goods received
        civilianTotalReceived, militaryTotalReceived, shipTotalReceived,
        // The goods received last turn
        civilianTurnReceived, militaryTurnReceived, shipTurnReceived,
        // The population sent to developments
        popTotalSent, popTurnSent;
    public ContractTerminal contractTerminal;

    public Palace()
    {
        contractTerminal = new ContractTerminal(this, Resource.Economy, GetImportResources());
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        return new Dictionary<Resource, float>();
    }

    public Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        return new Dictionary<Resource, float>();
    }

    public Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        Dictionary<Resource, float> importDemand = new Dictionary<Resource, float>();

        importDemand.Add(Resource.CivilianGoods, civilianDemand);
        importDemand.Add(Resource.MilitaryGoods, militaryDemand);
        importDemand.Add(Resource.ShipParts, shipDemand);

        return importDemand;
    }

    public float GenerateOutput()
    {
        // Update our resources received values
        militaryTurnReceived = 0;
        foreach (Contract c in contractTerminal.importContracts[Resource.MilitaryGoods])
        {
            militaryTurnReceived += c.amount;
        }
        militaryTotalReceived += militaryTurnReceived;
        civilianTurnReceived = 0;
        foreach (Contract c in contractTerminal.importContracts[Resource.CivilianGoods])
        {
            civilianTurnReceived += c.amount;
        }
        civilianTotalReceived += civilianTurnReceived;
        shipTurnReceived = 0;
        foreach (Contract c in contractTerminal.importContracts[Resource.ShipParts])
        {
            shipTurnReceived += c.amount;
        }
        shipTotalReceived += shipTurnReceived;
        // We don't export
        return 0;
    }

    public Dictionary<Resource, float> CalculatePrice()
    {
        // We don't export
        return new Dictionary<Resource, float>();
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    protected List<Resource> GetImportResources()
    {
        return new List<Resource>
        {
            Resource.CivilianGoods,
            Resource.MilitaryGoods,
            Resource.ShipParts
        };
    }
}
