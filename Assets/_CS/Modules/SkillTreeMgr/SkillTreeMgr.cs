using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class SkillInfo
{
    public string SkillId;
    public float NowExp;
    public int SkillLvl;
    public SkillAsset sa;
    public bool isOn = true;

    public SkillInfo()
    {

    }

    public SkillInfo(string skillId, SkillAsset sa)
    {
        this.SkillId = skillId;
        this.sa = sa;
    }
}



public class SkillTreeMgr : ModuleBase, ISkillTreeMgr
{

    IResLoader mResLoader;
    ICardDeckModule mCardMgr;
    IRoleModule mRoleMdl;

    public List<SkillInfo> OwnedSkills = new List<SkillInfo>();

    private Dictionary<string, int> TrackExps = new Dictionary<string, int>();

    private readonly static Dictionary<string, SkillAsset> SkillAssetDict = new Dictionary<string, SkillAsset>();

    public override void Setup()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        mRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();

        LoadAllSkills();
        GenFakeExtendSKills();
        GenFakeBaseSkills();

        GainSkills("test_01");
    }

    private void GenFakeExtendSKills()
    {
        for (int i = 0; i < 30; i++)
        {



            for(int n = 0; n < 5; n++)
            {
                ExtentSkillAsset sa = new ExtentSkillAsset();

                sa.SkillId = string.Format("test_extend_{0:00}_{1:00}", i + 1,n+1);
                sa.BaseSkillId = string.Format("test_{0:00}", i + 1);

                sa.SkillName = string.Format("扩展技能{0:00}-{1:00}", i + 1, n + 1);
                sa.SkingDesp = string.Format("这是测试用的扩展技能{0:00}-{1:00}", i + 1, n+1);

                sa.Prices.Add(0);

                sa.MaxLevel = 5;
                for (int j = 0; j < 5; j++)
                {
                    if (n < 2)
                    {
                        sa.LevelDesp.Add(string.Format("将一张基础攻击卡升级为test_{0:00}", i + 1 + j + 1));
                    }else if(n < 4)
                    {
                        sa.LevelDesp.Add(string.Format("将一张基础防御卡升级为test_armor_{0:00}", i + 1 + j + 1));
                    }
                    else
                    {
                        sa.LevelDesp.Add(string.Format("将一张基础加血卡升级为test_xue_{0:00}", i + 1 + j + 1));
                    }
                    sa.Difficulties.Add(i * 3 + j);

                    sa.LevelStatusAdd.Add(i + j);
                }
                for (int j = 0; j < 5; j++)
                {
                    AttachCardsInfo attached = new AttachCardsInfo();
                    CardOperator opt1 = new CardOperator();

                    BaseSkillAsset bsa = GetSkillAsset(sa.BaseSkillId) as BaseSkillAsset;

                    opt1.opt = eCardOperatorMode.Replace;


                    if (n < 2)
                    {
                        opt1.from = string.Format("test_{0:00}", i + 1);
                        opt1.to = string.Format("test_{0:00}", i + 1 + j + 1);
                    }
                    else if (n < 4)
                    {
                        opt1.from = string.Format("test_armor_{0:00}", i + 1);
                        opt1.to = string.Format("test_armor_{0:00}", i + 1 + j + 1);
                    }
                    else
                    {
                        opt1.from = string.Format("test_xue_{0:00}", i + 1);
                        opt1.to = string.Format("test_xue_{0:00}", i + 1 + j + 1);
                    }

                    //opt1.from = string.Format("test_{0:00}", i + 1);
                    //opt1.to = string.Format("test_{0:00}", i + 1 + j + 1);

                    attached.operators.Add(opt1);

                    sa.AttachCardInfos.Add(attached);

                }

                SkillAssetDict.Add(sa.SkillId, sa);
            }



        }
    }

    private void GenFakeBaseSkills()
    {
        for (int i = 0; i < 30; i++)
        {
            BaseSkillAsset sa = new BaseSkillAsset();
            sa.SkillId = string.Format("test_{0:00}", i + 1);
            sa.SkillName = string.Format("基础技能{0:00}", i + 1);
            sa.SkingDesp = string.Format("这是测试用的基础技能{0:00}", i + 1);
            sa.MaxLevel = 2;
            {
                sa.LevelDesp.Add("倍率0.8");
                sa.LevelStatusAdd.Add(5);
                sa.Prices.Add(100);
                sa.StatusBonus.Add(new float[] { 0, 0, 1, 1, 1 });
            }
            {
                sa.LevelDesp.Add("倍率1.2");
                sa.LevelStatusAdd.Add(10);
                sa.Prices.Add(300);
                sa.StatusBonus.Add(new float[] { 0, 0, 1, 1, 1 });
            }




            {
                sa.BaseCardList.Add(string.Format("test_{0:00}", i + 1));
            }
            {
                sa.BaseCardList.Add(string.Format("test_{0:00}", i + 1));
            }

            {

                sa.BaseCardList.Add(string.Format("test_xue_{0:00}", i + 1));
            }
            {
                sa.BaseCardList.Add(string.Format("test_armor_{0:00}", i + 1));
            }
            {
                sa.BaseCardList.Add(string.Format("test_armor_{0:00}", i + 1));
            }
            SkillAssetDict.Add(sa.SkillId, sa);
        }
    }

    public void GainExp(string sid)
    {
        SkillInfo skillInfo = GetOwnedSkill(sid);
        skillInfo.NowExp += CalculateExp(skillInfo);
        if(skillInfo.SkillLvl == GetSkillAsset(sid).MaxLevel)
        {
            return;
        }
        if (skillInfo.NowExp > 100)
        {
            GainSkills(sid);
            if(skillInfo.SkillLvl == GetSkillAsset(sid).MaxLevel)
            {
                skillInfo.NowExp = 0;
            }
            else
            {
                skillInfo.NowExp -= 100;
            }
        }
    }


    private int CalculateExp(SkillInfo skillInfo)
    {
        SkillAsset sa = GetSkillAsset(skillInfo.SkillId);
        if(sa == null)
        {
            return 0;
        }
        //难度 属性值 track经验值 计算出来 有min 和 max
        //int difficult=sa.Difficulties[0];
        //mRoleMdl.AddExpBonux
        return 30;
    }
    public void GainSkills(string skillId)
    {
        if (GetSkillAsset(skillId) == null)
        {
            return;
        }

        SkillAsset sa = GetSkillAsset(skillId);
        foreach (SkillPrerequist s in sa.PrerequistSkills)
        {
            SkillInfo preSkill = GetOwnedSkill(s.skillId);
            if(preSkill == null || preSkill.SkillLvl < s.level)
            {
                return;
            }
        }

        SkillInfo skillInfo = GetOwnedSkill(skillId);

        if (skillInfo == null)
        {
            skillInfo = new SkillInfo(skillId,sa);
            OwnedSkills.Add(skillInfo);
            skillInfo.SkillLvl = 1;

            BaseSkillAsset bsa = sa as BaseSkillAsset;
            if (bsa != null)
            {
                mCardMgr.AddSkillCards(skillId,new List<string>(bsa.BaseCardList));
            }


        }
        else
        {
            skillInfo.SkillLvl += 1;
        }


        mRoleMdl.AddAllStatus(sa.LevelStatusAdd[skillInfo.SkillLvl - 1]);
        Debug.Log("加了" + sa.LevelStatusAdd[skillInfo.SkillLvl - 1]);

        ExtentSkillAsset esa = sa as ExtentSkillAsset;
        if (esa == null)
        {
            return;
        }

        if (skillInfo.SkillLvl > 1)
        {
            AttachCardsInfo attachInfo = esa.AttachCardInfos[skillInfo.SkillLvl - 2];
            for (int i = 0; i < attachInfo.operators.Count; i++)
            {
                mCardMgr.ChangeSkillCard(esa.BaseSkillId, attachInfo.operators[i].to,attachInfo.operators[i].from);
            }
        }
        {
            AttachCardsInfo attachInfo = esa.AttachCardInfos[skillInfo.SkillLvl - 1];

            for (int i = 0; i < attachInfo.operators.Count; i++)
            {
                mCardMgr.ChangeSkillCard(esa.BaseSkillId, attachInfo.operators[i].from, attachInfo.operators[i].to);
            }
        }
    }


    public SkillInfo GetOwnedSkill(string SkillId)
    {
        for(int i = 0; i < OwnedSkills.Count; i++)
        {
            if(OwnedSkills[i].SkillId == SkillId)
            {
                return OwnedSkills[i];
            }
        }
        return null;
    }


    public SkillAsset GetSkillAsset(string skillId)
    {
        SkillAsset asset = null;
        if (!SkillAssetDict.TryGetValue(skillId,out asset))
        {
            asset = LoadSkillAsset(skillId);
        }
        return asset;
    }

    public SkillAsset LoadSkillAsset(string SkillId)
    {
        SkillAsset asset = mResLoader.LoadResource<SkillAsset>("Skills/SkillId");
        if(asset != null)
        {
            SkillAssetDict.Add(SkillId,asset);
        }
        return asset;
    }

    public void LoadAllSkills()
    {
        SkillAsset[] assets = mResLoader.LoadAllResouces<SkillAsset>("Skills/");
        foreach(SkillAsset sa in assets)
        {
            SkillAssetDict.Add(sa.SkillId, sa);
        }
    }


    public int Func(string rootSkill, int depth)
    {
        SkillAsset sa = GetSkillAsset(rootSkill);
        int totalWidth = 0;
        foreach(SkillPrerequist pre in sa.PrerequistSkills)
        {
            string nextId = pre.skillId;
            int width = Func(nextId, depth + 1);
            totalWidth += width;
        }
        return totalWidth;
    }

    public static void ConstructGraph()
    {

    }

    public List<string> GetSkillByType(eSkillType type)
    {
        List<string> ret = new List<string>();
        foreach(var kv in SkillAssetDict)
        {
            if(kv.Value.SkillType== type)
            {
                ret.Add(kv.Key);
            }
        }
        return ret;
    }

    public void PrintSkills()
    {
        Debug.Log("----------");
        for(int i=0;i< OwnedSkills.Count; i++)
        {
            Debug.Log(OwnedSkills[i].SkillId + "," + OwnedSkills[i].SkillLvl);
        }

    }

}
