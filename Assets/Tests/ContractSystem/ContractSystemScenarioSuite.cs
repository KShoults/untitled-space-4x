using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    // This test suite tests several contract system scenarios to guarantee the functionality
    // of the contract system across the several most common use cases.
    public class ContractSystemScenarioSuite
    {
        // The first scenario tests the use case of the homeworld developing over the early turns
        // For the first five turns it makes sure that the system grows every turn
        // For the next five turns it makes sure that the system has reached equilibrium
        [Test]
        public void Scenario1Test()
        {
            #region Scenario 1 Setup
            // Create our contract system
            ContractSystem contractSystem = new ContractSystem();
            GameManager.contractSystem = contractSystem;

            // Create an energy industry
            BasicIndustry energyIndustry = new BasicIndustry(Resource.Energy);
            // Add a tile
            energyIndustry.tiles.Add(new Tile(Resource.Energy, Yield.Medium));
            // Add development
            energyIndustry.tileDevelopments.Add(energyIndustry.tiles[0], 1);
            // Add population
            energyIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a water industry
            BasicIndustry waterIndustry = new BasicIndustry(Resource.Water);
            // Add a tile
            waterIndustry.tiles.Add(new Tile(Resource.Water, Yield.Medium));
            // Add development
            waterIndustry.tileDevelopments.Add(waterIndustry.tiles[0], 1);
            // Add population
            waterIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a food industry
            BasicIndustry foodIndustry = new BasicIndustry(Resource.Food);
            // Add a tile
            foodIndustry.tiles.Add(new Tile(Resource.Food, Yield.Medium));
            // Add development
            foodIndustry.tileDevelopments.Add(foodIndustry.tiles[0], 1);
            // Add population
            foodIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a minerals industry
            BasicIndustry mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.Medium));
            // Add development
            mineralsIndustry.tileDevelopments.Add(mineralsIndustry.tiles[0], 79);
            // Add population
            mineralsIndustry.population = 79 * Employer.POPTODEVRATIO;

            // Create a civilian industry
            AdvancedIndustry civilianIndustry = new AdvancedIndustry(Resource.CivilianGoods);
            // Add a tile
            civilianIndustry.tiles.Add(new Tile());
            // Add development
            civilianIndustry.tileDevelopments.Add(civilianIndustry.tiles[0], 46);
            // Add population
            civilianIndustry.population = 46 * Employer.POPTODEVRATIO;

            // Create a transport hub
            TransportHub transportHub = new TransportHub();
            // Add a tile
            transportHub.tiles.Add(new Tile());
            // Add development
            transportHub.tileDevelopments.Add(transportHub.tiles[0], 1);
            // Add population
            transportHub.population = 1 * Employer.POPTODEVRATIO;
            // Add stockpiles
            transportHub.stockpile[Resource.Energy] = .24f;
            transportHub.stockpile[Resource.Water] = .24f;
            transportHub.stockpile[Resource.Food] = .24f;
            transportHub.stockpile[Resource.Minerals] = 20;
            transportHub.stockpile[Resource.CivilianGoods] = 2;

            // Create a palace
            Palace palace = new Palace();
            // Set the palace demand
            palace.civilianDemand = .5f;
            #endregion
            #region Scenario 1 Tests
            #region Scenario 1 Turn 1
            // End the first turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, 0);
            float lastCivilianTurnReceived = palace.civilianTurnReceived;
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, 46);
            float lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, 79);
            float lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            Assert.Greater(foodIndustry.totalDevelopment, 1);
            float lastFoodDevelopment = foodIndustry.totalDevelopment;
            Assert.Greater(waterIndustry.totalDevelopment, 1);
            float lastWaterDevelopment = waterIndustry.totalDevelopment;
            Assert.Greater(energyIndustry.totalDevelopment, 1);
            float lastEnergyDevelopment = energyIndustry.totalDevelopment;
            #endregion
            #region Scenario 1 Turn 2
            // End the second turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
            lastCivilianTurnReceived = palace.civilianTurnReceived;
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            Assert.Greater(foodIndustry.totalDevelopment, lastFoodDevelopment);
            lastFoodDevelopment = foodIndustry.totalDevelopment;
            Assert.Greater(waterIndustry.totalDevelopment, lastWaterDevelopment);
            lastWaterDevelopment = waterIndustry.totalDevelopment;
            Assert.Greater(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            lastEnergyDevelopment = energyIndustry.totalDevelopment;
            #endregion
            #region Scenario 1 Turn 3
            // End the third turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
            lastCivilianTurnReceived = palace.civilianTurnReceived;
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            Assert.Greater(foodIndustry.totalDevelopment, lastFoodDevelopment);
            lastFoodDevelopment = foodIndustry.totalDevelopment;
            Assert.Greater(waterIndustry.totalDevelopment, lastWaterDevelopment);
            lastWaterDevelopment = waterIndustry.totalDevelopment;
            Assert.Greater(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            lastEnergyDevelopment = energyIndustry.totalDevelopment;
            #endregion
            #region Scenario 1 Turn 4
            // End the fourth turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
            lastCivilianTurnReceived = palace.civilianTurnReceived;
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            Assert.Greater(foodIndustry.totalDevelopment, lastFoodDevelopment);
            lastFoodDevelopment = foodIndustry.totalDevelopment;
            Assert.Greater(waterIndustry.totalDevelopment, lastWaterDevelopment);
            lastWaterDevelopment = waterIndustry.totalDevelopment;
            Assert.Greater(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            lastEnergyDevelopment = energyIndustry.totalDevelopment;
            #endregion
            #region Scenario 1 Turn 5
            // End the fifth turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
            lastCivilianTurnReceived = palace.civilianTurnReceived;
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            Assert.Greater(foodIndustry.totalDevelopment, lastFoodDevelopment);
            lastFoodDevelopment = foodIndustry.totalDevelopment;
            Assert.Greater(waterIndustry.totalDevelopment, lastWaterDevelopment);
            lastWaterDevelopment = waterIndustry.totalDevelopment;
            Assert.Greater(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            lastEnergyDevelopment = energyIndustry.totalDevelopment;
            #endregion
            #region Scenario 1 Turns 5-10
            for (int i = 0; i < 5; i++)
            {
                // End turn 5 + i
                contractSystem.EvaluateContractSystem();

                // Assert that the system is in the correct state
                // Check the palace
                Assert.AreEqual(palace.civilianTurnReceived, lastCivilianTurnReceived);
                lastCivilianTurnReceived = palace.civilianTurnReceived;
                // Check the transport hub stockpile ratios
                foreach (Resource r in transportHub.stockpileRatios.Keys)
                {
                    if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                    {
                        Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                        Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                    }
                }
                // Check civilian industry
                Assert.AreEqual(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
                lastCivilianDevelopment = civilianIndustry.totalDevelopment;
                // Check basic industries
                Assert.AreEqual(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
                lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
                Assert.AreEqual(foodIndustry.totalDevelopment, lastFoodDevelopment);
                lastFoodDevelopment = foodIndustry.totalDevelopment;
                Assert.AreEqual(waterIndustry.totalDevelopment, lastWaterDevelopment);
                lastWaterDevelopment = waterIndustry.totalDevelopment;
                Assert.AreEqual(energyIndustry.totalDevelopment, lastEnergyDevelopment);
                lastEnergyDevelopment = energyIndustry.totalDevelopment;
            }
            #endregion
            #endregion
        }

        // The second scenario tests the use case of setting up a new collection of industries
        [Test]
        public void Scenario2Test()
        {
            #region Scenario 2 Setup
            // Create our contract system
            ContractSystem contractSystem = new ContractSystem();
            GameManager.contractSystem = contractSystem;

            // Create an energy industry
            BasicIndustry energyIndustry = new BasicIndustry(Resource.Energy);
            // Add a tile
            energyIndustry.tiles.Add(new Tile(Resource.Energy, Yield.Rare));

            // Create a water industry
            BasicIndustry waterIndustry = new BasicIndustry(Resource.Water);
            // Add a tile
            waterIndustry.tiles.Add(new Tile(Resource.Water, Yield.Rare));

            // Create a food industry
            BasicIndustry foodIndustry = new BasicIndustry(Resource.Food);
            // Add a tile
            foodIndustry.tiles.Add(new Tile(Resource.Food, Yield.Rare));

            // Create minerals industries
            BasicIndustry mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.High));
            mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.High));
            mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.High));
            mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.High));
            mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.High));

            // Create a civilian industry
            AdvancedIndustry civilianIndustry = new AdvancedIndustry(Resource.CivilianGoods);
            // Add a tile
            civilianIndustry.tiles.Add(new Tile());
            civilianIndustry.tiles.Add(new Tile());
            civilianIndustry.tiles.Add(new Tile());
            civilianIndustry.tiles.Add(new Tile());
            civilianIndustry.tiles.Add(new Tile());

            // Create a transport hub
            TransportHub transportHub = new TransportHub();
            // Add a tile
            transportHub.tiles.Add(new Tile());

            // Create a palace
            Palace palace = new Palace();
            // Set the palace demand
            palace.civilianDemand = 10;
            // Add stockpiles
            transportHub.stockpile[Resource.Energy] = .1f;
            transportHub.stockpile[Resource.Water] = .1f;
            transportHub.stockpile[Resource.Food] = .1f;
            #endregion
            #region Scenario 2 Tests
            float lastCivilianTurnReceived = 0;
            for (int i = 0; i < 10; i++)
            {
                // End the turn
                contractSystem.EvaluateContractSystem();

                // Assert that there has been growth
                Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
                lastCivilianTurnReceived = palace.civilianTurnReceived;

                // Assert that the stockpile is at ideal ratio
                foreach (Resource r in transportHub.stockpileRatios.Keys)
                {
                    if (r != Resource.ShipParts && r != Resource.MilitaryGoods)
                    {
                        Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                        Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                    }
                }
            }
            #endregion
        }
        
        // The third scenario tests the use case of demand increasing in a contract system
        [Test]
        public void Scenario3Test()
        {
            #region Scenario 3 Setup
            // Create our contract system
            ContractSystem contractSystem = new ContractSystem();
            GameManager.contractSystem = contractSystem;

            // Create an energy industry
            BasicIndustry energyIndustry = new BasicIndustry(Resource.Energy);
            // Add a tile
            energyIndustry.tiles.Add(new Tile(Resource.Energy, Yield.Medium));
            // Add development
            energyIndustry.tileDevelopments.Add(energyIndustry.tiles[0], 1);
            // Add population
            energyIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a water industry
            BasicIndustry waterIndustry = new BasicIndustry(Resource.Water);
            // Add a tile
            waterIndustry.tiles.Add(new Tile(Resource.Water, Yield.Medium));
            // Add development
            waterIndustry.tileDevelopments.Add(waterIndustry.tiles[0], 1);
            // Add population
            waterIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a food industry
            BasicIndustry foodIndustry = new BasicIndustry(Resource.Food);
            // Add a tile
            foodIndustry.tiles.Add(new Tile(Resource.Food, Yield.Medium));
            // Add development
            foodIndustry.tileDevelopments.Add(foodIndustry.tiles[0], 1);
            // Add population
            foodIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a minerals industry
            BasicIndustry mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.Medium));
            // Add development
            mineralsIndustry.tileDevelopments.Add(mineralsIndustry.tiles[0], 84);
            // Add population
            mineralsIndustry.population = 84 * Employer.POPTODEVRATIO;

            // Create a civilian industry
            AdvancedIndustry civilianIndustry = new AdvancedIndustry(Resource.CivilianGoods);
            // Add a tile
            civilianIndustry.tiles.Add(new Tile());
            // Add development
            civilianIndustry.tileDevelopments.Add(civilianIndustry.tiles[0], 50);
            // Add population
            civilianIndustry.population = 50 * Employer.POPTODEVRATIO;

            // Create a transport hub
            TransportHub transportHub = new TransportHub();
            // Add a tile
            transportHub.tiles.Add(new Tile());
            // Add development
            transportHub.tileDevelopments.Add(transportHub.tiles[0], 1);
            // Add population
            transportHub.population = 1 * Employer.POPTODEVRATIO;
            // Add stockpiles
            transportHub.stockpile[Resource.Energy] = .24f;
            transportHub.stockpile[Resource.Water] = .24f;
            transportHub.stockpile[Resource.Food] = .24f;
            transportHub.stockpile[Resource.Minerals] = 20;
            transportHub.stockpile[Resource.CivilianGoods] = 2;

            // Create a palace
            Palace palace = new Palace();
            // Set the palace demand
            palace.civilianDemand = .5f;
            #endregion
            #region Scenario 3 Tests
            // End the first turn
            contractSystem.EvaluateContractSystem();

            // Find the state of the system
            // Check the palace
            float lastCivilianTurnReceived = palace.civilianTurnReceived;

            // Check civilian industry
            float lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            float lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            float lastFoodDevelopment = foodIndustry.totalDevelopment;
            float lastWaterDevelopment = waterIndustry.totalDevelopment;
            float lastEnergyDevelopment = energyIndustry.totalDevelopment;

            // Increase demand
            palace.civilianDemand = 1;

            // End the second turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Greater(palace.civilianTurnReceived, lastCivilianTurnReceived);
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Greater(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            // Check basic industries
            Assert.Greater(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            Assert.Greater(foodIndustry.totalDevelopment, lastFoodDevelopment);
            Assert.Greater(waterIndustry.totalDevelopment, lastWaterDevelopment);
            Assert.Greater(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            #endregion
        }
        
        // The fourth scenario tests the use case of demand decreasing in a contract system
        [Test]
        public void Scenario4Test()
        {
            #region Scenario 3 Setup
            // Create our contract system
            ContractSystem contractSystem = new ContractSystem();
            GameManager.contractSystem = contractSystem;

            // Create an energy industry
            BasicIndustry energyIndustry = new BasicIndustry(Resource.Energy);
            // Add a tile
            energyIndustry.tiles.Add(new Tile(Resource.Energy, Yield.Medium));
            // Add development
            energyIndustry.tileDevelopments.Add(energyIndustry.tiles[0], 1);
            // Add population
            energyIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a water industry
            BasicIndustry waterIndustry = new BasicIndustry(Resource.Water);
            // Add a tile
            waterIndustry.tiles.Add(new Tile(Resource.Water, Yield.Medium));
            // Add development
            waterIndustry.tileDevelopments.Add(waterIndustry.tiles[0], 1);
            // Add population
            waterIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a food industry
            BasicIndustry foodIndustry = new BasicIndustry(Resource.Food);
            // Add a tile
            foodIndustry.tiles.Add(new Tile(Resource.Food, Yield.Medium));
            // Add development
            foodIndustry.tileDevelopments.Add(foodIndustry.tiles[0], 1);
            // Add population
            foodIndustry.population = 1 * Employer.POPTODEVRATIO;

            // Create a minerals industry
            BasicIndustry mineralsIndustry = new BasicIndustry(Resource.Minerals);
            // Add a tile
            mineralsIndustry.tiles.Add(new Tile(Resource.Minerals, Yield.Medium));
            // Add development
            mineralsIndustry.tileDevelopments.Add(mineralsIndustry.tiles[0], 84);
            // Add population
            mineralsIndustry.population = 84 * Employer.POPTODEVRATIO;

            // Create a civilian industry
            AdvancedIndustry civilianIndustry = new AdvancedIndustry(Resource.CivilianGoods);
            // Add a tile
            civilianIndustry.tiles.Add(new Tile());
            // Add development
            civilianIndustry.tileDevelopments.Add(civilianIndustry.tiles[0], 50);
            // Add population
            civilianIndustry.population = 50 * Employer.POPTODEVRATIO;

            // Create a transport hub
            TransportHub transportHub = new TransportHub();
            // Add a tile
            transportHub.tiles.Add(new Tile());
            // Add development
            transportHub.tileDevelopments.Add(transportHub.tiles[0], 1);
            // Add population
            transportHub.population = 1 * Employer.POPTODEVRATIO;
            // Add stockpiles
            transportHub.stockpile[Resource.Energy] = .24f;
            transportHub.stockpile[Resource.Water] = .24f;
            transportHub.stockpile[Resource.Food] = .24f;
            transportHub.stockpile[Resource.Minerals] = 20;
            transportHub.stockpile[Resource.CivilianGoods] = 2;

            // Create a palace
            Palace palace = new Palace();
            // Set the palace demand
            palace.civilianDemand = .5f;
            #endregion
            #region Scenario 3 Tests
            // End the first turn
            contractSystem.EvaluateContractSystem();

            // Find the state of the system
            // Check the palace
            float lastCivilianTurnReceived = palace.civilianTurnReceived;

            // Check civilian industry
            float lastCivilianDevelopment = civilianIndustry.totalDevelopment;
            // Check basic industries
            float lastMineralsDevelopment = mineralsIndustry.totalDevelopment;
            float lastFoodDevelopment = foodIndustry.totalDevelopment;
            float lastWaterDevelopment = waterIndustry.totalDevelopment;
            float lastEnergyDevelopment = energyIndustry.totalDevelopment;

            // Reduce demand
            palace.civilianDemand = .25f;

            // End the second turn
            contractSystem.EvaluateContractSystem();

            // Assert that the system is in the correct state
            // Check the palace
            Assert.Less(palace.civilianTurnReceived, lastCivilianTurnReceived);
            // Check the transport hub stockpile ratios
            foreach (Resource r in transportHub.stockpileRatios.Keys)
            {
                if (r != Resource.ShipParts && r != Resource.MilitaryCapacity)
                {
                    Assert.Greater(transportHub.stockpileRatios[r], TransportHub.IDEALRATIO);
                    Assert.Less(transportHub.stockpileRatios[r], TransportHub.SURPLUSRATIO);
                }
            }
            // Check civilian industry
            Assert.Less(civilianIndustry.totalDevelopment, lastCivilianDevelopment);
            // Check basic industries
            Assert.Less(mineralsIndustry.totalDevelopment, lastMineralsDevelopment);
            Assert.Less(foodIndustry.totalDevelopment, lastFoodDevelopment);
            Assert.Less(waterIndustry.totalDevelopment, lastWaterDevelopment);
            Assert.Less(energyIndustry.totalDevelopment, lastEnergyDevelopment);
            #endregion
        }
    }
}
