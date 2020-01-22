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

    public abstract Dictionary<Resource, float> EstimateResourceCapacity();

    public abstract Dictionary<Resource, float> EstimateCost();

    public virtual Dictionary<Resource, float> CalculateImportDemand(Dictionary<Resource, SortedSet<Tuple<ContractTerminal, float, float>>> suppliers)
    {
        return CalculateDevelopmentDemand(contractTerminal.boughtResourceCapacity[producedResource]);
    }

    public abstract float GenerateOutput();

    public abstract Dictionary<Resource, float> CalculatePrice();

    /**************************************************************
        Personal Members
    **************************************************************/
}
