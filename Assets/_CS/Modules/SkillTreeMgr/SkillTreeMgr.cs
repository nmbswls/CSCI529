using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class SkillInfo
{
    public string SkillId;
    public float NowExp;
    public int SkillLvl;

    public SkillInfo()
    {

    }

    public SkillInfo(string skillId)
    {
        this.SkillId = skillId;
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
        mRoleMdl = GameMain.GetInstance().GetModule<IRoleModule>();

        LoadAllSkills();
        FakeSkillTree();
    }

    private void FakeSkillTree()
    {
        //GainSkills("game_01");
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
            skillInfo = new SkillInfo(skillId);
            OwnedSkills.Add(skillInfo);
            skillInfo.SkillLvl = 1;

        }
        else
        {
            skillInfo.SkillLvl += 1;
            mCardMgr.RemoveSkillCards(skillId);
        }
        if(skillInfo.SkillLvl - 1 < sa.AttachCards.Count)
        {
            string[] cards = sa.AttachCards[skillInfo.SkillLvl - 1].Split(',');
            mCardMgr.AddSkillCards(skillId, new List<string>(cards));
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

    public List<string> GetSkillByType(string type)
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


}
