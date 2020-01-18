using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract
{
    // The resource in this contract
    public Resource resource;
    // The amount of resources in this contract
    public float amount;
    // The cost per unit of resources
    public float cost;
    // The ContractTerminal exporting the goods
    public ContractTerminal exporter;
    // The ContractTerminal importing the goods
    public ContractTerminal importer;

    public Contract(Resource resource, float amount, float cost, ContractTerminal exporter, ContractTerminal importer)
    {
        this.resource = resource;
        this.amount = amount;
        this.cost = cost;
        this.exporter = exporter;
        this.importer = importer;
    }
}
