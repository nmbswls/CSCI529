using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(LogOnImport = true)]
public class TaobaoProductList : ScriptableObject
{
    public List<TaobaoProducts> Entities;
}

[System.Serializable]
public class TaobaoProducts
{
    public string Index;
    public string Name;
    public int Cost;
    public string Desp;
    public string EffectDesp;
    public string CardRelate;
    public int LeftInStock;
    public int LevelUnlock;
}
