using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class AdvancedIndustryTestSuite
    {

        [TestCase(new float[] {50}, new float[] {2}, new float[] {6},
                  ExpectedResult=.51f)]
        [TestCase(new float[] {100, 50}, new float[] {1.6f}, new float[] {13},
                  ExpectedResult=1.3f)]
        [TestCase(new float[] {0}, new float[] {2}, new float[] {2},
                  ExpectedResult=.01f)]
        public float EstimateResourceCapacityReturnsTheCorrectCapacity(float[] tileDevelopments, float[] suppliers, float[] mineralSuppliers)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our advanced industry
            AdvancedIndustry advancedIndustry = new AdvancedIndustry(Resource.CivilianGoods);

            // Add the tiles with developments
            for (int i = 0; i < tileDevelopments.Length; i++)
            {
                advancedIndustry.tiles.Add(new Tile());
                if (tileDevelopments[i] > 0)
                {
                    advancedIndustry.tileDevelopments.Add(advancedIndustry.tiles[i], tileDevelopments[i]);
                }
            }

            // Add population
            advancedIndustry.population = (ulong)advancedIndustry.totalDevelopment * Employer.POPTODEVRATIO;

            // Add the suppliers
            if (!advancedIndustry.contractTerminal.suppliers.ContainsKey(Resource.Energy))
            {
                advancedIndustry.contractTerminal.suppliers.Add(Resource.Energy, GameManager.contractSystem.FindSuppliers(Resource.Energy));
            }
            if (!advancedIndustry.contractTerminal.suppliers.ContainsKey(Resource.Water))
            {
                advancedIndustry.contractTerminal.suppliers.Add(Resource.Water, GameManager.contractSystem.FindSuppliers(Resource.Water));
            }
            if (!advancedIndustry.contractTerminal.suppliers.ContainsKey(Resource.Food))
            {
                advancedIndustry.contractTerminal.suppliers.Add(Resource.Food, GameManager.contractSystem.FindSuppliers(Resource.Food));
            }
            if (!advancedIndustry.contractTerminal.suppliers.ContainsKey(Resource.Minerals))
            {
                advancedIndustry.contractTerminal.suppliers.Add(Resource.Minerals, GameManager.contractSystem.FindSuppliers(Resource.Minerals));
            }
            for (int i = 0; i < suppliers.Length; i++)
            {
                advancedIndustry.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                        suppliers[i], 0));
                advancedIndustry.contractTerminal.suppliers[Resource.Water].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Water, new List<Resource>()),
                                                                        suppliers[i], 0));
                advancedIndustry.contractTerminal.suppliers[Resource.Food].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Food, new List<Resource>()),
                                                                        suppliers[i], 0));
                advancedIndustry.contractTerminal.suppliers[Resource.Minerals].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Food, new List<Resource>()),
                                                                        mineralSuppliers[i], 0));
            }

            // Call EstimateResourceCapacity
            return advancedIndustry.EstimateResourceCapacity()[Resource.CivilianGoods];
        }
    }
}
