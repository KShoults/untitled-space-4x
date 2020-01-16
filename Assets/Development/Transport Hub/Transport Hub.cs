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
}
