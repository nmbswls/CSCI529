using UnityEngine;
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


    public Image PhoneMiniIcon;
    public Image PhoneBigPic;
    public Image Close;

    public Image weibo;
    public Image mail;
    public Image taobao;

    public Transform Properties;

    public Transform EventsContainer;
    public List<EventView> EventViewList = new List<EventView>();

    public Transform AppsContainer;
    public List<AppView> appViews = new List<AppView>();
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

    bool closeCtr = false;


    public override void Init(){

		rm = GameMain.GetInstance ().GetModule<RoleModule> ();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        pCoreMgr = GameMain.GetInstance().GetModule<CoreManager>();

        GetApps();

    }

    public void GetApps()
    {
        //get from role 
        model.UnlockedApps = rm.GetApps();
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


    public override void BindView(){
        //view.NextStage = root.
        view.InspectBtn = root.Find("Deck").GetComponent<Button>();
        view.ScheduleBtn = root.Find("Schedule").GetComponent<Button>();
        view.NextStage = root.Find("NextTurn").GetComponent<Button>();


        view.PhoneBigPic = root.Find("PhoneMenu").GetComponent<Image>();
        view.Close = view.PhoneBigPic.transform.Find("Close").GetComponent<Image>();
        view.PhoneMiniIcon = root.Find("Phone_miniicon").GetComponent<Image>();

        view.Properties = root.Find("Properties");

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
    }

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
        view.NextStage.onClick.AddListener(delegate ()
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            mUIMgr.CloseCertainPanel(this);
            cm.ChangeScene("Zhibo", null,delegate {
                MainGameMode gm = pCoreMgr.GetGameMode() as MainGameMode;
                if (gm == null)
                {
                    Debug.LogError("load gm error");
                }
                gm.NextTurn();
            });
        });

        view.ScheduleBtn.onClick.AddListener(delegate ()
        {
            mUIMgr.ShowPanel("SchedulePanel");
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
                    }
                    view.PhoneBigPic.gameObject.SetActive(false);
                    view.PhoneMiniIcon.gameObject.SetActive(true);
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
                };
            }
        }


    }

    public void OpenApp(AppView vv)
    {
        if (view.appViews.IndexOf(vv) == 2)
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            mUIMgr.CloseCertainPanel(this);
            cm.ChangeScene("Travel", null);
        }
        else
        {
            closeCtr = true;
            {
                Tween tween = DOTween.To
                    (
                      () => view.PhoneBigPic.rectTransform.anchoredPosition,
                      (x) => view.PhoneBigPic.rectTransform.anchoredPosition = x,
                      new Vector2(159, -540),
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
            if (view.appViews.IndexOf(vv) == 4)
            {
                //weibo
                //insert a card

            }
            else if (view.appViews.IndexOf(vv) == 3)
            {
                //taobao
                //insert a card with money paid
            }
            else if (view.appViews.IndexOf(vv) == 1)
            {
                //mail
                
            }
            else if (view.appViews.IndexOf(vv) == 0)
            {
                //wechat
            }
        }
    }


    public void BindTravelCallbck()
    {

    }

    public void NextStage(){
		
	}


}

