using System;
using System.Collections;
using System.Collections.Generic;

public class Palace : IContractEndpoint
{
    // The artificial demand for goods at this palace
    public int civilianDemand, militaryDemand, shipDemand,
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

    public void Grow()
    {
        // Do nothing
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
