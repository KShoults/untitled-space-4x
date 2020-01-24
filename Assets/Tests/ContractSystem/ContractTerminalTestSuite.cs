using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class ContractTerminalTestSuite
    {
        [TestCase(10, 0, 10, ExpectedResult=0)]
        [TestCase(0, 10, 10, ExpectedResult=0)]
        [TestCase(5, 5, 15, ExpectedResult=5)]
        public float CheckForSuppliersReturnsTheCorrectShortage(float totalImports, float totalSupply, float demand)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminal(null, Resource.Minerals, new List<Resource> {Resource.Energy});

            // Add some imports
            contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, totalImports / 2, 0, null, contractTerminal));
            contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, totalImports / 2, 0, null, contractTerminal));

            // Add some suppliers
            if (!contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                contractTerminal.suppliers.Add(Resource.Energy, new SortedSet<Tuple<ContractTerminal, float, float>>());
            }
            // These have to be different somehow, SortedSet doesn't allow duplicates
            contractTerminal.suppliers[Resource.Energy].Add(new Tuple<ContractTerminal, float, float>(null, totalSupply / 2, 0));
            contractTerminal.suppliers[Resource.Energy].Add(new Tuple<ContractTerminal, float, float>(null, totalSupply / 2, 1));

            // Call CheckForSuppliers
            return contractTerminal.CheckForSuppliers(Resource.Energy, demand);
        }
    }
}
