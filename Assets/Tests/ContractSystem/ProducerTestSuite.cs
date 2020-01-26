using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class ProducerTestSuite
    {
        private class ProducerMock : Producer
        {
            public ProducerMock(Resource resource) : base(resource)
            { }

            protected override ContractTerminal CreateContractTerminal()
            {
                return new IndustryContractTerminal(this, producedResource, GetImportResources());
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
            ProducerMock producer = new ProducerMock(Resource.Minerals);

            // Call EstimateCost
            return producer.CalculateImportDemand(targetResourceCapacity)[Resource.Energy];
        }

        [TestCase(new float[] {}, new float[] {}, ExpectedResult=0)]
        [TestCase(new float[] {1}, new float[] {.5f}, ExpectedResult=1)]
        [TestCase(new float[] {10}, new float[] {2}, ExpectedResult=40)]
        public float CalculatePriceReturnsCorrectPrice(float[] imports, float[] importCosts)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            ProducerMock producer = new ProducerMock(Resource.Minerals);

            // Add some tiles with developments
            for (int i = 0; i < 5; i++)
            {
                producer.tiles.Add(new Tile());
            }
            producer.tileDevelopments.Add(producer.tiles[0], 50);

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                producer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, producer.contractTerminal));
            }

            // Call EstimateCost
            return producer.CalculatePrice()[Resource.Minerals];
        }

        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  1.5f, ExpectedResult=2 / 1.5f)]
        [TestCase(new float[] {1, 1.5f, 2}, new float[] {.5f, .75f, 1},
                  new float[] {1}, new float[] {2},
                  2, ExpectedResult=1.25f / 2)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1.5f, 1, 2}, new float[] {.75f, .5f, 1},
                  3.5f, ExpectedResult=2.625f / 3.5f)]
        [TestCase(new float[] {1}, new float[] {1},
                  new float[] {1}, new float[] {2},
                  3, ExpectedResult=3 / 3)]
        public float EstimateCostReturnsCorrectCost(float[] imports, float[] importCosts, float[] suppliers, float[] supplierCosts, float targetCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            ProducerMock producer = new ProducerMock(Resource.Minerals);

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                producer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, producer.contractTerminal));
            }

            // Add the suppliers
            if (!producer.contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                producer.contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                producer.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new IndustryContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                        suppliers[i], supplierCosts[i]));
            }

            // Call EstimateCost
            return producer.EstimateCost(targetCapacity)[Resource.Minerals];
        }

        [TestCase(new float[] {0}, new float[] {1},
                  ExpectedResult=1)]
        [TestCase(new float[] {50, 0}, new float[] {1.5f},
                  ExpectedResult=1.5f)]
        [TestCase(new float[] {0}, new float[] {2},
                  ExpectedResult=1)]
        public float EstimateResourceCapacityReturnsTheCorrectCapacity(float[] tileDevelopments, float[] suppliers)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            ProducerMock producer = new ProducerMock(Resource.Minerals);

            // Add the tiles with developments
            for (int i = 0; i < tileDevelopments.Length; i++)
            {
                producer.tiles.Add(new Tile());
                if (tileDevelopments[i] > 0)
                {
                    producer.tileDevelopments.Add(producer.tiles[i], tileDevelopments[i]);
                }
            }

            // Add the suppliers
            if (!producer.contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                producer.contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                producer.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new IndustryContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                        suppliers[i], 0));
            }

            // Call EstimateResourceCapacity
            return producer.EstimateResourceCapacity()[Resource.Minerals];
        }

        [TestCase(new float[] {5}, 2, ExpectedResult=2)]
        [TestCase(new float[] {1}, 2, ExpectedResult=1)]
        public float GenerateOutputReturnsCorrectOutput(float[] imports, float boughtCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            ProducerMock producer = new ProducerMock(Resource.Minerals);

            // Add some tiles 
            for (int i = 0; i < 5; i++)
            {
                producer.tiles.Add(new Tile());
            }

            // Add the imports
            for (int i = 0; i < imports.Length; i++)
            {
                producer.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], 0, null, producer.contractTerminal));
            }

            // Call EstimateResourceCapacity
            return producer.GenerateOutput(boughtCapacity);
        }
    }
}
