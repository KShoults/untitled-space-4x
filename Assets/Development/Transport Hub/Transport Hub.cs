using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportHub : Development, IContractEndpoint
{
    public ContractTerminal contractTerminal;
    public TransportHub(Resource resource) : base()
    {
        this.resource = resource;
        contractTerminal = new ContractTerminal(resource, this);
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public float CalculateCapacity()
    {
        // Get the maximum development capacity
        float developmentCapacity = CalculateDevelopmentCapacity();

        // Search for transport hubs to supply this new capacity
        // For now we don't need supplies

        return developmentCapacity;
    }
}
