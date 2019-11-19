using System.Collections;
using System.Collections.Generic;
using System.IO;

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
        using (StreamReader sr = File.OpenText("Assets/Map/Namelists/Namelist.txt"))
        {
            string name;
            while ((name = sr.ReadLine()) != null)
            {
                newNameList.Add(name);
            }
        }
        return newNameList;
    }
}