using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class SuffixContentList : ScriptableObject
{
	public List<TVSuffixLoader> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
