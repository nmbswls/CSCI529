using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class SkillInfo
{
    public string SkillId;
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

    public List<SkillInfo> OwnedSkills = new List<SkillInfo>();

    private readonly static Dictionary<string, SkillAsset> SkillAssetDict = new Dictionary<string, SkillAsset>();

    public override void Setup()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();

        FakeSkillTree();
    }

    private void FakeSkillTree()
    {
        GainSkills("game_01");
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
        mCardMgr.AddSkillCards(skillId,sa.AttachCards[skillInfo.SkillLvl]);

    }



    public SkillInfo GetOwnedSkill(string SkillId)
    {
        for(int i = 0; i < OwnedSkills.Count - 1; i++)
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

}
