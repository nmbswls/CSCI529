using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ScheduleModel : BaseModel
{
    //public List<ScheduleInfo> Choosavles = new List<ScheduleInfo>();
    public List<string> NowSkills = new List<string>();
    public string[] Chooseds;
    public int MaxSchedule;
    public int OverdueSchedule;

    public int nowTab = -1;
}

public class ScheduleView : BaseView
{
	public Transform ScheduledContainer;
    public List<ScheduleSlot> slots = new List<ScheduleSlot>();

	public Transform ChoicesContainer;
    public List<SkillItemView> SkillViewList = new List<SkillItemView>();


    public Transform Detail;
    public Text DetailName;
    public Text DetailDesp;





    public Transform BeforeStudyPanel;
    public Text MoneyCost;
    public Text Prerequist;

    public Transform AfterStudyPanel;
    public Text Level;
    public Slider ExpSlider;
    public Text ExpValue;
    public Text Difficulty;
    public Text PracticeAward;

    public Text CurLevelCards;
    public Text NextLevelCards;

    public Transform DetailMask;

    public Button LearnBtn;
    public Button PracticeBtn;






    public TabGroup TypeTabGroup;
	public Button BtnClose;
}

public class SkillItemView
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

    public void Select()
    {
        Icon.color = Color.green;
    }

    public void Unselect()
    {
        Icon.color = Color.white;
    }
}

public class ScheduleSlot
{
    public RectTransform root;
    public Image Bg;
    public Text Content;

    public void BindView(Transform root)
    {
        this.root = (RectTransform)root;
        Bg = root.Find("BG").GetComponent<Image>();
        Content = root.Find("Content").GetComponent<Text>();
    }
}

public class SkillTabView : TabGroupChildView
{
    public Text Title;
    public Image BG;
    public override void BindView()
    {
        base.BindView();
        Title = root.GetChild(0).GetComponent<Text>();
        BG = root.GetComponent<Image>();
    }
}


public class SKillCtrl : UIBaseCtrl<ScheduleModel, ScheduleView>
{
    IRoleModule rmgr;
    ISkillTreeMgr pSkillMgr;
    IResLoader resLoader;

    TabGroup tabGroup;

    int selectedSlot = -1;
    int selectedSkill = -1;
    public override void Init(){
		
        rmgr = GameMain.GetInstance().GetModule<RoleModule>();
        resLoader = GameMain.GetInstance().GetModule<ResLoader>();

        pSkillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr>();

        //model.Choosavles = rmgr.getAllScheduleChoises();

        model.MaxSchedule = rmgr.ScheduleMax;
        model.OverdueSchedule = rmgr.OverDueSchedule;
        model.Chooseds = rmgr.getScheduled();

        model.nowTab = -1;

    }

    public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}



        view.TypeTabGroup = root.Find("Pool").Find("Header").GetChild(0).GetChild(0).GetComponent<TabGroup>();


        view.BtnClose = root.Find("Close").GetComponent<Button>();



        view.ChoicesContainer = root.Find("Pool").Find("Choices").GetChild(0).GetChild(0);
        view.ScheduledContainer = root.Find("ClassShedule").GetChild(0).GetChild(0);

        view.Detail = root.Find("Pool").Find("Detail");



        view.DetailName = view.Detail.Find("Title").GetComponent<Text>();
        view.DetailDesp = view.Detail.Find("ContentBg").Find("Content").GetComponent<Text>();
        view.DetailMask = view.Detail.Find("Mask");

        view.BeforeStudyPanel = view.Detail.Find("BeforeStudy");

        view.LearnBtn = view.BeforeStudyPanel.Find("LearnBtn").GetComponent<Button>();
        view.MoneyCost = view.BeforeStudyPanel.Find("Money").GetComponent<Text>();
        view.Prerequist = view.BeforeStudyPanel.Find("Prerequists").GetComponent<Text>();


        view.AfterStudyPanel = view.Detail.Find("AfterStudy");

        view.PracticeAward = view.AfterStudyPanel.Find("PracticeAward").Find("Content").GetComponent<Text>();
        view.Difficulty = view.AfterStudyPanel.Find("Difficulty").GetComponent<Text>();
        view.Level = view.AfterStudyPanel.Find("Level").GetComponent<Text>();
        view.ExpSlider = view.AfterStudyPanel.Find("Exp").GetComponent<Slider>();
        view.ExpValue = view.ExpSlider.transform.GetChild(1).GetComponentInChildren<Text>();
        view.PracticeBtn = view.AfterStudyPanel.Find("PracticeBtn").GetComponent<Button>();

        view.CurLevelCards = view.Detail.Find("当前等级卡牌").GetComponentInChildren<Text>();
        view.NextLevelCards = view.Detail.Find("下一等级卡牌").GetComponentInChildren<Text>();

        //for (int i = 0; i < model.MaxSchedule; i++)
        //{
        //    GameObject go = resLoader.Instantiate("UI/Schedule/Slot", view.ScheduledContainer);
        //    ScheduleSlot vv = new ScheduleSlot();
        //    vv.BindView(go.transform);
        //    if (model.Chooseds[i] == null)
        //    {
        //        vv.Content.text = "空闲";
        //    }
        //    else
        //    {
        //        vv.Content.text = model.Chooseds[i];
        //    }
        //    vv.Bg.color = Color.white;
        //    view.slots.Add(vv);
        //}

    }


    public void ShowDetail(string skillId)
    {

        SkillAsset sa = pSkillMgr.GetSkillAsset(skillId);

        if (sa == null)
        {
            view.DetailMask.gameObject.SetActive(true);
            return;
        }

        view.DetailName.text = sa.SkillName;
        view.DetailDesp.text = sa.SkingDesp;
        SkillInfo info = pSkillMgr.GetOwnedSkill(skillId);
        if(info == null)
        {
            //未学习技能
            view.AfterStudyPanel.gameObject.SetActive(false);
            view.BeforeStudyPanel.gameObject.SetActive(true);
            view.MoneyCost.text = "1000";
            string s = "";
            for(int i = 0; i < sa.PrerequistSkills.Count; i++)
            {
                s += sa.PrerequistSkills[i].skillId;
                s += sa.PrerequistSkills[i].level;
                s += "\n";
            }

            view.Prerequist.text = s;

            string cards = sa.AttachCards[0];
            string[] cardsArray = cards.Split(',');
            view.CurLevelCards.text = "";
            for (int i = 0; i < cardsArray.Length; i++)
            {
                view.NextLevelCards.text += cardsArray[i] + "\n";
            }
        }
        else
        {
            view.AfterStudyPanel.gameObject.SetActive(true);
            view.BeforeStudyPanel.gameObject.SetActive(false);
            view.Difficulty.text = "300";


            UpdateExp(info);


            string cards = sa.AttachCards[info.SkillLvl-1];
            string[] cardsArray = cards.Split(',');
            view.CurLevelCards.text = "";
            for (int i = 0; i < cardsArray.Length; i++)
            {
                view.CurLevelCards.text += cardsArray[i] + "\n";
            }

        }

        view.DetailMask.gameObject.SetActive(false);
    }

    private void UpdateExp(SkillInfo info)
    {
        if (info.SkillLvl == pSkillMgr.GetSkillAsset(info.SkillId).MaxLevel)
        {
            view.ExpSlider.value = 1;
            view.ExpValue.text = "max";
        }
        else
        {
            view.ExpSlider.value = info.NowExp * 0.01f;
            view.ExpValue.text = info.NowExp + "/100";
        }
        view.Level.text = info.SkillLvl + "";
    }

    public void HideDetail()
    {
        view.DetailMask.gameObject.SetActive(true);
    }

    public void SwitchChoose(int newTab)
    {
        if (newTab == -1 || model.nowTab == newTab)
        {
            return;
        }


        model.nowTab = newTab;


        for (int i = 0; i < view.TypeTabGroup.tabs.Count; i++)
        {
            CardsTabView childView = view.TypeTabGroup.tabs[i] as CardsTabView;
            childView.BG.color = Color.white;
        }
        {
            CardsTabView childView = view.TypeTabGroup.tabs[newTab] as CardsTabView;
            childView.BG.color = Color.red;
        }

        List<string> skills = new List<string>();
        if(newTab == 0)
        {
            skills = pSkillMgr.GetSkillByType("common");
        }else if (newTab == 1)
        {
            skills = pSkillMgr.GetSkillByType("quality");
        }
        else if (newTab == 2)
        {
            skills = pSkillMgr.GetSkillByType("caiyi");
        }
        else if (newTab == 3)
        {
            skills = pSkillMgr.GetSkillByType("game");
        }
        model.NowSkills = skills;
        foreach (SkillItemView vv in view.SkillViewList)
        {
            resLoader.ReleaseGO("UI/Schedule/ScheduleItem",vv.root.gameObject);
        }
        view.SkillViewList.Clear();


        foreach (string sid in skills)
        {
            SkillAsset sa = pSkillMgr.GetSkillAsset(sid);

            GameObject go = resLoader.Instantiate("UI/Schedule/ScheduleItem", view.ChoicesContainer);
            SkillItemView vv = new SkillItemView();
            vv.BindView(go.transform);
            vv.Title.text = sa.SkillName;
            view.SkillViewList.Add(vv);
            vv.Unselect();

            ClickEventListerner listener = vv.Icon.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = vv.Icon.gameObject.AddComponent<ClickEventListerner>();
            }
            listener.ClearClickEvent();
            listener.OnClickEvent += delegate {
                SelectSkill(vv);
            };
        }

        HideDetail();
    }

    bool lockLearnButton = false;
    public override void RegisterEvent()
    {

        view.TypeTabGroup.InitTab(typeof(CardsTabView));
        view.TypeTabGroup.OnValueChangeEvent += SwitchChoose;
        view.TypeTabGroup.switchTab(0);

        view.BtnClose.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.PracticeBtn.onClick.AddListener(delegate ()
        {
            if (!lockLearnButton)
            {
                LearnCurSkill();

            }
        });

        view.LearnBtn.onClick.AddListener(delegate ()
        {
            if (!lockLearnButton)
            {
                LearnCurSkill();

            }
        });


    }

    public override void PostInit()
    {
        selectedSlot = -1;
        selectedSkill = -1;
        HideDetail();
        SwitchChoose(0);
    }

    //public void UnloadSchedule(ScheduleSlot vv)
    //{
    //    vv.Content.text = "死宅";
    //    rmgr.ChangeSchedule(view.slots.IndexOf(vv),null);

    //    view.DespHint.gameObject.SetActive(false);
    //    SelectSchedule(null);
    //}

    public void LearnCurSkill()
    {
        if(selectedSkill == -1 || selectedSkill >= model.NowSkills.Count)
        {
            return;
        }
        string skillId = model.NowSkills[selectedSkill];
        SkillInfo skill = pSkillMgr.GetOwnedSkill(skillId);

        if(skill == null)
        {
            Debug.Log("learn");
            pSkillMgr.GainSkills(model.NowSkills[selectedSkill]);
            view.BeforeStudyPanel.gameObject.SetActive(false);
            view.AfterStudyPanel.gameObject.SetActive(true);

            UpdateExp(pSkillMgr.GetOwnedSkill(skillId));

        }
        else
        {

            if (skill.SkillLvl == pSkillMgr.GetSkillAsset(skillId).MaxLevel)
            {
                return;
            }
            if (!rmgr.CanPractice())
            {
                return;
            }
            rmgr.Practive();
            pSkillMgr.GainExp(skillId);
            lockLearnButton = true;

            view.ExpSlider.DOValue(skill.NowExp*0.01f,0.3f).OnComplete(delegate
                {
                    lockLearnButton = false;
                    UpdateExp(skill);
                }
            );

        }

    }


    public void SelectSkill(SkillItemView vv)
    {
        if(selectedSkill != -1)
        {
            view.SkillViewList[selectedSkill].Unselect();
        }
        if(vv == null)
        {
            selectedSkill = -1;
            return;
        }
        selectedSkill = view.SkillViewList.IndexOf(vv);
        vv.Select();

        ShowDetail(model.NowSkills[selectedSkill]);

        //UpdateDetailPanel(model.Choosavles[selectSchedule]);
    }

    //private void UpdateDetailPanel(string skillId)
    //{

    //    SkillAsset sa = pSkillMgr.GetSkillAsset(skillId);

    //    if (sa == null)
    //    {
    //        view.DetailName.text = "未安排";
    //        view.DetailDesp.text = "摸鱼是没有未来的";
    //    }
    //    else
    //    {
    //        view.DetailName.text = sa.SkillId;
    //        view.DetailDesp.text = sa.SkingDesp;
    //    }
    //}

    private void InitChoices()
    {
        //fake get schedule list


    }

    private void InitScheduled()
    {

    }



}

