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

    public string ShuxingRelate;
    public int ShuxingValue = 0;

    public int LevelRemove;

    public string Picture;
}

public class TaobaoItemInfo
{
    public string Name;
    public int Cost;
    public string CardRelate;
    public int LeftInStock = 1;
    public string Desp;
    public string EffectDesp;

    public string ShuxingRelate;
    public int ShuxingValue = 0;

    public int LevelRemove = 30;

    public string Picture = "tb0000";

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

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp, int LeftInStock, string EffectDesp, string ShuxingRelate, int ShuxingValue) : this(Name, Cost, CardRelate, Desp, LeftInStock, EffectDesp)
    {
        this.ShuxingRelate = ShuxingRelate;
        this.ShuxingValue = ShuxingValue;
    }

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp, int LeftInStock, string EffectDesp, string ShuxingRelate, int ShuxingValue, int LevelRemove) : this(Name, Cost, CardRelate, Desp, LeftInStock, EffectDesp, ShuxingRelate, ShuxingValue)
    {
        this.LevelRemove = LevelRemove;
    }

    public TaobaoItemInfo(string Name, int Cost, string CardRelate, string Desp, int LeftInStock, string EffectDesp, string ShuxingRelate, int ShuxingValue, int LevelRemove, string Picture) : this(Name, Cost, CardRelate, Desp, LeftInStock, EffectDesp, ShuxingRelate, ShuxingValue, LevelRemove)
    {
        if(Picture!=null && Picture.Length>0)
        {
            this.Picture = Picture;
        }
    }
}


public class TaobaoModule : ModuleBase
{

    IRoleModule pRoleMdl;
    ICardDeckModule pCardMgr;
    IUIMgr pUIMgr;

    List<TaobaoItemInfo> productList = new List<TaobaoItemInfo>();
    Dictionary<int, List<TaobaoItemInfo>> levelBindAddItem = new Dictionary<int, List<TaobaoItemInfo>>();
    Dictionary<string, TaobaoItemInfo> nameToItem = new Dictionary<string, TaobaoItemInfo>();

    Dictionary<int, List<TaobaoItemInfo>> levelBindRemoveItem = new Dictionary<int, List<TaobaoItemInfo>>();

    bool isUnloaded = true;

    public override void Setup()
    {
        pRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
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
                        nameToItem[p.Name].Cost += p.Cost;
                    }
                    else
                    {
                        TaobaoItemInfo t = new TaobaoItemInfo(p.Name, p.Cost, p.CardRelate, p.Desp, p.LeftInStock, p.EffectDesp, p.ShuxingRelate, p.ShuxingValue, p.LevelRemove, p.Picture);
                        nameToItem.Add(p.Name, t);
                        productList.Add(t);
                    }
                }
                else //放到对应回合加入的表里面
                {
                    if (!levelBindAddItem.ContainsKey(p.LevelUnlock))
                    {
                        levelBindAddItem[p.LevelUnlock] = new List<TaobaoItemInfo>();
                    }
                    TaobaoItemInfo t = new TaobaoItemInfo(p.Name, p.Cost, p.CardRelate, p.Desp, p.LeftInStock, p.EffectDesp, p.ShuxingRelate, p.ShuxingValue, p.LevelRemove, p.Picture);
                    levelBindAddItem[p.LevelUnlock].Add(t);
                }

                //放到对应回合移除的表里面
                {
                    if (!levelBindRemoveItem.ContainsKey(p.LevelRemove))
                    {
                        levelBindRemoveItem[p.LevelRemove] = new List<TaobaoItemInfo>();
                    }
                    TaobaoItemInfo t = new TaobaoItemInfo(p.Name, p.Cost, p.CardRelate, p.Desp, p.LeftInStock, p.EffectDesp, p.ShuxingRelate, p.ShuxingValue, p.LevelRemove, p.Picture);
                    levelBindRemoveItem[p.LevelRemove].Add(t);
                }
                
            }
            isUnloaded = false;
        }
    }

    public void LoadProductInDifferentTurn()
    {
        int curTurn = pRoleMdl.GetCurrentTurn();

        //add product
        if(levelBindAddItem.ContainsKey(curTurn))
        {
            /**
             * 1. 把卡加进去
             * 2. 显示卡
             * 3. 标出最新的卡
             **/
             // 把卡加进去
            foreach(TaobaoItemInfo t in levelBindAddItem[curTurn])
            {
                if (nameToItem.ContainsKey(t.Name))
                {
                    nameToItem[t.Name].LeftInStock += t.LeftInStock;
                    nameToItem[t.Name].Cost = t.Cost;
                }
                else
                {
                    nameToItem.Add(t.Name, t);
                    productList.Add(t);
                }
            }
        }

        //remove product
        if(levelBindRemoveItem.ContainsKey(curTurn))
        {
            foreach (TaobaoItemInfo t in levelBindRemoveItem[curTurn])
            {
                if (nameToItem.ContainsKey(t.Name))
                {
                    productList.Remove(nameToItem[t.Name]);
                    //clear the nameToItem map
                    nameToItem.Remove(t.Name);
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

    public void ConfirmBuy(int wantBuyIdx)
    {
        //if(wantBuyIdx<0|| wantBuyIdx >= fakeList.Count)
        if (wantBuyIdx < 0 || wantBuyIdx >= productList.Count)
        {
            return;
        }
        //int cost = fakeList[wantBuyIdx].Cost;
        int cost = GetDetailItem(wantBuyIdx).Cost;
        pRoleMdl.GainMoney(-cost);
        //if(fakeList[wantBuyIdx].LeftInStock > 0)
        if (CheckAvaiableLeftInStock(wantBuyIdx))
        {
            //fakeList[wantBuyIdx].LeftInStock -= 1;
            //pTaobaoMgr.GetProductList()[wantBuyIdx].LeftInStock -= 1;
            ReduceLeftInStock(wantBuyIdx);
        }

        //pCardMgr.GainNewCard(fakeList[wantBuyIdx].CardRelate);
        
    }

    public string GainCard(int wantBuyIdx)
    {
        pCardMgr.GainNewCard(productList[wantBuyIdx].CardRelate);
        CardAsset ca = pCardMgr.GetCardInfo(productList[wantBuyIdx].CardRelate);
        if(ca!=null) return ca.CardName;
        return "";
    }

    public string GainShuxing(int wantBuyIdx)
    {
        int val = productList[wantBuyIdx].ShuxingValue;
        string shuxingInfo = "";
        switch(productList[wantBuyIdx].ShuxingRelate)
        {
            case "jishu":
                pRoleMdl.AddJishu(val);
                shuxingInfo += "技术";
                break;
            case "koucai":
                pRoleMdl.AddKoucai(val);
                shuxingInfo += "口才";
                break;
            case "waiguan":
                pRoleMdl.AddWaiguan(val);
                shuxingInfo += "魅力";
                break;
            case "kangya":
                pRoleMdl.AddKangya(val);
                shuxingInfo += "抗压";
                break;
            case "caiyi":
                pRoleMdl.AddCaiyi(val);
                shuxingInfo += "才艺";
                break;
            default:
                break;
        }
        if (shuxingInfo != "")
        {
            shuxingInfo += " + " + val.ToString();
        }        
        return shuxingInfo;
    }

}
