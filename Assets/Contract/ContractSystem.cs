using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractSystem
{
    // A dictionary of the lists of contract terminals used by the contract system
    // The lists' orders should be maintained as the age of the contract terminal determines its priority
    public Dictionary<Resource, List<ContractTerminal>> contractTerminalLists;

    public ContractSystem()
    {
        InitializeContractTerminalLists();
    }

    // Register a new contract terminal
    public void RegisterTerminal(Resource resource, ContractTerminal terminal)
    {
        contractTerminalLists[resource].Add(terminal);
    }

    // Called at the beginning of the end turn calculations to run the entire contract system
    public void EvaluateContracts()
    {
        CalculateCapacities();
        GrowDevelopments();
    }

    private void InitializeContractTerminalLists()
    {
        contractTerminalLists = new Dictionary<Resource, List<ContractTerminal>>
        {
            {Resource.Energy, new List<ContractTerminal>()},
            {Resource.Food, new List<ContractTerminal>()},
            {Resource.Water, new List<ContractTerminal>()},
            {Resource.Minerals, new List<ContractTerminal>()},
            {Resource.CivilianGoods, new List<ContractTerminal>()},
            {Resource.MilitaryGoods, new List<ContractTerminal>()},
            {Resource.ShipParts, new List<ContractTerminal>()},
            {Resource.Economy, new List<ContractTerminal>()},
            {Resource.MilitaryCapacity, new List<ContractTerminal>()},
            {Resource.TransportCapacity, new List<ContractTerminal>()}
        };
    }

    private void CalculateCapacities()
    {
        foreach (KeyValuePair<Resource, List<ContractTerminal>> kvp in contractTerminalLists)
        {
            foreach (ContractTerminal c in kvp.Value)
            {
                c.CalculateCapacity();
            }
        }
    }

    private void GrowDevelopments()
    {
        foreach (KeyValuePair<Resource, List<ContractTerminal>> kvp in contractTerminalLists)
        {
            foreach (ContractTerminal c in kvp.Value)
            {
                c.Grow();
            }
        }
    }
}
