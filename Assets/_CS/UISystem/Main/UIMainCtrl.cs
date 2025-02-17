﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class MainModel : BaseModel
{
    public List<AppInfo> UnlockedApps = new List<AppInfo>();
    public List<TurnMsg> Msgs = new List<TurnMsg>();

}



public class MainView : BaseView
{
	public Button NextStage;
    public Button ScheduleBtn;
    public Button InspectBtn;
    public Button FightDanmuBtn;

    public Slider XinqingSlider;
    public Slider FumianSlider;

    public Image PhoneMiniIcon;
    public Image PhoneBigPic;
    public Image Close;

    public Image Mail;
    public Image Taobao;

    public Transform Properties;
    public List<PropertyMainView> PropertyViewList = new List<PropertyMainView>();


    public Text NowSkillShow;
    public Text SkillListShow;

    public Text NextItemPrice;
    public Text MoneyShow;
    public Text FansShow;
    public Text SkillExpShow;
    public Text CardPowerShow;
    public Text SkillNeddExp;

    public Transform EventsContainer;
    public List<EventView> EventViewList = new List<EventView>();

    public Transform AppsContainer;
    public List<AppView> appViews = new List<AppView>();

    public Text meili;
    public Text fanying;
    public Text tili;
    public Text jiyi;
    public Text koucai;

    //test
    public Text showAll;
    public Button NextTest;
    public Button Next30Turn;
    public Button BuyThings;
    public Button UpdateCards;

    // 3 bars
    public Text moneyValue;

    //当前回合
    public Text currentTurn;


    //蒙版管理
    public Image Mask1;
    public Image Mask2;

}

public class PropertyMainView
{
    public RectTransform root;
    public Text Value;
    public Text Add;

    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        Value = root.Find("Value").GetComponent<Text>();
        Add = root.Find("Add").GetComponent<Text>();
    }

}

public class EventView
{
    public RectTransform root;
    public Image icon;
    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        icon = root.Find("Icon").GetComponent<Image>();

        icon.gameObject.SetActive(true);
    }
}
public class AppView
{
    public RectTransform root;
    public Image icon;
    public Text title;
    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        icon = root.Find("icon").GetComponent<Image>();
        title = root.Find("title").GetComponent<Text>();
    }
}


public class UIMainCtrl : UIBaseCtrl<MainModel, MainView>
{

	IRoleModule rm;
    IResLoader pResLoader;
    ICoreManager pCoreMgr;
    IShopMgr pShopMgr;
    ISkillTreeMgr pSkillMgr;

    bool closeCtr = false;


    public override void Init(){

		rm = GameMain.GetInstance ().GetModule<RoleModule> ();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        pCoreMgr = GameMain.GetInstance().GetModule<CoreManager>();
        pShopMgr = GameMain.GetInstance().GetModule<ShopMgr>();
        pSkillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr>();

        GetApps();
        
    }

    public void GetApps()
    {
        //get from role 
        model.UnlockedApps = rm.GetApps();
    }

    public void GetUnlockedParts()
    {
        if (rm.GetCurrentTurn() > 1)
        {
            view.InspectBtn.gameObject.SetActive(true);
        }
        //if (rm.GetCurrentTurn() > 7)
        if (rm.GetCurrentTurn() > 0)
        {
            view.ScheduleBtn.gameObject.SetActive(true);
        }
        if (rm.GetCurrentTurn() == 9)
        {
            view.FightDanmuBtn.gameObject.SetActive(true);
        }
    }

    public void AddMsg(TurnMsg msg)
    {
        model.Msgs.Add(msg);
        GameObject go = pResLoader.Instantiate("UI/Main/e0", view.EventsContainer);
        EventView vv = new EventView();
        vv.BindView(go.transform);
        ClickEventListerner listerner = vv.icon.gameObject.GetComponent<ClickEventListerner>();
        if(listerner == null)
        {
            listerner = vv.icon.gameObject.AddComponent<ClickEventListerner>();
        }
        listerner.ClearClickEvent();
        listerner.OnClickEvent += delegate
        {
            mUIMgr.ShowMsgBox(msg.content);
            RemoveMsg(msg);
        };
        view.EventViewList.Add(vv);
    }


    public void RemoveMsg(TurnMsg msg)
    {
        int msgIdx = model.Msgs.IndexOf(msg);
        if(msgIdx == -1)
        {
            return;
        }
        model.Msgs.Remove(msg);
        EventView vv = view.EventViewList[msgIdx];
        view.EventViewList.RemoveAt(msgIdx);
        vv.icon.gameObject.SetActive(false);
        DOTween.To
            (
                () => vv.root.sizeDelta,
                (x) => vv.root.sizeDelta = x,
                new Vector2(0,vv.root.sizeDelta.y),
                0.3f
            ).OnComplete(delegate {
                GameObject.Destroy(vv.root.gameObject);
            }).OnUpdate(delegate {
                LayoutRebuilder.ForceRebuildLayoutImmediate(view.EventsContainer as RectTransform);
                //view.EventsContainer.

            });
    }

    public void ShowMsg(List<TurnMsg> msgs)
    {
        model.Msgs = msgs;
        foreach (TurnMsg msg in msgs)
        {
            GameObject go = pResLoader.Instantiate("UI/Main/e0", view.EventsContainer);
            EventView vv = new EventView();
            vv.BindView(go.transform);

            ClickEventListerner listerner = vv.icon.gameObject.GetComponent<ClickEventListerner>();
            if (listerner == null)
            {
                listerner = vv.icon.gameObject.AddComponent<ClickEventListerner>();
            }
            listerner.ClearClickEvent();
            listerner.OnClickEvent += delegate
            {
                mUIMgr.ShowMsgBox(msg.content);
                RemoveMsg(msg);
            };

            view.EventViewList.Add(vv);
        }
    }

    public void UpdateXintai()
    {
        view.XinqingSlider.value = rm.GetXinqingLevel() * 1.0f / 20;
    }

    public void UpdateFumian()
    {
        view.XinqingSlider.value = rm.GetBadLevel() * 1.0f / 20;
    }

    public override void BindView(){
        //view.NextStage = root.
        view.InspectBtn = root.Find("Deck").GetComponent<Button>();
        view.ScheduleBtn = root.Find("Schedule").GetComponent<Button>();
        view.NextStage = root.Find("NextTurn").GetComponent<Button>();
        view.FightDanmuBtn = root.Find("FightDanmuBtn").GetComponent<Button>();

        view.XinqingSlider = root.Find("Property2").Find("Xintai").Find("Slider").GetComponent<Slider>();
        view.FumianSlider = root.Find("Property2").Find("Fumian").Find("Slider").GetComponent<Slider>();


        view.PhoneBigPic = root.Find("PhoneMenu").GetComponent<Image>();
        view.Close = view.PhoneBigPic.transform.Find("Close").GetComponent<Image>();
        view.PhoneMiniIcon = root.Find("Phone_miniicon").GetComponent<Image>();

        view.MoneyShow = root.Find("Money").Find("Value").GetComponent<Text>();
        view.SkillExpShow = root.Find("SkillExp").Find("Value").GetComponent<Text>();
        view.CardPowerShow = root.Find("CardPower").Find("Value").GetComponent<Text>();

        view.moneyValue = root.Find("money bar").Find("Value").GetComponent<Text>();

        view.Properties = root.Find("Properties");

        view.meili = view.Properties.Find("VBox").GetChild(0).GetChild(0).GetComponent<Text>();
        view.jiyi = view.Properties.Find("VBox").GetChild(1).GetChild(0).GetComponent<Text>();
        view.tili = view.Properties.Find("VBox").GetChild(2).GetChild(0).GetComponent<Text>();
        view.koucai = view.Properties.Find("VBox").GetChild(3).GetChild(0).GetComponent<Text>();
        view.fanying = view.Properties.Find("VBox").GetChild(4).GetChild(0).GetComponent<Text>();

        view.Mask1 = root.Find("Mask#1").GetComponent<Image>();
        view.Mask2 = root.Find("Mask#2").GetComponent<Image>();


        foreach (Transform child in view.Properties.Find("VBox"))
        {
            PropertyMainView vv = new PropertyMainView();
            vv.BindView(child);
            view.PropertyViewList.Add(vv);
        }

        view.EventsContainer = root.Find("Events");

        view.AppsContainer = view.PhoneBigPic.transform.Find("Apps");

        foreach(AppInfo app in model.UnlockedApps)
        {
            GameObject go = pResLoader.Instantiate("UI/app",view.AppsContainer);
            AppView appView = new AppView();
            appView.BindView(go.transform);
            appView.icon.sprite = pResLoader.LoadResource<Sprite>("Textures/" + app.AppId);
            appView.title.text = app.ShowName;
            view.appViews.Add(appView);
        }

        view.FansShow = root.Find("Fans").Find("Value").GetComponent<Text>();
        view.NextItemPrice = root.Find("NextItem").Find("Value").GetComponent<Text>();
        view.NowSkillShow = root.Find("NowSkill").Find("Value").GetComponent<Text>();
        view.SkillListShow = root.Find("NowSkill").Find("List").GetComponent<Text>();
        //
        view.showAll = root.Find("Test").Find("Text").GetComponent<Text>();
        view.BuyThings = root.Find("Test").Find("BuyThing").GetComponent<Button>();
        view.NextTest = root.Find("Test").Find("Next").GetComponent<Button>();
        view.UpdateCards = root.Find("Test").Find("UpdateCard").GetComponent<Button>();
        view.Next30Turn = root.Find("Test").Find("Next30").GetComponent<Button>();
        view.NextTest.onClick.AddListener(delegate
        {
            TestNextTurn();
        });

        view.BuyThings.onClick.AddListener(delegate
        {
            AdjustSkillCardCtrl cc =mUIMgr.ShowPanel("AdjustSkillCardsPanel") as AdjustSkillCardCtrl;
            cc.SetContent("test_01");
            //UseResource();
        });

        view.UpdateCards.onClick.AddListener(delegate
        {
            AddSkill();
        });
        view.Next30Turn.onClick.AddListener(delegate
        {
            while(turn<30)
            {
                UseResource();
                AddSkill();
                TestNextTurn();
            }
            testUpdateWords();
            pSkillMgr.PrintSkills();
        });

        view.currentTurn = root.Find("CurrentTurn").GetChild(0).GetComponent<Text>();

        //testUpdateWords();
        UpdateWords();

        updateInitalData();
        GetUnlockedParts();
    }

    //50点资源 = 每回合属性+1
    //5点资源 = 属性+1
    //每点属性 每回合 使技能进度+1
    //技能进度+1 卡片战力+1

    //每回合获得资源100 * 回合数 (最多*1.2)
    //每回合固定获得属性

    //1级技能 0点经验 强度5伤害
    //2级技能 50点经验 强度8 伤害
    //3级技能 150点经验 强度12 伤害
    //4级技能 300点经验 强度17伤害
    //50级技能 



    int nowItemLevel = 1;

    int beginSkillLevel = 1;
    int nowSkillLevel = 1;

    int sameSkillNum = 0;


    int totalSkillLevel = 1;

    int SkillNum = 1;

    public void UseResource()
    {


        while (testResource > nowItemLevel * 50)
        {
            pShopMgr.FakeBuy(nowItemLevel);
            testResource -= nowItemLevel * 50;
            turnStatus += nowItemLevel;
            nowItemLevel++;
        }
        if (testResource > 0)
        {
            //testCardExp += testResource / 10;
            //testResource -= 0;
        }


        testUpdateWords();
    }

    public void AddSkill()
    {
        //testCardExp += 
        if (hasAddSkill) return;
        hasAddSkill = true;
        testCardExp += testStatus;

        while(testCardExp > nowSkillLevel * 50)
        {
            if (nowSkillLevel - beginSkillLevel > 5)
            {
                //if(testResource > (nowSkillLevel - 3) * 50 / 10)
                if (sameSkillNum > 3)
                {
                    sameSkillNum = 1;
                    pSkillMgr.GainSkills(string.Format("test_{0:00}", NumBaseSkill+1));
                    beginSkillLevel = beginSkillLevel + 3;
                    NumBaseSkill++;
                }
                else
                {
                    sameSkillNum++;
                }
                nowSkillLevel = beginSkillLevel;

                //sa.SkillId = string.Format("test_extend_{0:00}_{1:00}", i + 1, n + 1);
                //pSkillMgr.GainSkills(string.Format("test_extend_{0:00}", NumSkill + 1));

                //pSkillMgr.PrintSkills();

                NumSkill++;

                continue;
            }
            pSkillMgr.GainSkills(string.Format("test_extend_{0:00}_{1:00}", NumBaseSkill, nowSkillLevel - beginSkillLevel));
            testCardExp -= nowSkillLevel * 50;
            nowSkillLevel++;
            totalSkillLevel++;
        }

        testCardPower = totalSkillLevel * 5;
        testUpdateWords();
    }

    public void TestNextTurn()
    {
        turn += 1;
        testStatus += turnStatus;
        rm.GainMoney(30 + 50 * turn);
        testResource += 30 + 50 * turn;
        int adddd = (int)((30 + turn * 50 + (testStatus * 10 + testFans * 0.2f + 0) * 0.2f) * 1f);
        testFans += adddd;
        //nandu = 50 + 10 * Mathf.Pow(1.2f,turn);
        testUpdateWords();
        hasAddSkill = false;
    }

    public void testUpdateWords()
    {
        string s = ""; 
        s += "回合" + turn + "\n";
        view.showAll.text = s;

        view.SkillExpShow.text = testCardExp + "";
        view.CardPowerShow.text = testCardPower + "";

        for (int i = 0; i < 5; i++)
        {
            view.PropertyViewList[i].Value.text = (int)(testStatus * 0.2f)+"";
            view.PropertyViewList[i].Add.text = (int)(turnStatus * 0.2f) + "";
        }

        view.MoneyShow.text = (int)testResource + "";
        view.NowSkillShow.text = beginSkillLevel + "->" + nowSkillLevel;
        //view.FansShow.text = testFans + "";
        view.FansShow.text = rm.Fensi + "";
        view.NextItemPrice.text = (nowItemLevel * 50) + "";
        view.SkillListShow.text = nowSkillLevel * 50 + "";
    }

    public int turn = 1;
    public bool hasAddSkill = false;
    public float testResource = 10;
    public float testCardPower = 50;
    public float testStatus = 50;
    public float turnStatus = 0;
    public float testCardExp = 50;
    public float nandu;
    public int NumSkill = 1;
    public int NumBaseSkill = 1;
    public int testFans = 0;



    public override void Tick(float dTime)
    {
        base.Tick(dTime);
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddMsg(new TurnMsg("abc"));
        }
    }

    public override void PostInit()
    {
        view.PhoneBigPic.gameObject.SetActive(false);
        CloseMasks();
        UpdateFumian();
        UpdateXintai();
        InitEvents();
    }

    private void InitEvents()
    {
        foreach (EventView vv in view.EventViewList)
        {
            GameObject.Destroy(vv.root.gameObject);
        }
        view.EventViewList.Clear();
    }



    public override void RegisterEvent(){

        view.FightDanmuBtn.onClick.AddListener(delegate ()
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            mUIMgr.CloseCertainPanel(this);
            FightDanmuGMInitData data = new FightDanmuGMInitData();
            data.SkillList = new List<string>();
            data.SkillList.Add("0");
            data.SkillList.Add("1");
            data.SkillList.Add("2");
            data.SkillList.Add("3");
            view.FightDanmuBtn.gameObject.SetActive(false);
            cm.ChangeScene("FightDanmu", data);
        });


        view.NextStage.onClick.AddListener(delegate ()
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            mUIMgr.CloseCertainPanel(this);
            cm.ChangeScene("Zhibo", null ,null);
        });

        view.ScheduleBtn.onClick.AddListener(delegate ()
        {
            //mUIMgr.ShowPanel("SchedulePanel");
            mUIMgr.ShowPanel("SchedulePanel2");
        });
        view.InspectBtn.onClick.AddListener(delegate ()
        {
            mUIMgr.ShowPanel("CardsMgr");
        });

        {
            ClickEventListerner listener = view.PhoneMiniIcon.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.PhoneMiniIcon.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate (PointerEventData eventData) {

                    view.PhoneBigPic.gameObject.SetActive(true);
                    view.PhoneMiniIcon.gameObject.SetActive(false);
                    AddMask(view.Mask1);

                    view.PhoneBigPic.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                    Tween tween = DOTween.To
                        (
                            () => view.PhoneBigPic.transform.localScale,
                            (x) => view.PhoneBigPic.transform.localScale = x,
                            new Vector3(1f, 1f, 1f),
                            0.3f
                        );
                };
            }
        }

        {
            ClickEventListerner listener = view.Close.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.Close.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate (PointerEventData eventData) {
                    //check position of the phone
                    if(closeCtr)
                    {
                        {
                            Tween tween = DOTween.To
                                (
                                  () => view.PhoneBigPic.rectTransform.anchoredPosition,
                                  (x) => view.PhoneBigPic.rectTransform.anchoredPosition = x,
                                  new Vector2(778, -285),
                                  0.3f
                              );
                        }
                        closeCtr = false;
                        //mUIMgr.CloseCertainPanel();
                    }
                    view.PhoneBigPic.gameObject.SetActive(false);
                    view.PhoneMiniIcon.gameObject.SetActive(true);
                    CloseLastMask();
                };
            }
        }

        foreach(AppView vv in view.appViews)
        {
            ClickEventListerner listener = vv.root.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = vv.root.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate (PointerEventData eventData) {
                    OpenApp(vv);
                    AddMask(view.Mask2);
                };
            }
        }


    }

    public void OpenApp(AppView vv)
    {
        if (view.appViews.IndexOf(vv) == 3)
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            mUIMgr.CloseCertainPanel(this);
            cm.ChangeScene("Travel", null);
        }
        else if (view.appViews.IndexOf(vv) == 1)
        {
            //weibo
            //insert a card
            mUIMgr.ShowPanel("WeiboPanel");
        }
        else if (view.appViews.IndexOf(vv) == 2)
        {
            //taobao
            //insert a card with money paid
            mUIMgr.ShowPanel("TaobaoPanel");
        }
        else if (view.appViews.IndexOf(vv) == 0)
        {
            //mail
            mUIMgr.ShowPanel("MailPanel");
        }
        else
        {
            closeCtr = true;
            {
                Tween tween = DOTween.To
                    (
                      () => view.PhoneBigPic.rectTransform.anchoredPosition,
                      (x) => view.PhoneBigPic.rectTransform.anchoredPosition = x,
                      new Vector2(159, -500),
                      0.3f
                  );
            }
            {
                Tween tween = DOTween.To
                    (
                        () => view.PhoneBigPic.rectTransform.localScale,
                        (x) => view.PhoneBigPic.rectTransform.localScale = x,
                        new Vector3(1.6f, 1.6f, 1f),
                        0.3f
                 );
            }

        }
    }


    public void BindTravelCallbck()
    {

    }

    public void NextStage(){
		
	}

    public void updateInitalData()
    {
        view.moneyValue.text = rm.Money + "";
    }

    public void UpdateWords()
    {
        RoleStats rs = rm.GetStats();

        view.FansShow.text = (int)(rm.Fensi) + "";
        view.meili.text = (int)(rs.waiguan) + "";
        view.fanying.text = (int)(rs.caiyi) + "";
        view.tili.text = (int)(rs.kangya) + "";
        view.jiyi.text = (int)(rs.jishu) + "";
        view.koucai.text = (int)(rs.koucai) + "";
        view.moneyValue.text = (int)(rm.Money) + "";
        view.currentTurn.text = (int)(rm.GetCurrentTurn() + 1) + "";
    }

    public override void CloseMasks()
    {
        for(int i = masks.Count; i>0; i--)
        {
            masks.Peek().gameObject.SetActive(false);
            masks.Pop();
        }
    }

    public override void AddMask(Image img)
    {
        masks.Push(img);
        img.gameObject.SetActive(true);
    }

    public override void CloseLastMask()
    {
        if (masks.Count == 0) return;
        masks.Peek().gameObject.SetActive(false);
        masks.Pop();
    }
}

