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
    public Resource producedResource;
    // The turn that this contract terminal was created
    public int startDate = 0;
    // The resources that the owner needs to import
    public List<Resource> importResources;
    // The available amount of resources that the owner of this can produce
    // This dictionary is recalculated at the start of every contract system evaluation 
    public Dictionary<Resource, float> resourceCapacity;
    // The cost of the owner's current capacity
    // This dictionary is recalculated at the same time as capacity
    public Dictionary<Resource, float> cost;
    // The potential suppliers that can support the new capacity
    // Item1 is the supplier, Item 2 is capacity, Item 3 is cost
    // This dictionary is recalculated at the same time as capacity
    public Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers;
    // The amount of capacity that has been contracted by others this turn
    public Dictionary<Resource, float> boughtResourceCapacity;
    // This ContractTerminals import contracts
    public Dictionary<Resource, List<Contract>> importContracts;
    // This ContractTerminals export contracts
    public Dictionary<Resource, List<Contract>> exportContracts;
    
    // The contract system we are a part of
    protected ContractSystem contractSystem;

    public ContractTerminal(IContractEndpoint owner, Resource resource, List<Resource> importResources)
    {
        this.owner = owner;
        this.producedResource = resource;
        this.importResources = importResources;
        suppliers = new Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>>();
        resourceCapacity = new Dictionary<Resource, float> {{resource, 0}};
        cost = new Dictionary<Resource, float> {{resource, 0}};
        boughtResourceCapacity = new Dictionary<Resource, float>();
        InitializeImportContracts();
        InitializeExportContracts();
        contractSystem = GameManager.contractSystem;
        contractSystem.RegisterTerminal(resource, this);
    }

    public virtual void EstimateResourceCapacity()
    {
        // Clean up outdated fields
        boughtResourceCapacity.Clear();
        boughtResourceCapacity.Add(producedResource, 0);
        suppliers.Clear();

        // Find our suppliers
        foreach (Resource r in importResources)
        {
            suppliers.Add(r, contractSystem.FindHubSuppliers(r));
        }
        
        // Get the estimate capacity
        resourceCapacity = owner.EstimateResourceCapacity();

        // Get the estimate cost
        cost = owner.EstimateCost();
    }

    // Reevaluates the 10% (min 5) most expensive current import contracts
    // and evaluates an equal number of new, least expensive import contracts
    public virtual void EvaluateContracts()
    {
        // Select the most expensive contracts to reevaluate
        // This will be done as part of a later issue

        // Get the import demand from the owner
        Dictionary<Resource, float> importDemand = owner.CalculateImportDemand(suppliers);
        
        Resource[] keys = new Resource[importDemand.Count];
        importDemand.Keys.CopyTo(keys, 0);
        foreach (Resource r in keys)
        {
            if (importContracts.ContainsKey(r))
            {
                // Reduce our demand by our existing import contracts' amounts
                foreach (Contract c in importContracts[r])
                {
                    importDemand[r] -= c.amount;
                }
            }
            if (importDemand[r] > 0)
            {
                float resourcesAdded = 0;
                foreach (Tuple<ContractTerminal, float, float> c in suppliers[r])
                {
                    float amount;
                    if (producedResource == Resource.Energy || producedResource == Resource.Water || producedResource == Resource.Food)
                    {
                        // We need to request from the transport hub regardless of reported capacity if we're an essential resource
                        amount = importDemand[r] - resourcesAdded;
                    }
                    else
                    {
                        amount = c.Item2 < importDemand[r] - resourcesAdded ? c.Item2 : importDemand[r] - resourcesAdded;
                    }
                    if (amount > 0)
                    {
                        // Create a contract
                        Contract newContract = c.Item1.RequestContract(r, amount, this);
                        
                        if (newContract.amount > 0)
                        {
                            // See if there already exists a contract with this terminal
                            Contract existingContract = importContracts[r].Find(x => x.exporter == newContract.exporter);

                            if (existingContract == null)
                            {
                                importContracts[r].Add(newContract);
                            }
                            // We don't need an else because the exporter already took care of updating the existing contract

                            resourcesAdded += newContract.amount;
                            if (resourcesAdded > importDemand[r])
                            {
                                // We've added enough import contracts
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public virtual Contract RequestContract(Resource r, float amount, ContractTerminal importer)
    {
        // Limit by the amount of capacity left for that resource
        amount = amount < resourceCapacity[r] - boughtResourceCapacity[r] ? amount : resourceCapacity[r] - boughtResourceCapacity[r];
        Contract newContract = new Contract(r, GameManager.gameManager.turnCounter, amount, cost[r], this, importer);
        if (newContract.amount > 0)
        {
            // See if there already exists a contract with this terminal
            Contract existingContract = exportContracts[r].Find(x => x.importer == newContract.importer);

            if (existingContract == null)
            {
                exportContracts[r].Add(newContract);
            }
            else
            {
                // Calculate the new cost
                // newCost = (oldAmount * oldCost + newAmount * newCost) / (oldAmount + newAmount)
                existingContract.cost = (existingContract.amount * existingContract.cost + newContract.amount * newContract.cost) / (existingContract.amount + newContract.amount);
                existingContract.amount += newContract.amount;
            }
            boughtResourceCapacity[r] += newContract.amount;
        }
        return newContract;
    }

    public virtual void FulfillContracts()
    {
        // Grows into its bought capacity and returns how many resources it generated this turn
        float output = owner.GenerateOutput();
        // Determines the final price per unit of its exports
        Dictionary<Resource, float> price = owner.CalculatePrice();

        // Reverse the exportContracts' order so that the oldest contracts get fulfilled first
        Contract[] reversedExportContracts = new Contract[exportContracts[producedResource].Count];
        exportContracts[producedResource].CopyTo(reversedExportContracts);
        Array.Reverse(reversedExportContracts);

        // Limit the amount of any contracts that we can't fulfill
        foreach (Contract c in reversedExportContracts)
        {
            c.cost = price[c.resource];
            if (output < c.amount)
            {
                c.amount = output;
                output = 0;

                // Cancel any contracts that are reduced to 0
                if (c.amount == 0)
                {
                    c.importer.importContracts[c.resource].Remove(c);
                    exportContracts[c.resource].Remove(c);
                }
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
        exportContracts.Add(producedResource, new List<Contract>());
    }
}