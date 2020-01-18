using System;
using System.Collections;
using System.Collections.Generic;

public interface IContractEndpoint
{
    // Calculates the available amount of resources that this IContractEndpoint can produce
    // This method is called at the start of every contract system evaluation 
    Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);
    // Calculate the cost per unit of buying the resources that this IContractEndpoint produces
    // This method is called just after CalculateCapacity
    Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);
    // Calculates the need for each resource to determine contract reevaluation
    // This method is called after every contract terminal has calculated their capacity and cost
    Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);
    void Grow();
}
