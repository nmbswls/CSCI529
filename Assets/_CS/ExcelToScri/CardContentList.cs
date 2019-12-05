using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(LogOnImport = true)]
[System.Serializable]
public class CardContentList : ScriptableObject
{
    public List<CardAsset> Cards;
}
