using System;
using System.Collections;
using System.Collections.Generic;

// Defines an employer that creates and exports a basic resource from tile yields
public class BasicIndustry : Employer
{

    public BasicIndustry(Resource resource) : base(resource)
    { }

    /**************************************************************
        IContractEndpoint Member Overrides
    **************************************************************/

    // Estimates the available amount of resources that this IContractEndpoint can produce
    // In this override we sort the tiles before we estimate our resource capacity
    public override Dictionary<Resource, float> EstimateResourceCapacity()
    {
        // Sort the tiles by the order they should be developed
        SortTiles();

        return base.EstimateResourceCapacity();
    }

    /**************************************************************
        Producer Member Overrides
    **************************************************************/

    // Returns the contract terminal for this producer
    protected override ContractTerminal CreateContractTerminal()
    {
        return new ContractTerminal(this, producedResource, GetImportResources());
    }

    // Returns the amount of output produced at the target development
    // Doesn't actually add any development to tiles
    // If there isn't enough room for the target development it calculates based on full development
    protected override float CalculateOutputAtDevelopment(float targetDevelopment)
    {
        float totalResources = 0;
        float developmentToAdd = targetDevelopment;

        // Add up current output
        if (developmentToAdd > 0)
        {
            foreach (Tile t in tiles)
            {
                // If it's developed
                if (tileDevelopments.ContainsKey(t))
                {
                    // Limit to the developmentToAdd
                    float tileDevelopment = tileDevelopments[t] < developmentToAdd ? tileDevelopments[t] : developmentToAdd;
                    
                    // Add the output
                    totalResources += (int)t.resources[producedResource] * tileDevelopment / 100f;
                    developmentToAdd -= tileDevelopment;

                    if (developmentToAdd <= 0)
                    {
                        // We've added all of the development we need
                        break;
                    }
                }
            }
        }

        // Add up projected output
        if (developmentToAdd > 0)
        {
            foreach (Tile t in tiles)
            {
                // If it's developed
                if (tileDevelopments.ContainsKey(t))
                {
                    // If the tile isn't full
                    if (tileDevelopments[t] < 100)
                    {
                        // Find how much room is on the tile
                        float tileDevelopment = 100 - tileDevelopments[t];

                        //Limit to the developmentToAdd
                        tileDevelopment = tileDevelopment < developmentToAdd ? tileDevelopment : developmentToAdd;

                        // Add the projected output
                        totalResources += (int)t.resources[producedResource] * tileDevelopment / 100f;
                        developmentToAdd -= tileDevelopment;
                    }
                }
                else
                {
                    // Limit to the developmentToAdd
                    float tileDevelopment = 100 < developmentToAdd ? 100 : developmentToAdd;

                    // Add the projected output
                    totalResources += (int)t.resources[producedResource] * tileDevelopment / 100;
                    developmentToAdd -= tileDevelopment;
                }

                if (developmentToAdd <= 0)
                {
                    // We've added all of our new development
                    break;
                }
            }
        }

        // Return the total resources
        return totalResources;
    }

    // Returns the amount of development required to meet the target output
    // Doesn't actually add any development to tiles
    // If there isn't enough room for the target output it calculates based on full development
    protected override float CalculateDevelopmentAtOutput(float targetOutput)
    {
        float totalDevelopment = 0;
        float outputToAdd = targetOutput;

        // Add up current development
        if (targetOutput > 0)
        {
            foreach (Tile t in tiles)
            {
                // If it's developed
                if (tileDevelopments.ContainsKey(t))
                {
                    // Find the output
                    float tileOutput = (int)t.resources[producedResource] * tileDevelopments[t] / 100f;

                    // Limit to the outputToAdd
                    tileOutput = tileOutput < outputToAdd ? tileOutput : outputToAdd;
                    
                    // Add the development
                    totalDevelopment += tileOutput / (int)t.resources[producedResource] * 100;
                    outputToAdd -= tileOutput;

                    if (outputToAdd <= 0)
                    {
                        // We've added all of the development we need
                        break;
                    }
                }
            }
        }

        // Add up projected development
        if (targetOutput > 0)
        {
            foreach (Tile t in tiles)
            {
                // If it's developed
                if (tileDevelopments.ContainsKey(t))
                {
                    // If the tile isn't full
                    if (tileDevelopments[t] < 100)
                    {
                        // Find how much room is on the tile
                        float tileDevelopment = 100 - tileDevelopments[t];

                        // Find the output
                        float tileOutput = (int)t.resources[producedResource] * tileDevelopment / 100f;

                        //Limit to the outputToAdd
                        tileOutput = tileOutput < outputToAdd ? tileOutput : outputToAdd;

                        // Add the projected development
                        totalDevelopment += tileOutput / (int)t.resources[producedResource] * 100;
                        outputToAdd -= tileOutput;
                    }
                }
                else
                {
                    // Find the output
                    float tileOutput = (int)t.resources[producedResource];

                    // Limit to the outputToAdd
                    tileOutput = tileOutput < outputToAdd ? tileOutput : outputToAdd;

                    // Add the projected development
                    totalDevelopment += tileOutput / (int)t.resources[producedResource] * 100;
                    outputToAdd -= tileOutput;
                }

                if (outputToAdd <= 0)
                {
                    // We've added all of our new development
                    break;
                }
            }
        }
            
        return totalDevelopment;
    }

    /**************************************************************
        Personal Members
    **************************************************************/

    // Sorts tiles first by their yield of the produced resource
    // then by the yield of their secondary resource
    private void SortTiles()
    {
        // Sort the tiles
        tiles.Sort(delegate(Tile x, Tile y)
        {
            // Compare yields for the produced resource
            if ((int)x.resources[producedResource] > (int)y.resources[producedResource])
            {
                return -1;
            }
            else if ((int)x.resources[producedResource] < (int)y.resources[producedResource])
            {
                return 1;
            }
            else
            {
                // Check for secondary yields
                if (x.resources.Count < y.resources.Count)
                {
                    return -1;
                }
                else if (x.resources.Count > y.resources.Count)
                {
                    return 1;
                }
                else
                {
                    // Find our secondary resources
                    Resource xResource = Resource.None, yResource = Resource.None;
                    foreach (Resource r in x.resources.Keys)
                    {
                        if (r != producedResource)
                        {
                            xResource = r;
                        }
                    }
                    foreach (Resource r in y.resources.Keys)
                    {
                        if (r != producedResource)
                        {
                            yResource = r;
                        }
                    }

                    // Compare secondary yields
                    if ((int)x.resources[xResource] < (int)y.resources[yResource])
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        });
    }
}
