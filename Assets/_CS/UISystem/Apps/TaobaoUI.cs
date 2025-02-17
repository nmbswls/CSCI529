﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaobaoView : BaseView
{
    public Transform ItemContainer;
    public List<TaobaoItemView> ItemList = new List<TaobaoItemView>();

    public Button Close;

    public Transform DetailPanel;
    public Text DetailDesp;
    public Text DetailName;
    public Text DetailEffectDesp;

    public Button NextPage;
    public Button PrePage;
    public Text PageNum;

    public Button Buy;

    public Image ConfirmBuyView;

    public Button ConfirmBuy;
    public Button ConfirmCancel;
}

public class TaobaoItemView
{
    public RectTransform root;
    public Text ItemName;
    public Image ItemBG;
    public Image Hint;
    public Image Picture;
    public Text Price;
    public Text InStock;
    public GameObject SelloutMark;

    public void BindView(Transform transform)
    {
        root = transform as RectTransform;
        ItemBG = root.Find("ItemBG").GetComponent<Image>();
        Hint = root.Find("Hint").GetComponent<Image>();
        ItemName = root.Find("Title").GetComponent<Text>();
        Picture = root.Find("Picture").GetComponent<Image>();

        Price = root.Find("Price").GetComponent<Text>();
        InStock = root.Find("InStock").GetComponent<Text>();

        SelloutMark = root.Find("Sellout").gameObject;
    }
}

public class TaobaoUI : UIBaseCtrl<BaseModel, TaobaoView>
{

    IResLoader pResLoader;
    IRoleModule pRoleMgr;

    TaobaoItemView preItemView;
    ICardDeckModule pCardMgr;

    TaobaoModule pTaobaoMgr;

    UIMainCtrl pMainUI;

    //List<TaobaoItemInfo> fakeList = new List<TaobaoItemInfo>();

    private int totalPage = 0;
    private int nowPage = 0;

    public static int PageFixItemNum = 6;

    public static Vector3 DetailOffset = new Vector3(2f,-1f,0);

    public override void Init()
    {
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();

        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        pTaobaoMgr = GameMain.GetInstance().GetModule<TaobaoModule>();
        pMainUI = (UIMainCtrl)mUIMgr.GetCtrl("UIMain") as UIMainCtrl;
    }

    public override void BindView()
    {
        view.Close = root.Find("Close").GetComponent<Button>();

        view.ItemContainer = root.Find("ItemContainer");
        view.DetailPanel = root.Find("DetailPanel");

        view.DetailDesp = view.DetailPanel.Find("DetailDesp").GetComponent<Text>();
        view.DetailName = view.DetailPanel.Find("DetailName").GetComponent<Text>();
        view.DetailEffectDesp = view.DetailPanel.Find("DetailEffectDesp").GetComponent<Text>();

        view.NextPage = root.Find("Next").GetComponent<Button>();
        view.PrePage = root.Find("Pre").GetComponent<Button>();
        view.PageNum = root.Find("PageNum").GetComponent<Text>();

        view.ConfirmBuyView = root.Find("确认购买").GetComponent<Image>();
        view.ConfirmBuy = view.ConfirmBuyView.transform.Find("Confirm").GetComponent<Button>();
        view.ConfirmCancel = view.ConfirmBuyView.transform.Find("Cancel").GetComponent<Button>();

        //view.Buy = root.Find("Buy").GetComponent<Button>();


        foreach (TaobaoItemView vv in view.ItemList)
        {
            pResLoader.ReleaseGO("UI/Apps/TaobaoItem", vv.root.gameObject);
        }

        view.ItemList.Clear();

        for (int i=0; i< PageFixItemNum; i++)
        {
            GameObject go = pResLoader.Instantiate("UI/Apps/TaobaoItem", view.ItemContainer);
            TaobaoItemView vv = new TaobaoItemView();
            vv.BindView(go.transform);
            view.ItemList.Add(vv);
            vv.Hint.gameObject.SetActive(false);
            int idx = i;
            vv.root.gameObject.SetActive(false);
            {
                DragEventListener listener = vv.ItemBG.gameObject.GetComponent<DragEventListener>();
                if (listener == null)
                {
                    listener = vv.ItemBG.gameObject.AddComponent<DragEventListener>();
                }

                listener.ClearClickEvent();
                listener.OnClickEvent += delegate {
                    //ChooseOne(vv);
                    BuyOne(vv);
                };
                listener.PointerEnterEvent += delegate {
                    ShowPopupInfo(idx);
                    //idx
                };
                listener.PointerExitEvent += delegate {

                    HidePopupInfo(idx);
                };
            }
        }

    }

    public void ShowPopupInfo(int idx)
    {
        view.DetailPanel.gameObject.SetActive(true);
        view.DetailPanel.transform.position = view.ItemList[idx].root.transform.position + DetailOffset;
        //view.DetailName.text = fakeList[nowPage * PageFixItemNum + idx].Name;
        view.DetailName.text = pTaobaoMgr.GetDetailItem(nowPage * PageFixItemNum + idx).Name;
        view.DetailDesp.text = pTaobaoMgr.GetDetailItem(nowPage * PageFixItemNum + idx).Desp;
        view.DetailEffectDesp.text = pTaobaoMgr.GetDetailItem(nowPage * PageFixItemNum + idx).EffectDesp;
    }

    public void HidePopupInfo(int idx)
    {
        view.DetailPanel.gameObject.SetActive(false);
    }


    //public void FakeItems()
    //{
    //    fakeList = new List<TaobaoItemInfo>();
    //    for(int i = 0; i < 35; i++)
    //    {
    //        fakeList.Add(new TaobaoItemInfo("道具" + i, i * 50, string.Format("item_{0:00}", i + 1)));
    //    }
    //}

    public override void RegisterEvent()
    {
        view.Close.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.NextPage.onClick.AddListener(delegate ()
        {
            if(nowPage < totalPage - 1)
            {
                nowPage++;
                ShowItems();
                view.PageNum.text = (nowPage + 1) + "";
            }
        });
        view.PrePage.onClick.AddListener(delegate ()
        {
            if (nowPage > 0)
            {
                nowPage--;
                ShowItems();
                view.PageNum.text = (nowPage + 1) + "";
            }
        });
        //view.Buy.onClick.AddListener(delegate ()
        //{
        //    if(pRoleMgr.Money > 0)
        //    {
        //        pRoleMgr.GainMoney(-10);
        //        pCardMgr.GainNewCard("card9001");
        //        mUIMgr.ShowHint("buy card");
        //    }

        //});

        view.ConfirmBuy.onClick.AddListener(delegate ()
        {
            ConfirmBuy();
            view.ConfirmBuyView.gameObject.SetActive(false);
        });

        view.ConfirmCancel.onClick.AddListener(delegate ()
        {
            view.ConfirmBuyView.gameObject.SetActive(false);
        });
    }

    public override void PostInit()
    {
        //pTaobaoMgr.LoadProductList();
        //FakeItems();
        //if (fakeList.Count == 0)
        //{
        //    totalPage = 1;
        //}
        //else
        //{
        //    totalPage = (fakeList.Count - 1) / 6 + 1;
        //}
        if(pTaobaoMgr.CheckIsEmptyList())
        {
            totalPage = 1;
        } else
        {
            totalPage = (pTaobaoMgr.GetNumberOfProduct() - 1) / 6 + 1;
        }
        ShowItems();
    }

    public void ShowItems()
    {

        int from = nowPage * PageFixItemNum;
        int to = Mathf.Min(nowPage * PageFixItemNum + 5, pTaobaoMgr.GetNumberOfProduct() - 1);
        //int to = Mathf.Min(nowPage * PageFixItemNum + 5,fakeList.Count-1);
        int idx = 0;
        for (int i= from; i<= to; i++)
        {
            //TaobaoItemInfo info = fakeList[i];
            TaobaoItemInfo info = pTaobaoMgr.GetDetailItem(i);
            view.ItemList[idx].ItemName.text = info.Name;
            view.ItemList[idx].Price.text = info.Cost + " G";
            view.ItemList[idx].Picture.sprite = pResLoader.LoadResource<Sprite>("TaobaoItemImages/"+ info.Picture);
            Debug.Log(info.Picture);
            if(info.LeftInStock == 0)
            {
                view.ItemList[idx].SelloutMark.SetActive(true);
                view.ItemList[idx].InStock.text = "库存 "+info.LeftInStock;
            }
            else
            {
                view.ItemList[idx].SelloutMark.SetActive(false);
                view.ItemList[idx].InStock.text = "库存 "+info.LeftInStock;
            }

            view.ItemList[idx].root.gameObject.SetActive(true);
            idx++;

        }
        for(int i = idx; i < PageFixItemNum; i++)
        {
            view.ItemList[i].root.gameObject.SetActive(false);
        }
    }

    public void BuyOne(TaobaoItemView vv)
    {
        int idx = view.ItemList.IndexOf(vv);
        if (idx == -1)
        {
            return;
        }

        int idxInList = nowPage * PageFixItemNum +idx;


        //int cost = fakeList[idxInList].Cost;
        int cost = pTaobaoMgr.GetDetailItem(idxInList).Cost;
        //if(fakeList[idxInList].LeftInStock == 0)
        if (!pTaobaoMgr.CheckAvaiableLeftInStock(idxInList))
        {
            mUIMgr.ShowHint("无货");
            return;
        }
        if (pRoleMgr.Money > cost)
        {
            wantBuyIdx = idxInList;
            view.ConfirmBuyView.gameObject.SetActive(true);
            //mUIMgr.ShowConfirmBox("确认购买？", ConfirmBuy);
        }
        else
        {
            mUIMgr.ShowHint("金币不足");
        }
    }

    private int wantBuyIdx;

    public void ChooseOne(TaobaoItemView vv)
    {
        int idx = view.ItemList.IndexOf(vv);
        if (idx == -1)
        {
            return;
        }

        if (preItemView != null)
        {
            preItemView.Hint.gameObject.SetActive(false);
        }

        vv.Hint.gameObject.SetActive(true);
        preItemView = vv;
    }

    public void ConfirmBuy()
    {
        pTaobaoMgr.ConfirmBuy(wantBuyIdx);
        

        if (pTaobaoMgr.GetDetailItem(wantBuyIdx).CardRelate != "")
        {
            string cardName = pTaobaoMgr.GainCard(wantBuyIdx);
            if(cardName!="") mUIMgr.ShowHint("获得卡牌: " + cardName);
        }

        if (pTaobaoMgr.GetDetailItem(wantBuyIdx).ShuxingRelate != "")
        {
            string shuxingInfo = pTaobaoMgr.GainShuxing(wantBuyIdx);
            if (shuxingInfo !="") mUIMgr.ShowHint("获得属性: " + shuxingInfo);
        }
        ShowItems();
        pMainUI.UpdateWords();
    }
}
