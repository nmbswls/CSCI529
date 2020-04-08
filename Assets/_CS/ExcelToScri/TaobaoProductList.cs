using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(LogOnImport = true)]
public class TaobaoProductList : ScriptableObject
{
    public List<TaobaoProducts> Entities;
}

