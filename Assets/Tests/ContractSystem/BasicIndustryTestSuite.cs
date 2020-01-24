using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace Tests
{
    public class BasicIndustryTestSuite
    {
        public class CalculateOutputAtDevelopment
        {
            [TestCase(1, 50, 50, ExpectedResult=5)]
            [TestCase(1, 50, 25, ExpectedResult=2.5)]
            [TestCase(1, 50, 75, ExpectedResult=7.5f)]
            [TestCase(2, 50, 75, ExpectedResult=7.5f)]
            [TestCase(5, 495, 510, ExpectedResult=22)]
            [TestCase(5, 500, 510, ExpectedResult=22)]
            public float CalculateOutputAtDevelopmentReturnsCorrectOutput(int numberOfTiles, float startingDevelopment, float targetDevelopment)
            {
                // Make an empty contract system
                GameManager.contractSystem = new ContractSystem();

                // The basic industry for testing
                BasicIndustry basicIndustry = new BasicIndustry(Resource.Energy);

                // Add some tiles to the basic industry with the needed development
                List<Tile> tiles = new List<Tile>();
                Dictionary<Tile, float> tileDevelopments = new Dictionary<Tile, float>();
                float developmentToAdd = startingDevelopment;
                for (int i = 0; i < numberOfTiles; i++)
                {
                    Tile newTile = new Tile();
                    // Make sure tiles are created in sorted order
                    if (i == 0)
                    {
                        newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.High}, {Resource.Water, Yield.Medium}};
                    }
                    else if (i == 1)
                    {
                        newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Medium}};
                    }
                    else
                    {
                        newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Low}};
                    }
                    tiles.Add(newTile);
                    if (developmentToAdd > 0)
                    {
                        tileDevelopments[newTile] = developmentToAdd < 100 ? developmentToAdd : 100;
                        developmentToAdd -= developmentToAdd < 100 ? developmentToAdd : 100;
                    }
                }
                basicIndustry.tiles = tiles;
                basicIndustry.tileDevelopments = tileDevelopments;

                // Use PrivateObject to reach CalculateOutputPerDevelopment
                PrivateObject privateBasicIndustry = new PrivateObject(basicIndustry);

                return (float)privateBasicIndustry.Invoke("CalculateOutputAtDevelopment", new object[] {targetDevelopment});
            }
        }
    }
}