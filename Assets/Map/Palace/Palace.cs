using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palace
{
    // The artificial demand for goods at this palace
    public int civilianDemand, militaryDemand, shipDemand,
        // The total goods received
        civilianTotalReceived, militaryTotalReceived, shipTotalReceived,
        // The goods received last turn
        civilianTurnReceived, militaryTurnReceived, shipTurnReceived,
        // The population sent to developments
        popTotalSent, popTurnSent;
}
