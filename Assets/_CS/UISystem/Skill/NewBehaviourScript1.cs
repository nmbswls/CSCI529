using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

    public Text RequirmentText;

    public Button Learn;
}

public class SkillCtrl2 : UIBaseCtrl<ScheduleModel2, ScheduleView2>
{
    IRoleModule rmgr;
    SkillTreeMgr2 pSkillMgr;
    IResLoader resLoader;

    MainGameMode mainGameMode;

    string selectedSkillId = "";

    public override void Init()
    {

        mainGameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as MainGameMode;

        rmgr = GameMain.GetInstance().GetModule<RoleModule>();
        resLoader = GameMain.GetInstance().GetModule<ResLoader>();

        pSkillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr2>();

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

        view.Detail = view.Pool.Find("DetailPanel");
        view.Title = view.Detail.Find("Title").GetComponent<Text>();
        view.Level = view.Detail.Find("Level").GetComponent<Text>();
        view.Learned = view.Detail.Find("Learned").GetComponent<Text>();
        view.EffectDescription = view.Detail.Find("EffectDescription").GetComponent<Text>();
        view.Description = view.Detail.Find("Description").GetComponent<Text>();
        view.RequirmentText = view.Detail.Find("Requirement").GetComponent<Text>();
    }
}