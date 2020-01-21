using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ProfixContentList : ScriptableObject
{
    public List<TVProfixLoader> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
