using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopItem
{
    public int MaxOwnNum;
    public int TurnStatus;
    public int Cost;

    public string AddCardId;
}
public class ShopMgr : ModuleBase,IShopMgr
{
    IRoleModule mPoleMgr;
    ICardDeckModule mCardMgr;

    public override void Setup()
    {
        mPoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        mCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        FakeItems();
    }

    public List<ShopItem> mItemList = new List<ShopItem>();
    public void FakeItems()
    {

        for(int i = 0; i < 50; i++)
        {
            ShopItem shopItem = new ShopItem();
            shopItem.Cost = i * 50;
            shopItem.TurnStatus = i;
            shopItem.AddCardId = string.Format("item_{0:00}", i + 1);
            mItemList.Add(shopItem);
        }
    }

    public void FakeBuy(int idx)
    {
        if(idx >= mItemList.Count)
        {
            idx = mItemList.Count - 1;
        }
        ShopItem item = mItemList[idx];
        mCardMgr.GainNewCard(item.AddCardId);
    }

    public void GetRandomCards()
    {

    }

}
