using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        contractTerminal = new ContractTerminal(Resource.Economy, this);
    }

    public void Grow()
    {
        // Do nothing
    }
}
