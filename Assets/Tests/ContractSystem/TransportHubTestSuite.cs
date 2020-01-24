﻿using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace Tests
{
    public class TransportHubTestSuite
    {
        public class UpdateStockpileRatios
        {
            [TestCase(5, 1, 1, 1, 5, 0)]
            [TestCase(1, 1, .5f, 1, 1, -.5f)]
            [TestCase(1, .5f, 1, 1, 2, 1)]
            [TestCase(.1f, 0, 0, 10, .2f, 0)]
            public void UpdateStockpileRatiosUpdatesValuesCorrectly(float stockpile, float exports, float imports, float totalDevelopment,
                                                                    float expectedRatio, float expectedTrend)
            {
                // Make an empty contract system
                GameManager.contractSystem = new ContractSystem();

                // Make the transport hub we're testing
                TransportHub transportHub = new TransportHub();
                // The resource we are going to be focused on
                Resource r = Resource.Energy;

                // Set up development
                Tile newTile = new Tile();
                transportHub.tiles = new List<Tile>() {newTile};
                transportHub.tileDevelopments = new Dictionary<Tile, float>() {{newTile, totalDevelopment}};
                
                // Set up the transport hub
                transportHub.stockpile[r] = stockpile;

                // Set up the contract terminal import contracts
                float importsToAdd = imports;
                while (importsToAdd > 0)
                {
                    float amount = importsToAdd < 10 ? importsToAdd : 10;
                    transportHub.contractTerminal.importContracts[r].Add(new Contract(r, 0, amount, 0, null, transportHub.contractTerminal));
                    importsToAdd -= amount;
                }

                // Set up the contract terminal export contracts
                float exportsToAdd = exports;
                while (exportsToAdd > 0)
                {
                    float amount = exportsToAdd < 10 ? exportsToAdd : 10;
                    transportHub.contractTerminal.exportContracts[r].Add(new Contract(r, 0, amount, 0, transportHub.contractTerminal, null));
                    exportsToAdd -= amount;
                }

                // Use PrivateObject to reach UpdateStockpileRatios
                PrivateObject privateTransportHub = new PrivateObject(transportHub);

                privateTransportHub.Invoke("UpdateStockpileRatios");

                // Assert the outputs
                NUnit.Framework.Assert.AreEqual(expectedRatio, transportHub.stockpileRatios[r]);
                NUnit.Framework.Assert.AreEqual(expectedTrend, transportHub.stockpileTrends[r]);
            }
        }
    }
}