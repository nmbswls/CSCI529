using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleModel : BaseModel
{
    //public List<ScheduleInfo> Choosavles = new List<ScheduleInfo>();
    public List<string> NowSkills = new List<string>();
    public string[] Chooseds;
    public int MaxSchedule;
    public int OverdueSchedule;

    public int nowTab = -1;
}


//红白机基础 fps moba
//base 
//lv1 2张 贪吃蛇I 2张 推箱子I 2张俄罗斯方块I bonus: x0.5
//lv2 bonus x0.8 自由移除2张卡
//lv3 bonus x1.5 自由移除4张卡

//分支技能 
//推箱王 lv1 推箱子II*1
//推箱王 lv2 推箱子II*2
//推箱王 lv3 推箱子超人I

//
public class ScheduleView : BaseView
{
	public Transform ScheduledContainer;
    public List<ScheduleSlot> slots = new List<ScheduleSlot>();

    public ScrollRect ChoicesScrollRect;
	public RectTransform ChoicesContainer;
    public List<SkillItemView> SkillViewList = new List<SkillItemView>();

    public Button BackToTopLevel;
    public Text ActionCost;
    public List<Transform> SkillPanels = new List<Transform>();
    public List<SkillItem> TopSkills = new List<SkillItem>();

    public Transform Detail;
    public Text DetailName;
    public Text DetailDesp;





    public Transform LearnPanel;
    public Text MoneyCost;
    public Text Prerequist;


    public Text LearningNextLevel;


    public Transform PracticeePanel;
    public Text Level;
    public Slider ExpSlider;
    public Text ExpValue;
    public Text Difficulty;

    public Transform CurLevelComp;
    public Transform NextLevelComp;

    public Text CurLevelAward;
    public Text NextLevelAward;

    public Text CurLevelCards;
    public Text NextLevelCards;

    public Transform DetailMask;

    public Button LearnBtn;
    public Button PracticeBtn;
    public Button UpgradeBtn;






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

    MainGameMode mainGameMode;

    public bool isSubLevel = false;

    //int selectedSkill = -1;

    string selectedSkillId = "";
    public override void Init(){

        mainGameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as MainGameMode;

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


        view.BackToTopLevel = root.Find("Pool").Find("BackToTopLevel").GetComponent<Button>();
        view.ActionCost = root.Find("Pool").Find("ActionCost").GetComponentInChildren<Text>();

        view.ChoicesScrollRect = root.Find("Pool").Find("Choices").GetComponent<ScrollRect>();
        view.ChoicesContainer = root.Find("Pool").Find("Choices").GetChild(0).GetChild(0) as RectTransform;
        view.ScheduledContainer = root.Find("ClassShedule").GetChild(0).GetChild(0);
        Transform panels = view.ChoicesContainer.Find("Panels");
        foreach (Transform child in panels)
        {
            view.SkillPanels.Add(child);
        }

        view.Detail = root.Find("Pool").Find("Detail");



        view.DetailName = view.Detail.Find("Title").GetComponent<Text>();
        view.DetailDesp = view.Detail.Find("ContentBg").Find("Content").GetComponent<Text>();
        view.DetailMask = view.Detail.Find("Mask");

        view.LearnPanel = view.Detail.Find("LearnPanel");


        view.MoneyCost = view.LearnPanel.Find("Money").GetComponent<Text>();
        view.Prerequist = view.LearnPanel.Find("Prerequists").GetComponent<Text>();

        view.LearningNextLevel = view.LearnPanel.Find("LearningNextLevel").GetComponent<Text>();


        view.PracticeePanel = view.Detail.Find("PracticeePanel");

        //view.PracticeAward = view.AfterStudyPanel.Find("PracticeAward").Find("Content").GetComponent<Text>();
        view.Difficulty = view.PracticeePanel.Find("Difficulty").GetComponent<Text>();
        view.Level = view.PracticeePanel.Find("Level").GetComponent<Text>();
        view.ExpSlider = view.PracticeePanel.Find("Exp").GetComponent<Slider>();
        view.ExpValue = view.ExpSlider.transform.GetChild(1).GetComponentInChildren<Text>();

        view.PracticeBtn = view.Detail.Find("PracticeBtn").GetComponent<Button>();
        view.LearnBtn = view.Detail.Find("LearnBtn").GetComponent<Button>();


        view.CurLevelComp = view.Detail.Find("CurLevel");
        view.NextLevelComp = view.Detail.Find("NextLevel");

        view.CurLevelAward = view.Detail.Find("CurLevel").Find("LevelAward").GetComponent<Text>();
        view.NextLevelAward = view.Detail.Find("NextLevel").Find("LevelAward").GetComponent<Text>();

        view.CurLevelCards = view.Detail.Find("CurLevel").Find("Title").GetComponent<Text>();
        view.NextLevelCards = view.Detail.Find("NextLevel").Find("Title").GetComponent<Text>();

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
            HideDetail();
            return;
        }

        view.DetailName.text = sa.SkillName;
        view.DetailDesp.text = sa.SkingDesp;
        SkillInfo info = pSkillMgr.GetOwnedSkill(skillId);
        if(info == null)
        {
            //未学习技能
            view.PracticeePanel.gameObject.SetActive(false);
            view.LearnPanel.gameObject.SetActive(true);

            view.LearnBtn.gameObject.SetActive(true);
            {
                view.MoneyCost.gameObject.SetActive(true);
                view.MoneyCost.text = sa.Prices[0] + "";
                view.LearningNextLevel.text = "(0->1)";
            }
            view.PracticeBtn.gameObject.SetActive(false);

            view.CurLevelComp.gameObject.SetActive(false);
            view.NextLevelComp.gameObject.SetActive(true);


            string s = "";
            for(int i = 0; i < sa.PrerequistSkills.Count; i++)
            {
                s += sa.PrerequistSkills[i].skillId;
                s += sa.PrerequistSkills[i].level;
                s += "\n";
            }

            view.NextLevelCards.text = sa.LevelDesp[0];
            view.NextLevelAward.text = "属性+"+sa.LevelStatusAdd[0];

            view.Prerequist.text = s;

            //ExtentSkillAsset esa = sa as ExtentSkillAsset;
            //if(esa != null && esa.AttachCardInfos.Count!=0)
            //{
            //    AttachCardsInfo attach = esa.AttachCardInfos[0];
            //    view.CurLevelCards.text = "";
            //    for (int i = 0; i < attach.operators.Count; i++)
            //    {

            //    }
            //}

        }
        else
        {
            BaseSkillAsset bsa = info.sa as BaseSkillAsset;
            if (bsa == null)
            {
                //普通技能
                view.PracticeePanel.gameObject.SetActive(true);
                view.LearnPanel.gameObject.SetActive(false);
                view.Difficulty.text = "300";

                if (info.SkillLvl == info.sa.MaxLevel)
                {
                    view.PracticeBtn.gameObject.SetActive(false);
                }
                else
                {
                    view.PracticeBtn.gameObject.SetActive(true);
                }


                view.LearnBtn.gameObject.SetActive(false);

            }
            else
            {
                //base技能
                view.PracticeePanel.gameObject.SetActive(false);
                view.LearnPanel.gameObject.SetActive(true);

                if (info.SkillLvl == info.sa.MaxLevel)
                {
                    view.LearnBtn.gameObject.SetActive(false);
                    view.MoneyCost.gameObject.SetActive(false);
                    view.LearningNextLevel.text = "(已满级)";
                }
                else
                {
                    view.LearnBtn.gameObject.SetActive(true);
                    view.MoneyCost.text = sa.Prices[info.SkillLvl] + "";
                    view.MoneyCost.gameObject.SetActive(true);
                    view.LearningNextLevel.text = "(" + (info.SkillLvl) +"->"+(info.SkillLvl+1) + ")";
                }

                view.PracticeBtn.gameObject.SetActive(false);


            }



            view.CurLevelComp.gameObject.SetActive(true);

            UpdateExp(info);



            view.CurLevelCards.text = sa.LevelDesp[info.SkillLvl-1];
            view.CurLevelAward.text = "属性+" + sa.LevelStatusAdd[info.SkillLvl-1];

            if (info.SkillLvl == sa.MaxLevel)
            {
                view.NextLevelComp.gameObject.SetActive(false);
            }
            else
            {
                view.NextLevelComp.gameObject.SetActive(true);
                view.NextLevelCards.text = sa.LevelDesp[info.SkillLvl];
                view.NextLevelAward.text = "属性+" + sa.LevelStatusAdd[info.SkillLvl];
            }

            //ExtentSkillAsset esa = sa as ExtentSkillAsset;
            //if (esa != null && esa.AttachCardInfos.Count != 0)
            //{
            //    AttachCardsInfo attach = esa.AttachCardInfos[0];
            //    view.CurLevelCards.text = "";
            //    for (int i = 0; i < attach.operators.Count; i++)
            //    {

            //    }
            //}

        }

        //view.DetailMask.gameObject.SetActive(false);
        view.Detail.gameObject.SetActive(true);
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
        //view.DetailMask.gameObject.SetActive(true);
        view.Detail.gameObject.SetActive(false);
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
            skills = pSkillMgr.GetSkillByType(eSkillType.None);
        }else if (newTab == 1)
        {
            skills = pSkillMgr.GetSkillByType(eSkillType.Koucai);
        }
        else if (newTab == 2)
        {
            skills = pSkillMgr.GetSkillByType(eSkillType.Game);
        }
        else if (newTab == 3)
        {
            skills = pSkillMgr.GetSkillByType(eSkillType.None);
        }


        model.NowSkills = skills;

        for(int i = 0; i < view.SkillPanels.Count; i++)
        {
            view.SkillPanels[i].gameObject.SetActive(false);
        }

        view.SkillPanels[newTab].gameObject.SetActive(true);

        SwitchTabInit();

        //foreach (SkillItemView vv in view.SkillViewList)
        //{
        //    resLoader.ReleaseGO("UI/Schedule/ScheduleItem",vv.root.gameObject);
        //}
        //view.SkillViewList.Clear();


        //foreach (string sid in skills)
        //{
        //    SkillAsset sa = pSkillMgr.GetSkillAsset(sid);

        //    GameObject go = resLoader.Instantiate("UI/Schedule/ScheduleItem", view.ChoicesContainer);
        //    SkillItemView vv = new SkillItemView();
        //    vv.BindView(go.transform);
        //    vv.Title.text = sa.SkillName;
        //    view.SkillViewList.Add(vv);
        //    vv.Unselect();

        //    ClickEventListerner listener = vv.Icon.gameObject.GetComponent<ClickEventListerner>();
        //    if (listener == null)
        //    {
        //        listener = vv.Icon.gameObject.AddComponent<ClickEventListerner>();
        //    }
        //    listener.ClearClickEvent();
        //    listener.OnClickEvent += delegate {
        //        SelectSkill(vv);
        //    };
        //}

        HideDetail();
    }

    bool lockLearnButton = false;
    public override void RegisterEvent()
    {

        view.TypeTabGroup.InitTab(typeof(CardsTabView));
        view.TypeTabGroup.OnValueChangeEvent += SwitchChoose;
        view.TypeTabGroup.switchTab(0);

        view.BackToTopLevel.onClick.AddListener(delegate {
            if (isChanging) return;
            BackToTop();

        });

        view.BtnClose.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.PracticeBtn.onClick.AddListener(delegate ()
        {
            if (!lockLearnButton)
            {
                PracticeSkill();

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
        //selectedSkill = -1;
        selectedSkillId = "";
        HideDetail();
        UpdateActionCost();
        SwitchChoose(0);
    }

    public void UpdateActionCost()
    {
        view.ActionCost.text = "消耗" + mainGameMode.GetPracticeCost() + "点";
    }
    //public void UnloadSchedule(ScheduleSlot vv)
    //{
    //    vv.Content.text = "死宅";
    //    rmgr.ChangeSchedule(view.slots.IndexOf(vv),null);

    //    view.DespHint.gameObject.SetActive(false);
    //    SelectSchedule(null);
    //}
    public void PracticeSkill()
    {
        if (selectedSkillId == "")
        {
            return;
        }
        string skillId = selectedSkillId;
        SkillInfo skill = pSkillMgr.GetOwnedSkill(skillId);
        if(skill == null)
        {
            return;
        }
        if (skill.SkillLvl == pSkillMgr.GetSkillAsset(skillId).MaxLevel)
        {
            return;
        }
        if((skill.sa as BaseSkillAsset) != null)
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

        view.ExpSlider.DOValue(skill.NowExp * 0.01f, 0.3f).OnComplete(delegate
            {
                lockLearnButton = false;
                ShowDetail(skillId);
            }
        );

        UpdateActionCost();
    }

    public void LearnCurSkill()
    {
        //if(selectedSkill == -1 || selectedSkill >= model.NowSkills.Count)
        //{
        //    return;
        //}

        if (selectedSkillId == "")
        {
            return;
        }
        //string skillId = model.NowSkills[selectedSkill];
        string skillId = selectedSkillId;
        SkillInfo skill = pSkillMgr.GetOwnedSkill(skillId);

        //满级则返回
        if(skill != null && skill.SkillLvl == pSkillMgr.GetSkillAsset(skillId).MaxLevel)
        {
            return;
        }
        int targetLevel = 1;
        if(skill != null)
        {
            targetLevel = skill.SkillLvl+1;
        }
        SkillAsset sa = pSkillMgr.GetSkillAsset(selectedSkillId);
        if (rmgr.Money < sa.Prices[targetLevel - 1])
        {
            mUIMgr.ShowHint("没钱");
            return;
        }
        rmgr.GainMoney(sa.Prices[targetLevel - 1]);

        pSkillMgr.GainSkills(skillId);
        ShowDetail(skillId);
    }

    public void SelectSkill(string vv)
    {

        ShowDetail(vv);
        selectedSkillId = vv;
        //UpdateDetailPanel(model.Choosavles[selectSchedule]);
    }

    //public void SelectSkill(SkillItemView vv)
    //{
    //    if(selectedSkill != -1)
    //    {
    //        view.SkillViewList[selectedSkill].Unselect();
    //    }
    //    if(vv == null)
    //    {
    //        selectedSkill = -1;
    //        return;
    //    }
    //    selectedSkill = view.SkillViewList.IndexOf(vv);
    //    vv.Select();

    //    ShowDetail(model.NowSkills[selectedSkill]);

    //    //UpdateDetailPanel(model.Choosavles[selectSchedule]);
    //}

    
    public void BackToTop()
    {
        isChanging = true;
        for (int i = 0; i < view.TopSkills.Count; i++)
        {
            view.TopSkills[i].TurnOrigin();
            view.TopSkills[i].gameObject.SetActive(true);
        }
        view.BackToTopLevel.gameObject.SetActive(false);


        DOTween.To
            (
                () => view.ChoicesContainer.anchoredPosition,
                (x) => view.ChoicesContainer.anchoredPosition = x,
                Vector2.zero,
                0.3f
            ).OnComplete(delegate
            {
                UnLockScroll();
                isChanging = false;
            });

        //ChoicesScrollToOrigin();

        isSubLevel = false;
        HideDetail();
    }

    public void ChooseSubskill(string id)
    {
        SelectSkill(id); 
        //ShowDetail(id);
    }

    private bool isChanging = false;
    private Vector3 tmpPos;

    public void ChooseBaseSkill(SkillItem choose)
    {
        if (isSubLevel)
        {
            SelectSkill(choose.SkillId);
        }
        else
        {
            if (isChanging) return;
            for (int i = 0; i < view.TopSkills.Count; i++)
            {
                if (view.TopSkills[i] != choose)
                {
                    view.TopSkills[i].TurnOrigin();
                    view.TopSkills[i].gameObject.SetActive(false);
                }
                else
                {
                    view.TopSkills[i].FocusNow();
                    view.TopSkills[i].gameObject.SetActive(true);
                }
            }

            isChanging = true;

            LockScroll();
            Vector3 posScreen = RectTransformUtility.WorldToScreenPoint(mUIMgr.GetCamera(), choose.rt.position);
            Vector2 posTarget = ChoicesScrollTo(posScreen);
            DOTween.To
            (
                () => view.ChoicesContainer.anchoredPosition,
                (x) => view.ChoicesContainer.anchoredPosition = x,
                posTarget,
                0.3f
            ).OnComplete(delegate
            {
                isChanging = false;
                SelectSkill(choose.SkillId);
            });
            view.BackToTopLevel.gameObject.SetActive(true);
            isSubLevel = true;
            //ChoicesScrollTo(posScreen);
            //ShowDetail(choose.SkillId);
        }

    }

    public Vector3 ChoicesScrollTo(Vector3 target)
    {
        Vector3 posScreen = RectTransformUtility.WorldToScreenPoint(mUIMgr.GetCamera(), view.ChoicesContainer.position);
        return (posScreen - target);
    }

    public void ChoicesScrollToOrigin()
    {
        view.ChoicesContainer.anchoredPosition = Vector3.zero;
    }

    public void SwitchTabInit()
    {
        if (isChanging) return;
        BackToTop();
        view.TopSkills.Clear();
        foreach (Transform child in view.SkillPanels[model.nowTab])
        {
            SkillItem si = child.GetComponent<SkillItem > ();
            si.Init(this);
            view.TopSkills.Add(si);
        }
        //全部xianshi
        view.BackToTopLevel.gameObject.SetActive(false);
        UnLockScroll();
        isSubLevel = false;
    }

    public void LockScroll()
    {
        view.ChoicesScrollRect.horizontal = false;
        view.ChoicesScrollRect.vertical = false;
        view.ChoicesScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
    }

    public void UnLockScroll()
    {
        view.ChoicesScrollRect.horizontal = true;
        view.ChoicesScrollRect.vertical = true;
        view.ChoicesScrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

}

