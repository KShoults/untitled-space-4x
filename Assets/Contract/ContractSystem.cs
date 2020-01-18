using System;
using System.Collections;
using System.Collections.Generic;

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
        //EvaluateContracts(Resource.MilitaryCapacity);
        EvaluateContracts(Resource.Economy);
        EvaluateContracts(Resource.TransportCapacity);
        EvaluateContracts(Resource.MilitaryGoods);
        EvaluateContracts(Resource.ShipParts);
        EvaluateContracts(Resource.CivilianGoods);
        EvaluateContracts(Resource.TransportCapacity);
        EvaluateContracts(Resource.Minerals);
        EvaluateContracts(Resource.Food);
        EvaluateContracts(Resource.Water);
        EvaluateContracts(Resource.Energy);
        GrowDevelopments();
    }

    // Returns a sorted set of industry suppliers for a given resource
    // Item1 is the ContractTerminal, Item2 is the available capacity, Item3 is the cost per unit
    public SortedSet<Tuple<ContractTerminal, float, float>> FindSuppliers(Resource resource)
    {
        SortedSet<Tuple<ContractTerminal, float, float>> suppliers = new SortedSet<Tuple<ContractTerminal, float, float>>();
        foreach(ContractTerminal c in contractTerminalLists[resource])
        {
            suppliers.Add(new Tuple<ContractTerminal, float, float>(c, c.capacity[resource], c.cost[resource]));
        }
        return suppliers;
    }

    // Returns a sorted set of transport hub suppliers for a given resource
    // Item1 is the ContractTerminal, Item2 is the available capacity, Item3 is the cost per unit
    public SortedSet<Tuple<ContractTerminal, float, float>> FindHubSuppliers(Resource resource)
    {
        SortedSet<Tuple<ContractTerminal, float, float>> suppliers = new SortedSet<Tuple<ContractTerminal, float, float>>();
        foreach(ContractTerminal c in contractTerminalLists[Resource.TransportCapacity])
        {
            suppliers.Add(new Tuple<ContractTerminal, float, float>(c, c.capacity[resource], c.cost[resource]));
        }
        return suppliers;
    }

    private void InitializeContractTerminalLists()
    {
        // The order here is important
        // Transport hubs need to be first
        contractTerminalLists = new Dictionary<Resource, List<ContractTerminal>>
        {
            {Resource.TransportCapacity, new List<ContractTerminal>()},
            {Resource.Energy, new List<ContractTerminal>()},
            {Resource.Food, new List<ContractTerminal>()},
            {Resource.Water, new List<ContractTerminal>()},
            {Resource.Minerals, new List<ContractTerminal>()},
            {Resource.CivilianGoods, new List<ContractTerminal>()},
            {Resource.MilitaryGoods, new List<ContractTerminal>()},
            {Resource.ShipParts, new List<ContractTerminal>()},
            {Resource.Economy, new List<ContractTerminal>()},
            {Resource.MilitaryCapacity, new List<ContractTerminal>()}
        };
    }

    // Tells all of the registered ContractTerminals to calculate their capacity and cost for goods
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

    // Tells all of the registered ContractTerminals for a specific resource to reevaluate their contracts
    private void EvaluateContracts(Resource resource)
    {
        foreach (ContractTerminal c in contractTerminalLists[resource])
        {
            c.EvaluateContracts();
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

    // Defines a comparer to create a sorted set of import suppliers
    // that is sorted by the cost per unit of the goods which is the last part of the tuple
    private class ByUnitCost : IComparer<Tuple<ContractTerminal, float, float>>
    {
        public int Compare(Tuple<ContractTerminal, float, float> x, Tuple<ContractTerminal, float, float> y)
        {
            if (x.Item3 == y.Item3)
            {
                return 0;
            }
            else if (x.Item3 < y.Item3)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
