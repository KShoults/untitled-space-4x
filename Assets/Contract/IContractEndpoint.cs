using System.Collections;
using System.Collections.Generic;

public interface IContractEndpoint
{
    // Calculates the available amount of resources that this IContractEndpoint can produce
    // This method is called at the start of every contract system evaluation 
    float CalculateCapacity();
    void Grow();
}
