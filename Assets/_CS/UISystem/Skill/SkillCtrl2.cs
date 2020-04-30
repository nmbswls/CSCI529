using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingleSkillView
{
    public string SkillId;
    public RectTransform root;
    private SkillCtrl2 pSkillCtrl;
    public Image star;

    public SingleSkillView(SkillCtrl2 pSkillCtrl)
    {
        this.pSkillCtrl = pSkillCtrl;
    }

    public void Init(Transform tf)
    {
        this.root = (RectTransform)tf;
        this.star = tf.GetChild(0).GetComponent<Image>();
        RegisterEvent();
    }

    public void RegisterEvent()
    {
        ClickEventListerner listener = root.gameObject.GetComponent<ClickEventListerner>();
        if (listener == null)
        {
            listener = root.gameObject.AddComponent<ClickEventListerner>();
        }
        listener.ClearClickEvent();
        listener.OnClickEvent += delegate
        {
            pSkillCtrl.SelectSkill(this.SkillId);
        };
    }

    public void ShowStar()
    {
        this.star.gameObject.SetActive(true);
    }

}

public class ScheduleModel2 : BaseModel
{

}

public class ScheduleView2 : BaseView
{
    public Transform Pool;
    public Transform TreePanel;

    public Transform Detail;

    public Text Title;
    public Text Level;
    public Text EffectDescription;
    public Text Description;
    public Text Learned;

    public Text SkillPoint;

    public Text RequirmentText;

    public Button Learn;

    public List<Transform> SkillPanels = new List<Transform>();

    public Button Close;

    public Transform Pool2;
    public Transform TreePanel2;

    public Button Switcher1;
    public Button Switcher2;
}

public class SkillCtrl2 : UIBaseCtrl<ScheduleModel2, ScheduleView2>
{
    IRoleModule rmgr;
    SkillTreeMgr2 pSkillMgr;
    IResLoader resLoader;

    UIMainCtrl pMainUI;

    MainGameMode mainGameMode;

    Dictionary<string, SingleSkillView> skillViewMap = new Dictionary<string, SingleSkillView>();

    string selectedSkillId = "";

    public override void Init()
    {

        mainGameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as MainGameMode;

        rmgr = GameMain.GetInstance().GetModule<RoleModule>();
        resLoader = GameMain.GetInstance().GetModule<ResLoader>();

        pSkillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr2>();

        pMainUI = (UIMainCtrl)mUIMgr.GetCtrl("UIMain") as UIMainCtrl;

        //model.Choosavles = rmgr.getAllScheduleChoises();
        
    }

    public override void BindView()
    {
        base.BindView();
        {
            if(root == null)
            {
                Debug.Log("bind fail no root found");
            }
        }

        view.Pool = root.Find("Pool");
        view.TreePanel = view.Pool.Find("TreePanel");

        

        //TODO: Children

        //foreach (Transform child in view.TreePanel.Find("Tree"))
        //{
            
        //    view.SkillPanels.Add(child);
        //}

        for(int i = 0; i < view.TreePanel.Find("Tree").childCount; i++)
        {
            for(int j = 0; j<view.TreePanel.Find("Tree").GetChild(i).childCount; j++)
            {
                SingleSkillView singleSkill = new SingleSkillView(this);
                singleSkill.Init(view.TreePanel.Find("Tree").GetChild(i).GetChild(j));
                if (!pSkillMgr.SkillBranchDict.ContainsKey(i))
                {
                    continue;
                }
                if (!pSkillMgr.SkillBranchDict[i].ContainsKey(j))
                {
                    continue;
                }
                singleSkill.SkillId = pSkillMgr.SkillBranchDict[i][j];
                skillViewMap[singleSkill.SkillId] = singleSkill;
            }
        }

        view.Detail = root.Find("DetailPanel");
        view.Title = view.Detail.Find("Title").GetComponent<Text>();
        view.Level = view.Detail.Find("Level").GetComponent<Text>();
        view.Learned = view.Detail.Find("Learned").GetComponent<Text>();
        view.EffectDescription = view.Detail.Find("EffectDescription").GetComponent<Text>();
        view.Description = view.Detail.Find("Description").GetComponent<Text>();
        view.RequirmentText = view.Detail.Find("Requirement").GetComponent<Text>();

        view.Learn = view.Detail.Find("LearnButton").GetComponent<Button>();
        view.SkillPoint = root.Find("剩余点数").GetChild(0).GetComponent<Text>();

        view.Close = root.Find("Close").GetComponent<Button>();

        //Bonus
        view.Pool2 = root.Find("Pool2");
        view.TreePanel2 = view.Pool2.Find("TreePanel");

        for (int i = 0; i < view.TreePanel2.childCount; i++)
        {
            SingleSkillView singleSkill = new SingleSkillView(this);
            singleSkill.Init(view.TreePanel2.GetChild(i));
            if (!pSkillMgr.SkillBranchDict[5].ContainsKey(i))
            {
                continue;
            }
            singleSkill.SkillId = pSkillMgr.SkillBranchDict[5][i];
            skillViewMap[singleSkill.SkillId] = singleSkill;
        }

        //Switcher
        view.Switcher1 = root.Find("Switcher1").GetComponent<Button>();
        view.Switcher2 = root.Find("Switcher2").GetComponent<Button>();

        UpdateSkillPoint();

        ShowStarsOfLearnedSkill();
    }

    public void ShowDetail(string skillId)
    {
        SkillInfo2 si = pSkillMgr.GetSkillAsset(skillId);
        
        if(si == null)
        {
            HideDetail();
            return;
        }

        view.Title.text = si.SkillName;
        view.Level.text = si.Level + "";
        if(si.isLearned)
        {
            view.Learned.text = "已学会";
            view.Learned.color = Color.green;
        } else
        {
            view.Learned.text = "未学会";
            view.Learned.color = Color.red;
        }

        view.EffectDescription.text = si.EffectDes;
        view.Description.text = si.Des;

        string reqText = "";
        for (int i = 0; i < si.Requirements.reqStats.Count; i++)
        {
            reqText += "需要";
            reqText += si.Requirements.reqStats[i].ToString();
            reqText += ": ";
            reqText += si.Requirements.reqValues[i] + "";
            reqText += "\n";
        }
        reqText += "需要技能点数:";
        reqText += si.Requirements.reqSkillPointValue.ToString();

        view.RequirmentText.text = reqText;

        view.Detail.gameObject.SetActive(true);
    }

    public void HideDetail()
    {
        view.Detail.gameObject.SetActive(false);
    }

    public override void RegisterEvent()
    {
        view.Learn.onClick.AddListener(delegate ()
        {
            //学技能
            LearnCurSkill();
        });

        view.Close.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.Switcher1.onClick.AddListener(delegate ()
        {
            view.Switcher1.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            view.Switcher2.transform.localScale = new Vector3(1f, 1f, 1f);
            view.Pool.gameObject.SetActive(true);
            view.Pool2.gameObject.SetActive(false);
        });

        view.Switcher2.onClick.AddListener(delegate ()
        {
            view.Switcher2.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            view.Switcher1.transform.localScale = new Vector3(1f, 1f, 1f);
            view.Pool2.gameObject.SetActive(true);
            view.Pool.gameObject.SetActive(false);
        });
    }

    public void LearnCurSkill()
    {
        if (selectedSkillId == "")
        {
            return;
        }
        bool success = pSkillMgr.learnSkill(selectedSkillId);
        if(success)
        {
            skillViewMap[selectedSkillId].ShowStar();
            UpdateSkillPoint();
            UpdateLearnedStatus(selectedSkillId);
            pMainUI.UpdateWords();
        }
    }

    public void SelectSkill(string vv)
    {
        ShowDetail(vv);
        selectedSkillId = vv;
    }

    public void UpdateSkillPoint()
    {
        view.SkillPoint.text = rmgr.GetSkillPoint() + "";
    }

    public void UpdateLearnedStatus(string selectedSkillId)
    {
        SkillInfo2 skill = pSkillMgr.GetSkillAsset(selectedSkillId);
        if (skill == null)
        {
            return;
        }
        if (skill.isLearned)
        {
            view.Learned.text = "已学会";
            view.Learned.color = Color.green;
        }
        else
        {
            view.Learned.text = "未学会";
            view.Learned.color = Color.red;
        }
    }

    public void ShowStarsOfLearnedSkill()
    {
        foreach(string skillId in pSkillMgr.LearnedSkill)
        {
            skillViewMap[skillId].ShowStar();
        }
    }
    
}