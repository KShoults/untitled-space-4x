using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class ContractTerminalTestSuite
    {
        private class ContractTerminalMock : ContractTerminal
        {
            public ContractTerminalMock(IContractEndpoint owner, Resource resource, List<Resource> importResources) : base(owner, resource, importResources)
            { }

            protected override void InitializeExportContracts()
            {
                base.InitializeExportContracts();
                exportContracts.Add(producedResource, new List<Contract>());
            }
        }

        private class ContractEndpointMock : IContractEndpoint
        {
            public Dictionary<Resource, float> EstimateResourceCapacity()
            {
                return new Dictionary<Resource, float>() {{Resource.Energy, 0}};
            }

            public Dictionary<Resource, float> EstimateCost(float targetResourceCapacity)
            {
                return new Dictionary<Resource, float>();
            }

            public Dictionary<Resource, float> CalculateImportDemand(float targetResourceCapacity)
            {
                return new Dictionary<Resource, float> {{Resource.Water, 1}};
            }

            public float GenerateOutput(float boughtResourceCapacity)
            {
                return 1;
            }

            public Dictionary<Resource, float> CalculatePrice()
            {
                return new Dictionary<Resource, float> {{Resource.Minerals, 1}};
            }
        }

        [TestCase(10, 0, 10, ExpectedResult=0)]
        [TestCase(0, 10, 10, ExpectedResult=0)]
        [TestCase(5, 5, 15, ExpectedResult=5)]
        [TestCase(5, 10, 6, ExpectedResult=0)]
        public float CheckForSuppliersReturnsTheCorrectShortage(float totalImports, float totalSupply, float demand)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminalMock(null, Resource.Minerals, new List<Resource> {Resource.Energy});

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
            ContractTerminal contractTerminal = new ContractTerminalMock(null, Resource.Minerals, new List<Resource> {Resource.Energy});

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
                                                                new IndustryContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                                        suppliers[i], supplierCosts[i]));
            }

            // Call EstimateImportCost
            return contractTerminal.EstimateImportCost(Resource.Energy, demand);
        }

        [Test]
        public void EstimateResourceCapacityCleansBoughtResourceCapacity()
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminalMock(new ContractEndpointMock(), Resource.Energy, new List<Resource>());

            // Set boughtResourceCapacity
            contractTerminal.boughtResourceCapacity.Add(Resource.Energy, 20);

            // Call EstimateResourceCapacity
            contractTerminal.EstimateResourceCapacity();

            // Assert that boughtResourceCapacity was cleaned up
            Assert.AreEqual(0, contractTerminal.boughtResourceCapacity[Resource.Energy]);
        }

        // Demand in this test case is always 1
        [TestCase(.5f, 10, 1, Description="This tests the use case of having more demand than our imports, but with plenty of supply")]
        [TestCase(.5f, 0, .5f, Description="This tests the use case of having more demand than we can supply")]
        public void EvaluateContractsCreatesContractsCorrectly(float imports, float supply, float expectedImports)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminalMock(new ContractEndpointMock(), Resource.Minerals, new List<Resource> {Resource.Water});

            // Set up our supplier
            TransportHub supplier = new TransportHub();
            supplier.tiles.Add(new Tile());
            supplier.stockpile[Resource.Energy] = supply;
            supplier.stockpile[Resource.Water] = supply;
            supplier.stockpile[Resource.Food] = supply;
            supplier.contractTerminal.EstimateResourceCapacity();
            contractTerminal.suppliers[Resource.Water] = GameManager.contractSystem.FindHubSuppliers(Resource.Water);

            // Set our imports
            contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports, 0, null, contractTerminal));

            // Call EvaluateContracts
            contractTerminal.EvaluateContracts();

            // Assert that we have the expected amount of imports
            Assert.AreEqual(expectedImports, contractTerminal.CalculateTotalImports(Resource.Water));
        }

        [Test]
        public void FulfillContractsLimitsContractsThatCantBeFulfilled()
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminalMock(new ContractEndpointMock(), Resource.Minerals, new List<Resource>());

            // Add an export contract
            Contract contract = new Contract(Resource.Minerals, 0, 1.5f, 1, contractTerminal, null);
            contractTerminal.exportContracts[Resource.Minerals].Add(contract);
            contractTerminal.boughtResourceCapacity[Resource.Minerals] = 0;

            // Call FulfillContracts
            // Reminder: the mock outputs 1 unit of minerals at a price of 1
            contractTerminal.FulfillContracts();

            // Assert that the contract has the expected amount
            Assert.AreEqual(1, contract.amount);
        }
        
        [TestCase(.5f, .25f, .25f, Description="This tests the use case of having more capacity than is requested.")]
        [TestCase(.5f, 1, .5f, Description="This tests the use case of having less capacity than is requested.")]
        public void RequestContractReturnsLimitsByItsCapacity(float resourceCapacity, float requestAmount, float expectedAmount)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // Make contract terminal for testing
            ContractTerminal contractTerminal = new ContractTerminalMock(new ContractEndpointMock(), Resource.Minerals, new List<Resource>());
            contractTerminal.resourceCapacity[Resource.Minerals] = resourceCapacity;
            contractTerminal.boughtResourceCapacity[Resource.Minerals] = 0;

            // Call RequestContract
            Contract contract = contractTerminal.RequestContract(Resource.Minerals, requestAmount, null);

            // Assert that the contract has the expected amount
            Assert.AreEqual(expectedAmount, contract.amount);
        }
    }
}
