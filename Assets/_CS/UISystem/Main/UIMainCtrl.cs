using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class MainModel : BaseModel
{
    public List<AppInfo> UnlockedApps = new List<AppInfo>();

}

public class MainView : BaseView
{
	public Button NextStage;
    public Button ScheduleBtn;
    public Button InspectBtn;


    public Image PhoneMiniIcon;
    public Image PhoneBigPic;
    public Image Close;

    public Transform AppsContainer;
    public List<AppView> appViews = new List<AppView>();
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
    IResLoader lr;
    ICoreManager pCoreMgr;


    public override void Init(){
		view = new MainView ();
		model = new MainModel ();

		rm = GameMain.GetInstance ().GetModule<RoleModule> ();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        lr = GameMain.GetInstance().GetModule<ResLoader>();

        pCoreMgr = GameMain.GetInstance().GetModule<CoreManager>();

        GetApps();
    }

    public void GetApps()
    {
        //get from role 
        model.UnlockedApps = rm.GetApps();
    }

    public override void BindView(){
        //view.NextStage = root.
        view.InspectBtn = root.Find("Deck").GetComponent<Button>();
        view.ScheduleBtn = root.Find("Schedule").GetComponent<Button>();
        view.NextStage = root.Find("NextTurn").GetComponent<Button>();


        view.PhoneBigPic = root.Find("PhoneMenu").GetComponent<Image>();
        view.Close = view.PhoneBigPic.transform.Find("Close").GetComponent<Image>();
        view.PhoneMiniIcon = root.Find("Phone_miniicon").GetComponent<Image>();

        view.AppsContainer = view.PhoneBigPic.transform.Find("Apps");

        foreach(AppInfo app in model.UnlockedApps)
        {
            GameObject go = lr.Instantiate("UI/app",view.AppsContainer);
            AppView appView = new AppView();
            appView.BindView(go.transform);
            appView.icon.sprite = lr.LoadResource<Sprite>("Textures/" + app.AppId);
            appView.title.text = app.ShowName;
            view.appViews.Add(appView);
        }
    }

    public override void PostInit()
    {
        view.PhoneBigPic.gameObject.SetActive(false);
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
    }


    public void BindTravelCallbck()
    {

    }

    public void NextStage(){
		
	}


}

