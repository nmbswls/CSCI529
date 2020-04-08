using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

public class TaobaoItemInfo
{
    public string Name;
    public int Cost;
    public string CardRelate;
    public int LeftInStock = 1;
    public string Desp;
    public string EffectDesp;

    public TaobaoItemInfo(string Name, int Cost, string CardRelate)
    {
        this.Name = Name;
        this.CardRelate = CardRelate;
        this.Cost = Cost;
    }

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp) : this(Name, Cost, CardRelate)
    {
        this.Desp = Desp;
    }

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp, int LeftInStock) : this(Name, Cost, CardRelate, Desp)
    {
        this.LeftInStock = LeftInStock;
    }

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp, int LeftInStock, string EffectDesp) : this(Name, Cost, CardRelate, Desp, LeftInStock)
    {
        this.EffectDesp = EffectDesp;
    }
}


public class TaobaoModule : ModuleBase
{

    IRoleModule pRoleMdl;
    ISkillTreeMgr pSKillMgr;
    ICardDeckModule pCardMgr;
    IUIMgr pUIMgr;

    List<TaobaoItemInfo> productList = new List<TaobaoItemInfo>();
    Dictionary<int, List<TaobaoItemInfo>> levelBindItem = new Dictionary<int, List<TaobaoItemInfo>>();
    Dictionary<string, TaobaoItemInfo> nameToItem = new Dictionary<string, TaobaoItemInfo>();

    bool isUnloaded = true;

    public override void Setup()
    {
        pRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
        pSKillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr>();
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        pUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        LoadProductList();
    }

    public List<TaobaoItemInfo> GetProductList()
    {
        return productList;
    }

    public TaobaoItemInfo GetDetailItem(int index)
    {
        return productList[index];
    }

    public int GetNumberOfProduct()
    {
        return productList.Count;
    }

    public void LoadProductList()
    {
        if (isUnloaded)
        {
            TaobaoProductList productExcel = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<TaobaoProductList>("TaobaoProduct/TaobaoProductList", false);
            List<TaobaoProducts> products = productExcel.Entities;
            foreach (TaobaoProducts p in products)
            {
            
                if (p.LevelUnlock == 0)
                {
                    //当有相同的卡时
                    if (nameToItem.ContainsKey(p.Name))
                    {
                        nameToItem[p.Name].LeftInStock+=p.LeftInStock;
                    }
                    else
                    {
                        TaobaoItemInfo t = new TaobaoItemInfo(p.Name, p.Cost, p.CardRelate, p.Desp, p.LeftInStock, p.EffectDesp);
                        nameToItem.Add(p.Name, t);
                        productList.Add(t);
                    }
                }
                else
                {
                    if (!levelBindItem.ContainsKey(p.LevelUnlock))
                    {
                        levelBindItem[p.LevelUnlock] = new List<TaobaoItemInfo>();
                    }
                    TaobaoItemInfo t = new TaobaoItemInfo(p.Name, p.Cost, p.CardRelate, p.Desp, p.LeftInStock, p.EffectDesp);
                    levelBindItem[p.LevelUnlock].Add(t);
                }
            }
            isUnloaded = false;
        }
    }

    public void LoadProductInDifferentTurn()
    {
        int curTurn = pRoleMdl.GetCurrentTurn();
        if(levelBindItem.ContainsKey(curTurn))
        {
            /**
             * 1. 把卡加进去
             * 2. 显示卡
             * 3. 标出最新的卡
             **/
             // 把卡加进去
            foreach(TaobaoItemInfo t in levelBindItem[curTurn])
            {
                if (nameToItem.ContainsKey(t.Name))
                {
                    nameToItem[t.Name].LeftInStock += t.LeftInStock;
                }
                else
                {
                    nameToItem.Add(t.Name, t);
                    productList.Add(t);
                }
            }
        }
        Debug.Log("当前回合:" + curTurn +",商店物品数:" + productList.Count);
    }

    public void ReduceLeftInStock(int index)
    {
        productList[index].LeftInStock-=1;
    }

    public bool CheckIsEmptyList()
    {
        return productList.Count == 0;
    }

    public bool CheckAvaiableLeftInStock(int index)
    {
        return productList[index].LeftInStock > 0;
    }


}
