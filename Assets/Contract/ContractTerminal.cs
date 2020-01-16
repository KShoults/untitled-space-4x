using System.Collections;
using System.Collections.Generic;

// Provides the ability to interface with the contract system
// Requires the owner to be an IContractEndpoint
public class ContractTerminal
{
    // The owner of this contractTerminal
    IContractEndpoint owner;
    Resource resource;
    public ContractTerminal(Resource resource, IContractEndpoint owner)
    {
        this.resource = resource;
        this.owner = owner;
        GameManager.gameManager.contractSystem.RegisterTerminal(resource, this);
    }

    public void Grow()
    {
        owner.Grow();
    }
}