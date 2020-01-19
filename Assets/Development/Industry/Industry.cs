using System;
using System.Collections;
using System.Collections.Generic;

// Defines a development that creates and exports a tangible resource
public abstract class Industry : Development, IContractEndpoint
{
    public ContractTerminal contractTerminal;

    public Industry(Resource resource) : base(resource)
    {
        contractTerminal = new ContractTerminal(this, resource, GetImportResources());
    }

    /**************************************************************
        IContractEndpoint Member Implementations
    **************************************************************/

    public abstract Dictionary<Resource, float> CalculateCapacity(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);

    public abstract Dictionary<Resource, float> CalculateCost(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers);

    public virtual Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        return CalculateDevelopmentDemand(contractTerminal.boughtCapacity[resource]);
    }

    public abstract float GenerateOutput();

    /**************************************************************
        Personal Members
    **************************************************************/
}
