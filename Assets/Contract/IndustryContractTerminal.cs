using System;
using System.Collections;
using System.Collections.Generic;

// This is the most common type of contract terminal that Industries use
public class IndustryContractTerminal : ContractTerminal
{
    public IndustryContractTerminal(IContractEndpoint owner, Resource resource, List<Resource> importResources) : base (owner, resource, importResources)
    {
        boughtResourceCapacity.Add(producedResource, 0);
    }

    // Estimates the resourceCapacity and cost per unit of output of the owner
    // In this override we find our suppliers first
    public override void EstimateResourceCapacity()
    {
        // Find our suppliers
        suppliers.Clear();
        foreach (Resource r in importResources)
        {
            suppliers.Add(r, contractSystem.FindHubSuppliers(r));
        }

        // Get the capacity and cost
        base.EstimateResourceCapacity();
    }
    
    protected override void InitializeExportContracts()
    {
        base.InitializeExportContracts();
        exportContracts.Add(producedResource, new List<Contract>());
    }
}
