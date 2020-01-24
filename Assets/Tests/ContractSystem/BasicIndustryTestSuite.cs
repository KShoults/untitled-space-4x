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
        [TestCase(1, 50, 5, ExpectedResult=50)]
        [TestCase(1, 50, 2.5f, ExpectedResult=25)]
        [TestCase(1, 50, 7.5f, ExpectedResult=75)]
        [TestCase(2, 50, 7.5f, ExpectedResult=75)]
        [TestCase(5, 495, 23, ExpectedResult=500)]
        [TestCase(5, 500, 23, ExpectedResult=500)]
        public float CalculateDevelopmentAtOutputReturnsCorrectOutput(int numberOfTiles, float startingDevelopment, float targetOutput)
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

            return (float)privateBasicIndustry.Invoke("CalculateDevelopmentAtOutput", new object[] {targetOutput});
        }

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

        [Test]
        public void SortTilesSortsCorrectly()
        {
            // Make an empty contract system
            GameManager.contractSystem = new ContractSystem();

            // The basic industry for testing
            BasicIndustry basicIndustry = new BasicIndustry(Resource.Energy);

            // Add some tiles to the basic industry
            List<Tile> tiles = new List<Tile>();

            Tile newTile = new Tile();
            newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Low}, {Resource.Water, Yield.High}};
            tiles.Add(newTile);

            newTile = new Tile();
            newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Low}, {Resource.Water, Yield.Low}};
            tiles.Add(newTile);

            newTile = new Tile();
            newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Low}};
            tiles.Add(newTile);

            newTile = new Tile();
            newTile.resources = new Dictionary<Resource, Yield>() {{Resource.Energy, Yield.Medium}};
            tiles.Add(newTile);

            basicIndustry.tiles = tiles;

            // Use PrivateObject to reach CalculateOutputPerDevelopment
            PrivateObject privateBasicIndustry = new PrivateObject(basicIndustry);
            privateBasicIndustry.Invoke("SortTiles");

            // Check for the order
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[0].resources[Resource.Energy], Yield.Medium);
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[1].resources[Resource.Energy], Yield.Low);
            NUnit.Framework.Assert.IsFalse(basicIndustry.tiles[1].resources.ContainsKey(Resource.Water));
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[2].resources[Resource.Energy], Yield.Low);
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[2].resources[Resource.Water], Yield.Low);
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[3].resources[Resource.Energy], Yield.Low);
            NUnit.Framework.Assert.AreEqual(basicIndustry.tiles[3].resources[Resource.Water], Yield.High);
        }
    }
}