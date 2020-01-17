using System;
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
    // The resources that the owner needs to import
    public List<Resource> importResources;
    // The available amount of resources that the owner of this can produce
    // This dictionary is recalculated at the start of every contract system evaluation 
    public Dictionary<Resource, float> capacity;
    // The cost of the owner's current capacity
    // This dictionary is recalculated at the same time as capacity
    public Dictionary<Resource, float> cost;
    // The potential suppliers that can support the new capacity
    // Item1 is the supplier, Item 2 is capacity, Item 3 is cost
    // This dictionary is recalculated at the same time as capacity
    public Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers;
    // 
    public Dictionary<Resource, float> boughtCapacity;
    // This ContractTerminals import contracts
    public Dictionary<Resource, List<Contract>> importContracts;
    // This ContractTerminals export contracts
    public Dictionary<Resource, List<Contract>> exportContracts;
    
    // The contract system we are a part of
    private ContractSystem contractSystem;

    public ContractTerminal(IContractEndpoint owner, Resource resource, List<Resource> importResources)
    {
        this.owner = owner;
        this.resource = resource;
        this.importResources = importResources;
        suppliers = new Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>>();
        capacity = new Dictionary<Resource, float> {{resource, 0}};
        cost = new Dictionary<Resource, float> {{resource, 0}};
        InitializeImportContracts();
        InitializeExportContracts();
        contractSystem = GameManager.gameManager.contractSystem;
        contractSystem.RegisterTerminal(resource, this);
    }

    public virtual void CalculateCapacity()
    {
        // Find our suppliers
        suppliers.Clear();
        foreach (Resource r in importResources)
        {
            suppliers.Add(r, contractSystem.FindHubSuppliers(r));
        }
        capacity = owner.CalculateCapacity(suppliers);
        cost = owner.CalculateCost(suppliers);
    }

    public void Grow()
    {
        owner.Grow();
    }

    protected virtual void InitializeImportContracts()
    {
        importContracts = new Dictionary<Resource, List<Contract>>();
        foreach (Resource r in importResources)
        {
            importContracts.Add(r, new List<Contract>());
        }
    }

    protected virtual void InitializeExportContracts()
    {
        exportContracts = new Dictionary<Resource, List<Contract>>();
        exportContracts.Add(resource, new List<Contract>());
    }
}