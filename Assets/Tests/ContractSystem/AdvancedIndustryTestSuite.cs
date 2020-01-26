using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class AdvancedIndustryTestSuite
    {
        [TestCase(0, ExpectedResult=0)]
        [TestCase(15, ExpectedResult=150)]
        public float CalculateImportDemandReturnsCorrectDemand(float targetResourceCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            AdvancedIndustry advancedIndustry = new AdvancedIndustry(Resource.CivilianGoods);

            // Call EstimateCost
            return advancedIndustry.CalculateImportDemand(targetResourceCapacity)[Resource.Minerals];
        }

        [TestCase(new float[] {}, new float[] {}, ExpectedResult=60)]
        [TestCase(new float[] {10}, new float[] {.5f}, ExpectedResult=70)]
        [TestCase(new float[] {100}, new float[] {2}, ExpectedResult=460)]
        public float CalculatePriceReturnsCorrectPrice(float[] mineralImports, float[] mineralImportCosts)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our advanced industry
            AdvancedIndustry advancedIndustry = new AdvancedIndustry(Resource.CivilianGoods);

            // Add some tiles with developments
            for (int i = 0; i < 5; i++)
            {
                advancedIndustry.tiles.Add(new Tile());
            }
            advancedIndustry.tileDevelopments.Add(advancedIndustry.tiles[0], 50);

            // Add the imports
            float[] imports = new float[] {10};
            float[] importCosts = new float[] {1};
            for (int i = 0; i < imports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
            }

            // Add the mineral imports
            for (int i = 0; i < mineralImports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Minerals].Add(new Contract(Resource.Minerals, 0, mineralImports[i], mineralImportCosts[i], null, advancedIndustry.contractTerminal));
            }

            // Call EstimateCost
            return advancedIndustry.CalculatePrice()[Resource.CivilianGoods];
        }

        [TestCase(new float[] {10}, new float[] {1},
                  new float[] {0}, new float[] {0},
                  1, ExpectedResult=11.5f)]
        [TestCase(new float[] {2}, new float[] {1},
                  new float[] {0}, new float[] {2},
                  1, ExpectedResult=3.5f)]
        public float EstimateCostReturnsCorrectCost(float[] mineralImports, float[] mineralImportCosts,
                                                    float[] mineralSuppliers, float[] mineralSupplierCosts, float targetCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our advancedIndustry
            AdvancedIndustry advancedIndustry = new AdvancedIndustry(Resource.CivilianGoods);

            // Add the imports
            float[] imports = new float[] {1, 1};
            float[] importCosts = new float[] {.5f, 1};
            for (int i = 0; i < imports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], importCosts[i], null, advancedIndustry.contractTerminal));
            }
            
            // Add the mineral imports
            for (int i = 0; i < mineralImports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Minerals].Add(new Contract(Resource.Minerals, 0, mineralImports[i], mineralImportCosts[i], null, advancedIndustry.contractTerminal));
            }

            // Add the suppliers
            float[] suppliers = new float[] {1, 1};
            float[] supplierCosts = new float[] {.5f, 1};
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
            for (int i = 0; i < suppliers.Length; i++)
            {
                advancedIndustry.contractTerminal.suppliers[Resource.Energy].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                         new ContractTerminal(null, Resource.Energy, new List<Resource>()),
                                                                         suppliers[i], supplierCosts[i]));
                advancedIndustry.contractTerminal.suppliers[Resource.Water].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                        new ContractTerminal(null, Resource.Water, new List<Resource>()),
                                                                        suppliers[i], supplierCosts[i]));
                advancedIndustry.contractTerminal.suppliers[Resource.Food].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                       new ContractTerminal(null, Resource.Food, new List<Resource>()),
                                                                       suppliers[i], supplierCosts[i]));
            }

            // Add the mineral suppliers
            if (!advancedIndustry.contractTerminal.suppliers.ContainsKey(Resource.Minerals))
            {
                advancedIndustry.contractTerminal.suppliers.Add(Resource.Minerals, GameManager.contractSystem.FindSuppliers(Resource.Minerals));
            }
            for (int i = 0; i < mineralSuppliers.Length; i++)
            {
                advancedIndustry.contractTerminal.suppliers[Resource.Minerals].Add(new System.Tuple<ContractTerminal, float, float>(
                                                                         new ContractTerminal(null, Resource.Minerals, new List<Resource>()),
                                                                         mineralSuppliers[i], mineralSupplierCosts[i]));
            }

            // Call EstimateCost
            return advancedIndustry.EstimateCost(targetCapacity)[Resource.CivilianGoods];
        }

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

        [TestCase(new float[] {20}, .01f, ExpectedResult=.01f)]
        [TestCase(new float[] {20}, .05f, ExpectedResult=.01f)]
        [TestCase(new float[] {.05f}, .0075f, ExpectedResult=.005f)]
        [TestCase(new float[] {.05f}, .0025f, ExpectedResult=.0025f)]
        public float GenerateOutputReturnsCorrectOutput(float[] mineralImports, float boughtCapacity)
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();
            
            // Create our producer
            AdvancedIndustry advancedIndustry = new AdvancedIndustry(Resource.CivilianGoods);

            // Add some tiles 
            for (int i = 0; i < 5; i++)
            {
                advancedIndustry.tiles.Add(new Tile());
            }

            // Add the imports
            float[] imports = new float[] {2};
            for (int i = 0; i < imports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Energy].Add(new Contract(Resource.Energy, 0, imports[i], 0, null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Water].Add(new Contract(Resource.Water, 0, imports[i], 0, null, advancedIndustry.contractTerminal));
                advancedIndustry.contractTerminal.importContracts[Resource.Food].Add(new Contract(Resource.Food, 0, imports[i], 0, null, advancedIndustry.contractTerminal));
            }

            // Add the mineralimports
            for (int i = 0; i < mineralImports.Length; i++)
            {
                advancedIndustry.contractTerminal.importContracts[Resource.Minerals].Add(new Contract(Resource.Minerals, 0, mineralImports[i], 0, null, advancedIndustry.contractTerminal));
            }

            // Call EstimateResourceCapacity
            return advancedIndustry.GenerateOutput(boughtCapacity);
        }
    }
}
