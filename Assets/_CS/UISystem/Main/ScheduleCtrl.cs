using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScheduleModel : BaseModel
{
	public List<ScheduleInfo> Choosavles = new List<ScheduleInfo>();
    public string[] Chooseds;
    public int MaxSchedule;
    public int OverdueSchedule;
}

public class ScheduleView : BaseView
{
	public Transform ScheduledContainer;
    public List<ScheduleSlot> slots = new List<ScheduleSlot>();

	public Transform ChoicesContainer;
    public List<ScheduleItemView> ScheduleViewList = new List<ScheduleItemView>();

    public Text DetailName;
    public Text DetailDesp;
    public Button ChangeSchedule;
    public Text DespHint;

	public Button BtnClose;
}

public class ScheduleItemView
{
    public RectTransform root;
    public Image Icon;
    public Text Title;

    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        Icon = root.GetChild(0).GetComponent<Image>();
        Title = root.GetChild(0).GetComponentInChildren<Text>();
    }
}

public class ScheduleSlot
{
    public RectTransform root;
    public Image Bg;
    public Text Date;
    public Text Content;
    public Image Extend;

    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        Date = root.Find("Date").GetComponent<Text>();
        Bg = root.Find("BG").GetComponent<Image>();
        Content = root.Find("Content").GetComponent<Text>();
        Extend = root.Find("Extent").GetComponent<Image>();
    }
}


public class ScheduleCtrl : UIBaseCtrl<ScheduleModel, ScheduleView>
{
    IRoleModule rmgr;
    IResLoader resLoader;

    int selectedSlot = -1;
    int selectSchedule = -1;
    public override void Init(){
		
        rmgr = GameMain.GetInstance().GetModule<RoleModule>();
        resLoader = GameMain.GetInstance().GetModule<ResLoader>();

        model.Choosavles = rmgr.getAllScheduleChoises();

        model.MaxSchedule = rmgr.ScheduleMax;
        model.OverdueSchedule = rmgr.OverDueSchedule;
        model.Chooseds = rmgr.getScheduled();


    }

    public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}

        view.BtnClose = root.Find("Close").GetComponent<Button>();

        view.ChoicesContainer = root.Find("Pool").GetChild(0).GetChild(0).GetChild(0);
        view.ScheduledContainer = root.Find("RowScroll").GetChild(0).GetChild(0);

        Transform DetailPanel = root.Find("Pool").Find("Detail");
        view.DetailName = DetailPanel.Find("Title").GetComponent<Text>();
        view.DetailDesp = DetailPanel.Find("Content").GetComponent<Text>();
        view.DespHint = DetailPanel.Find("ChosenHint").GetComponent<Text>();
        view.ChangeSchedule = DetailPanel.Find("Change").GetComponent<Button>();


        foreach (ScheduleInfo choice in model.Choosavles)
        {
            GameObject go = resLoader.Instantiate("UI/Schedule/ScheduleItem", view.ChoicesContainer);
            ScheduleItemView vv = new ScheduleItemView();
            vv.BindView(go.transform);

            vv.Title.text = choice.Desp;
            view.ScheduleViewList.Add(vv);
        }

        for (int i = 0; i < model.MaxSchedule; i++)
        {
            GameObject go = resLoader.Instantiate("UI/Schedule/Slot", view.ScheduledContainer);
            ScheduleSlot vv = new ScheduleSlot();
            vv.BindView(go.transform);
            vv.Date.text = "Day "+(i + 1);
            vv.Extend.gameObject.SetActive(false);
            if (model.Chooseds[i] == null)
            {
                vv.Content.text = "死宅";
            }
            else
            {
                vv.Content.text = model.Chooseds[i];
            }
            vv.Bg.color = Color.white;
            view.slots.Add(vv);
        }

    }

    public override void RegisterEvent()
    {
        view.BtnClose.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        foreach(ScheduleItemView vv in view.ScheduleViewList)
        {
            ClickEventListerner listener = vv.Icon.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = vv.Icon.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate {

                    SelectSchedule(vv);
                };
            }

        }
        foreach (ScheduleSlot vv in view.slots)
        {
            ClickEventListerner listener = vv.Bg.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = vv.Bg.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate(PointerEventData data) {
                    if(data.button == PointerEventData.InputButton.Left)
                    {
                        SelectSlot(vv);
                    }
                    else if (data.button == PointerEventData.InputButton.Right)
                    {
                        UnloadSchedule(vv);
                    }

                };
            }

        }

        view.ChangeSchedule.onClick.AddListener(delegate
        {
            if(selectedSlot == -1 || selectSchedule == -1)
            {
                return;
            }
            rmgr.ChangeSchedule(selectedSlot,model.Choosavles[selectSchedule].Name);
            model.Chooseds[selectedSlot] = model.Choosavles[selectSchedule].Name;
            ScheduleSlot vv = view.slots[selectedSlot];
            if (model.Chooseds[selectedSlot] == null)
            {
                vv.Content.text = "死宅";
            }
            else
            {
                vv.Content.text = model.Chooseds[selectedSlot];
            }
            view.DespHint.gameObject.SetActive(true);
            view.ChangeSchedule.gameObject.SetActive(false);

        });
    }

    public override void PostInit()
    {
        selectedSlot = -1;
        selectSchedule = -1;
    }

    public void UnloadSchedule(ScheduleSlot vv)
    {
        vv.Content.text = "死宅";
        rmgr.ChangeSchedule(view.slots.IndexOf(vv),null);

        view.ChangeSchedule.gameObject.SetActive(false);
        view.DespHint.gameObject.SetActive(false);
        SelectSchedule(null);
    }

    public void SelectSlot(ScheduleSlot vv)
    {
        if(selectedSlot != -1)
        {
            view.slots[selectedSlot].Bg.color = Color.white;
        }
        vv.Bg.color = Color.blue;
        selectedSlot = view.slots.IndexOf(vv);
        ScheduleInfo info = rmgr.GetInfo(model.Chooseds[selectedSlot]);
        UpdateDetailPanel(info);

        if(info == null)
        {
            view.ChangeSchedule.gameObject.SetActive(false);
            view.DespHint.gameObject.SetActive(false);
            SelectSchedule(null);
        }
        else
        {
            view.ChangeSchedule.gameObject.SetActive(false);
            view.DespHint.gameObject.SetActive(true);
            SelectSchedule(null);
        }

    }

    public void SelectSchedule(ScheduleItemView vv)
    {
        if(selectSchedule != -1)
        {
            view.ScheduleViewList[selectSchedule].Icon.color = Color.white;
        }
        if(vv == null)
        {
            selectSchedule = -1;
            return;
        }
        selectSchedule = view.ScheduleViewList.IndexOf(vv);
        vv.Icon.color = Color.green;

        if(selectedSlot != -1 && model.Chooseds[selectedSlot] != null && model.Chooseds[selectedSlot]== model.Choosavles[selectSchedule].SId)
        {
            view.DespHint.gameObject.SetActive(true);
            view.ChangeSchedule.gameObject.SetActive(false);
        }
        else
        {
            view.DespHint.gameObject.SetActive(false);
            view.ChangeSchedule.gameObject.SetActive(true);
        }

        UpdateDetailPanel(model.Choosavles[selectSchedule]);

    }

    private void UpdateDetailPanel(ScheduleInfo info)
    {
        if (info == null)
        {
            view.DetailName.text = "未安排";
            view.DetailDesp.text = "摸鱼是没有未来的";
        }
        else
        {
            view.DetailName.text = info.Name;
            view.DetailDesp.text = info.Desp;
        }
    }

    private void InitChoices()
    {
        //fake get schedule list


    }

    private void InitScheduled()
    {


    }
}

