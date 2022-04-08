using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class genericFilObj : vrObjectProperties
{
    public string path;
    public abstract void setup(string s);
}
