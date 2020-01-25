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
                                                                        new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                        suppliers[i], 0));
            }

            // Call EstimateResourceCapacity
            return producer.EstimateResourceCapacity()[Resource.Minerals];
        }
    }
}
