using System.Collections;
using System.Collections.Generic;

// Provides the ability to interface with the contract system
// Requires the owner to be an IContractEndpoint
public class ContractTerminal
{
    // The owner of this contractTerminal
    public IContractEndpoint owner;
    // The resource that the owner produces
    public Resource resource;
    // The available amount of resources that the owner of this can produce
    // This value is recalculated at the start of every contract system evaluation 
    public float capacity;

    public ContractTerminal(Resource resource, IContractEndpoint owner)
    {
        this.resource = resource;
        this.owner = owner;
        GameManager.gameManager.contractSystem.RegisterTerminal(resource, this);
    }

    public void CalculateCapacity()
    {
        capacity = owner.CalculateCapacity();
    }

    public void Grow()
    {
        owner.Grow();
    }
}