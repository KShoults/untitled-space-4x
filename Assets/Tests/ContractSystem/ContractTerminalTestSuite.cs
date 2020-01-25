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
        [TestCase(5, 10, 6, ExpectedResult=0)]
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

        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  1.5f, ExpectedResult=2)]
        [TestCase(new float[] {1, 1.5f, 2}, new float[] {.5f, .75f, 1},
                  new float[] {1}, new float[] {2},
                  2, ExpectedResult=1.25f)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1.5f, 1, 2}, new float[] {.75f, .5f, 1},
                  3.5f, ExpectedResult=2.625f)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  3, ExpectedResult=3)]
        public float EstimateImportCostReturnsCorrectCost(float[] imports, float[] importCosts, float[] suppliers, float [] supplierCosts, float demand)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminal(null, Resource.Minerals, new List<Resource> {Resource.Energy});

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, contractTerminal));
            }

            // Add the suppliers
            if (!contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                // We create a new contract terminal for each supplier because sorted set requires its elements to be unique
                contractTerminal.suppliers[Resource.Energy].Add(new Tuple<ContractTerminal, float, float>(
                                                                new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                                     suppliers[i], supplierCosts[i]));
            }

            // Call EstimateImportCost
            return contractTerminal.EstimateImportCost(Resource.Energy, demand);
        }
    }
}
