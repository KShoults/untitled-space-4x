using System;
using System.Collections;
using System.Collections.Generic;

public interface IContractEndpoint
{
    // Estimates the available amount of resources that this IContractEndpoint can produce
    // This method is called at the start of every contract system evaluation 
    Dictionary<Resource, float> EstimateResourceCapacity();
    // Estimates the cost per unit of buying the resources that this IContractEndpoint produces
    // This method is called just after CalculateCapacity
    Dictionary<Resource, float> EstimateCost();
    // Calculates the need for each resource to determine contract reevaluation
    // This method is called after every contract terminal has calculated their capacity and cost
    Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);
    // Grows into its bought capacity and return how much resources it generated this turn
    // This method is called at the end of the contract system evaluation during contract fulfillment
    float GenerateOutput();
    // Determines the final sale price of this IContractEndpoint's exports
    // This method is called just after GenerateOutput
    Dictionary<Resource, float> CalculatePrice();
}
