using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry
{
    public Resource resource;
    public float development, targetDevelopment;


    public Industry()
    { }

    public Industry(float targetDevelopment) : this()
    {
        this.targetDevelopment = targetDevelopment;
    }
}
