using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public class DevelopmentTestSuite
    {
        private class DevelopmentMock : Development
        { }

        [TestCase(new float[2] {50, 0}, 150, new float[2] {100, 50})]
        [TestCase(new float[2] {50, 0}, 25, new float[2] {25, 0})]
        [TestCase(new float[3] {0, 50, 0}, 75, new float[3] {25, 50, 0})]
        [TestCase(new float[3] {0, 50, 0}, 25, new float[3] {0, 25, 0})]
        [TestCase(new float[3] {0, 50, 0}, 400, new float[3] {100, 100, 100})]
        public void GrowAllocatesdevelopmentCorrectly(float[] startingDevelopments, float targetDevelopment, float[] tileDevelopmentExpectations)
        {
            // Create the mock development
            DevelopmentMock development = new DevelopmentMock();

            // Add tiles with developments
            List<Tile> tiles = new List<Tile>();
            Dictionary<Tile, float> tileDevelopments = new Dictionary<Tile, float>();
            for (int i = 0; i < startingDevelopments.Length; i++)
            {
                tiles.Add(new Tile());
                tileDevelopments.Add(tiles[i], startingDevelopments[i]);
            }
            development.tiles = tiles;
            development.tileDevelopments = tileDevelopments;

            // Execute the Grow function
            development.Grow(targetDevelopment);

            // Assert the tileDevelopmentExpectations
            for (int i = 0; i < startingDevelopments.Length; i++)
            {
                Assert.AreEqual(tileDevelopmentExpectations[i], development.tileDevelopments[development.tiles[i]]);
            }
        }
    }
}
