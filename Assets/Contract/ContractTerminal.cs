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
    // The amount of capacity that has been contracted by others this turn
    public Dictionary<Resource, float> boughtCapacity;
    // This ContractTerminals import contracts
    public Dictionary<Resource, List<Contract>> importContracts;
    // This ContractTerminals export contracts
    public Dictionary<Resource, List<Contract>> exportContracts;
    
    // The contract system we are a part of
    protected ContractSystem contractSystem;

    public ContractTerminal(IContractEndpoint owner, Resource resource, List<Resource> importResources)
    {
        this.owner = owner;
        this.resource = resource;
        this.importResources = importResources;
        suppliers = new Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>>();
        capacity = new Dictionary<Resource, float> {{resource, 0}};
        cost = new Dictionary<Resource, float> {{resource, 0}};
        boughtCapacity = new Dictionary<Resource, float>();
        InitializeImportContracts();
        InitializeExportContracts();
        contractSystem = GameManager.gameManager.contractSystem;
        contractSystem.RegisterTerminal(resource, this);
    }

    public virtual void CalculateCapacity()
    {
        boughtCapacity.Clear();
        boughtCapacity.Add(resource, 0);
        // Find our suppliers
        suppliers.Clear();
        foreach (Resource r in importResources)
        {
            suppliers.Add(r, contractSystem.FindHubSuppliers(r));
        }
        capacity = owner.CalculateCapacity(suppliers);
        cost = owner.CalculateCost(suppliers);
    }

    // Reevaluates the 10% (min 5) most expensive current import contracts
    // and evaluates an equal number of new, least expensive import contracts
    public virtual void EvaluateContracts()
    {
        // Select the most expensive contracts to reevaluate
        // This will be done as part of a later issue

        // Get the import demand from the owner
        Dictionary<Resource, float> importDemand = owner.CalculateImportDemand(suppliers);
        
        foreach (Resource r in importResources)
        {
            float resourcesAdded = 0;
            foreach (Tuple<ContractTerminal, float, float> c in suppliers[r])
            {
                float amount = c.Item2 < importDemand[r] - resourcesAdded ? c.Item2 : importDemand[r] - resourcesAdded;
                // Create a contract
                Contract newContract = c.Item1.RequestContract(r, amount, this);
                
                if (newContract.amount > 0)
                {
                    importContracts[r].Add(newContract);
                    resourcesAdded += amount;
                    if (resourcesAdded > importDemand[r])
                    {
                        // We've added enough import contracts
                        break;
                    }
                }
            }
        }
    }

    public virtual Contract RequestContract(Resource resource, float amount, ContractTerminal importer)
    {
        amount = amount < capacity[resource] - boughtCapacity[resource] ? amount : capacity[resource] - boughtCapacity[resource];
        Contract newContract = new Contract(resource, GameManager.gameManager.turnCounter, amount, cost[resource], this, importer);
        if (amount > 0)
        {
            exportContracts[resource].Add(newContract);
            boughtCapacity[resource] += amount;
        }
        return newContract;
    }

    public virtual void FulfillContracts()
    {
        // Grows into its bought capacity and returns how many resources it generated this turn
        float output = owner.GenerateOutput();

        // Reverse the exportContracts' order so that the oldest contracts get fulfilled first
        Contract[] reversedExportContracts = new Contract[exportContracts[resource].Count];
        exportContracts[resource].CopyTo(reversedExportContracts);
        Array.Reverse(reversedExportContracts);

        // Limit the amount of any contracts that we can't fulfill
        foreach (Contract c in reversedExportContracts)
        {
            if (output < c.amount)
            {
                c.amount = output;
                output = 0;
            }
            else
            {
                output -= c.amount;
            }
        }
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