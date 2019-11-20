using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

// Handles name management of clusters for the sector.
public class NameManager
{
    private List<string> nameList;
    private float seed;
    private int nameCount = 0;

    public NameManager(float newSeed, int newNameCount)
    {
        // Generate the name list given the seed
        nameList = GenerateNameList(newSeed);

        // Remove entries that have already been used
        if (newNameCount > 0)
        {
            nameList.RemoveRange(0, newNameCount-1);
        }
        nameCount = newNameCount;
        seed = newSeed;
    }

    public NameManager(float seed) : this(seed, 0)
    {}

    public string GetName()
    {
        // If we run out of names loop the list
        if (nameList.Count == 0)
        {
            nameList = GenerateNameList(seed);
        }
        string name = nameList[0];
        nameList.RemoveAt(0);
        nameCount++;
        return name;
    }

    private List<string> GenerateNameList(float seed)
    {
        List<string> newNameList = new List<string>();
        using (StreamReader sr = File.OpenText("Assets/Map/Namelists/Clusterlist.txt"))
        {
            string name;
            while ((name = sr.ReadLine()) != null)
            {
                newNameList.Add(name);
            }
        }

        // Shuffle the name list using the provided seed
        IEnumerable<string> shuffleIterator = Shuffle(newNameList, seed);
        List<string> shuffledNameList = new List<string>();
        foreach (string s in shuffleIterator)
        {
            shuffledNameList.Add(s);
        }

        return shuffledNameList;
    }

    // Shuffles an enumerable in a repeatable manner
    private static IEnumerable<T> Shuffle<T>(IEnumerable<T> source, float seed)
    {
        System.Random rng = new System.Random((int)Mathf.Floor(seed*1000));
        T[] elements = source.ToArray();
        for (int i = elements.Length - 1; i >= 0; i--)
        {
            // Swap element "i" with a random earlier element it (or itself)
            // ... except we don't really need to swap it fully, as we can
            // return it immediately, and afterwards it's irrelevant.
            int swapIndex = rng.Next(i + 1);
            yield return elements[swapIndex];
            elements[swapIndex] = elements[i];
        }
    }
}