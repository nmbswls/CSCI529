using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaobaoItemInfo
{
    public string Name;

    public TaobaoItemInfo(string Name)
    {
        this.Name = Name;
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

    public Button Buy;
}

public class TaobaoItemView
{
    public RectTransform root;
    public Text ItemName;
    public Image ItemBG;
    public Image Hint;

    public void BindView(Transform transform)
    {
        root = transform as RectTransform;
        ItemBG = root.Find("ItemBG").GetComponent<Image>();
        Hint = root.Find("Hint").GetComponent<Image>();
        ItemName = root.Find("Text").GetComponent<Text>();
    }
}

public class TaobaoUI : UIBaseCtrl<BaseModel, TaobaoView>
{

    IResLoader pResLoader;
    IRoleModule pRoleMgr;

    TaobaoItemView preItemView;
    ICardDeckModule pCardMgr;

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

        view.Buy = root.Find("Buy").GetComponent<Button>();

    }

    public override void RegisterEvent()
    {
        view.Close.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.NextPage.onClick.AddListener(delegate ()
        {
            ShowItems();
        });
        view.PrePage.onClick.AddListener(delegate ()
        {
            ShowItems();
        });
        view.Buy.onClick.AddListener(delegate ()
        {
            if(pRoleMgr.Money > 0)
            {
                pRoleMgr.GainMoney(-10);
                pCardMgr.GainNewCard("card9001");
                mUIMgr.ShowHint("buy card");
            }

        });
    }

    public override void PostInit()
    {
        ShowItems();
    }

    public void ShowItems()
    {

        foreach (TaobaoItemView vv in view.ItemList)
        {
            pResLoader.ReleaseGO("UI/Apps/TaobaoItem", vv.root.gameObject);
        }
        view.ItemList.Clear();
        preItemView = null;

        List<TaobaoItemInfo> fakeList = new List<TaobaoItemInfo>();
        {
            fakeList.Add(new TaobaoItemInfo("1"));
        }

        {
            fakeList.Add(new TaobaoItemInfo("2"));
        }
        {
            fakeList.Add(new TaobaoItemInfo("3"));
        }
        {
            fakeList.Add(new TaobaoItemInfo("4"));
        }
        {
            fakeList.Add(new TaobaoItemInfo("s"));
        }
        {
            fakeList.Add(new TaobaoItemInfo("4ss"));
        }
        for(int i=0;i< fakeList.Count; i++)
        {
            TaobaoItemInfo info = fakeList[i];
            GameObject go = pResLoader.Instantiate("UI/Apps/TaobaoItem", view.ItemContainer);
            TaobaoItemView vv = new TaobaoItemView();
            vv.BindView(go.transform);
            view.ItemList.Add(vv);
            vv.Hint.gameObject.SetActive(false);
            {
                ClickEventListerner listener = vv.ItemBG.gameObject.GetComponent<ClickEventListerner>();
                if (listener == null)
                {
                    listener = vv.ItemBG.gameObject.AddComponent<ClickEventListerner>();
                }

                listener.ClearClickEvent();
                listener.OnClickEvent += delegate {
                    ChooseOne(vv);
                };


            }

            vv.ItemName.text = info.Name;
        }

    }

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
        view.DetailPanel.gameObject.SetActive(true);
        vv.Hint.gameObject.SetActive(true);
        preItemView = vv;
    }
}
