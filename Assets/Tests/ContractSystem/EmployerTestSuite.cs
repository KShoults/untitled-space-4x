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
            
            // Create our producer
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
    }
}
