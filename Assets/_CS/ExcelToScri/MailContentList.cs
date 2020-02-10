using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class MailContentList : ScriptableObject
{
	public List<MailAsset> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
