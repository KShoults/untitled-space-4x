using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace Tests
{
    public class ContractSystemTestSuite
    {
        public class CalculateOutputPerDevelopment
        {
            [TestCase(1, 10, 10, ExpectedResult=.1f)]
            [TestCase(2, 10, 10, ExpectedResult=.1f)]
            [TestCase(3, 195, 15, ExpectedResult=16.2f / 210)]
            [TestCase(5, 495, 10, ExpectedResult=22f / 500)]
            [TestCase(5, 500, 10, ExpectedResult=22f / 500)]
            public float CalculateOutputPerDevelopmentReturnCorrectOutput(int numberOfTiles, float totalDevelopment, float addedDevelopment)
            {
                // Make an empty contract system
                GameManager.contractSystem = new ContractSystem();

                // The basic industry for testing
                BasicIndustry basicIndustry = new BasicIndustry(Resource.Energy);

                // Add some tiles to the basic industry with the needed development
                List<Tile> tiles = new List<Tile>();
                Dictionary<Tile, float> tileDevelopments = new Dictionary<Tile, float>();
                float developmentToAdd = totalDevelopment;
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

                return (float)privateBasicIndustry.Invoke("CalculateOutputPerDevelopment", new object[] {addedDevelopment});
            }
        }
    }
}