using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaobaoItemInfo
{
    public string Name;
    public int Cost;
    public string CardRelate;
    public int LeftInStock = 1;

    public TaobaoItemInfo(string Name, int Cost, string CardRelate)
    {
        this.Name = Name;
        this.CardRelate = CardRelate;
    }
}


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

    List<TaobaoItemInfo> fakeList = new List<TaobaoItemInfo>();
    private int totalPage = 0;
    private int nowPage = 0;

    public static int PageFixItemNum = 6;

    public static Vector3 DetailOffset = new Vector3(2f,-1f,0);

    public override void Init()
    {
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();

        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();




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
        view.DetailName.text = fakeList[nowPage * PageFixItemNum + idx].Name;
    }

    public void HidePopupInfo(int idx)
    {
        view.DetailPanel.gameObject.SetActive(false);
    }



    public void FakeItems()
    {
        fakeList = new List<TaobaoItemInfo>();
        for(int i = 0; i < 35; i++)
        {
            fakeList.Add(new TaobaoItemInfo("道具" + i, i * 50, string.Format("item_{0:00}", i + 1)));
        }
    }

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
    }

    public override void PostInit()
    {
        FakeItems();
        if(fakeList.Count == 0)
        {
            totalPage = 1;
        }
        else
        {
            totalPage = (fakeList.Count - 1) / 6 + 1;
        }
        ShowItems();
    }

    public void ShowItems()
    {

        int from = nowPage * PageFixItemNum;
        int to = Mathf.Min(nowPage * PageFixItemNum + 5,fakeList.Count-1);
        int idx = 0;
        for (int i= from; i<= to; i++)
        {
            TaobaoItemInfo info = fakeList[i];
            view.ItemList[idx].ItemName.text = info.Name;
            view.ItemList[idx].Price.text = info.Cost + "g";
            if(info.LeftInStock == 0)
            {
                view.ItemList[idx].SelloutMark.SetActive(true);
                view.ItemList[idx].InStock.text = "库存"+info.LeftInStock;
            }
            else
            {
                view.ItemList[idx].SelloutMark.SetActive(false);
                view.ItemList[idx].InStock.text = "库存"+info.LeftInStock;
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


        int cost = fakeList[idxInList].Cost;
        if(fakeList[idxInList].LeftInStock == 0)
        {
            mUIMgr.ShowHint("无货");
            return;
        }
        if (pRoleMgr.Money > cost)
        {
            wantBuyIdx = idxInList;
            mUIMgr.ShowConfirmBox("确认购买？", ConfirmBuy);
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
        if(wantBuyIdx<0|| wantBuyIdx >= fakeList.Count)
        {
            return;
        }
        int cost = fakeList[wantBuyIdx].Cost;
        pRoleMgr.GainMoney(-cost);
        if(fakeList[wantBuyIdx].LeftInStock > 0)
        {
            fakeList[wantBuyIdx].LeftInStock -= 1;
            ShowItems();
        }

        pCardMgr.GainNewCard(fakeList[wantBuyIdx].CardRelate);
        mUIMgr.ShowHint("buy card");
    }
}
