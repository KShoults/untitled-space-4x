using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EmployerTestSuite
    {
        private class EmployerMock : Employer
        {
            public EmployerMock(Resource resource) : base(resource)
            { }

            protected override ContractTerminal CreateContractTerminal()
            {
                return new ContractTerminal(this, producedResource, GetImportResources());
            }

            protected override float CalculateDevelopmentAtOutput(float targetOutput)
            {
                return targetOutput * 100;
            }

            protected override float CalculateOutputAtDevelopment(float targetDevelopment)
            {
                return targetDevelopment / 100f;
            }
        }

        [TestCase(0, ExpectedResult=0)]
        [TestCase(15, ExpectedResult=15)]
        public float CalculateImportDemandReturnsCorrectDemand(float targetResourceCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            EmployerMock employer = new EmployerMock(Resource.Minerals);

            // Call EstimateCost
            return employer.CalculateImportDemand(targetResourceCapacity)[Resource.Water];
        }

        [TestCase(new float[] {}, new float[] {}, ExpectedResult=0)]
        [TestCase(new float[] {1}, new float[] {.5f}, ExpectedResult=3)]
        [TestCase(new float[] {10}, new float[] {2}, ExpectedResult=120)]
        public float CalculatePriceReturnsCorrectPrice(float[] imports, float[] importCosts)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            EmployerMock employer = new EmployerMock(Resource.Minerals);

            // Add some tiles with developments
            for (int i = 0; i < 5; i++)
            {
                employer.tiles.Add(new Tile());
            }
            employer.tileDevelopments.Add(employer.tiles[0], 50);

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                employer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], importCosts[i], null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], importCosts[i], null, employer.contractTerminal));
            }

            // Call EstimateCost
            return employer.CalculatePrice()[Resource.Minerals];
        }

        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  1.5f, ExpectedResult=2 * 3 / 1.5f)]
        [TestCase(new float[] {1, 1.5f, 2}, new float[] {.5f, .75f, 1},
                  new float[] {1}, new float[] {2},
                  2, ExpectedResult=1.25f * 3 / 2)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1.5f, 1, 2}, new float[] {.75f, .5f, 1},
                  3.5f, ExpectedResult=2.625f * 3 / 3.5f)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  3, ExpectedResult=3 * 3 / 3)]
        public float EstimateCostReturnsCorrectCost(float[] imports, float[] importCosts, float[] suppliers, float[] supplierCosts, float targetCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our employer
            EmployerMock employer = new EmployerMock(Resource.Minerals);

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                employer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], importCosts[i], null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], importCosts[i], null, employer.contractTerminal));
            }

            // Add the suppliers
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                employer.contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }

            // Add the suppliers
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Water))
            {
                employer.contractTerminal.suppliers.Add(Resource.Water, GameManager.contractSystem.FindSuppliers(Resource.Water));
            }

            // Add the suppliers
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Food))
            {
                employer.contractTerminal.suppliers.Add(Resource.Food, GameManager.contractSystem.FindSuppliers(Resource.Food));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                employer.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                         new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                         suppliers[i], supplierCosts[i]));
                employer.contractTerminal.suppliers[Resource.Water].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Water, new List<Resource>()),
                                                                        suppliers[i], supplierCosts[i]));
                employer.contractTerminal.suppliers[Resource.Food].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                       new ContractTerminal(null, Resource.Food, new List<Resource>()),
                                                                       suppliers[i], supplierCosts[i]));
            }

            // Call EstimateCost
            return employer.EstimateCost(targetCapacity)[Resource.Minerals];
        }

        [TestCase(new float[] {50}, new float[] {2},
                  ExpectedResult=.51f)]
        [TestCase(new float[] {100, 50}, new float[] {1.2f},
                  ExpectedResult=1.2f)]
        [TestCase(new float[] {0}, new float[] {2},
                  ExpectedResult=.01f)]
        public float EstimateResourceCapacityReturnsTheCorrectCapacity(float[] tileDevelopments, float[] suppliers)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our employer
            EmployerMock employer = new EmployerMock(Resource.Minerals);

            // Add the tiles with developments
            for (int i = 0; i < tileDevelopments.Length; i++)
            {
                employer.tiles.Add(new Tile());
                if (tileDevelopments[i] > 0)
                {
                    employer.tileDevelopments.Add(employer.tiles[i], tileDevelopments[i]);
                }
            }

            // Add population
            employer.population = (ulong)employer.totalDevelopment * Employer.POPTODEVRATIO;

            // Add the suppliers
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                employer.contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Water))
            {
                employer.contractTerminal.suppliers.Add(Resource.Water, GameManager.contractSystem.FindSuppliers(Resource.Water));
            }
            if (!employer.contractTerminal.suppliers.ContainsKey(Resource.Food))
            {
                employer.contractTerminal.suppliers.Add(Resource.Food, GameManager.contractSystem.FindSuppliers(Resource.Food));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                employer.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                        suppliers[i], 0));
                employer.contractTerminal.suppliers[Resource.Water].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Water, new List<Resource>()),
                                                                        suppliers[i], 0));
                employer.contractTerminal.suppliers[Resource.Food].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Food, new List<Resource>()),
                                                                        suppliers[i], 0));
            }

            // Call EstimateResourceCapacity
            return employer.EstimateResourceCapacity()[Resource.Minerals];
        }

        [TestCase(new float[] {2}, .01f, ExpectedResult=.01f)]
        [TestCase(new float[] {2}, .05f, ExpectedResult=.01f)]
        [TestCase(new float[] {.005f}, .0075f, ExpectedResult=.005f)]
        [TestCase(new float[] {.005f}, .0025f, ExpectedResult=.0025f)]
        public float GenerateOutputReturnsCorrectOutput(float[] imports, float boughtCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            EmployerMock employer = new EmployerMock(Resource.Minerals);

            // Add some tiles 
            for (int i = 0; i < 5; i++)
            {
                employer.tiles.Add(new Tile());
            }

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                employer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], 0, null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], 0, null, employer.contractTerminal));
                employer.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], 0, null, employer.contractTerminal));
            }

            // Call EstimateResourceCapacity
            return employer.GenerateOutput(boughtCapacity);
        }
    }
}
