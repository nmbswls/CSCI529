using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SkillTreeMgr2 : ModuleBase
{

    IResLoader mResLoader;
    ICardDeckModule mCardMgr;
    IRoleModule mRoleMdl;
    IUIMgr mUIMgr;

    public List<SkillInfo2> OwnedSkills = new List<SkillInfo2>();

    private static Dictionary<string, SkillInfo2> SkillAssetDict = new Dictionary<string, SkillInfo2>();

    public Dictionary<int, Dictionary<int, string>> SkillBranchDict = new Dictionary<int, Dictionary<int, string>>();


    public HashSet<string> LearnedSkill = new HashSet<string>();

    public int[] branchLevel = { 0,0,0,0,0,0 };

    //private readonly static Dictionary<string, SkillAsset> SkillAssetDict = new Dictionary<string, SkillAsset>();

    

    public override void Setup()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        mRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        loadSkill2Asset();
    }

    public void loadSkill2Asset()
    {
        SkillAsset2Collection newSkillCollection  = mResLoader.LoadResource<SkillAsset2Collection>("SkillsNewType/SkillAsset2Collection");

        //for(int i = 0; i<5; i++)
        //{
        //    for(int j = 0; j<5; j++)
        //    {
        //        SkillAsset2 asset = mResLoader.LoadResource<SkillAsset2>("SkillsNewType/Skills/")
        //    }
           
        //}

        foreach(SkillAsset2List list in newSkillCollection.SkillCollection) {
            foreach(SkillAsset2 asset in list.SkillBranch)
            {
                SkillInfo2 assetInf = new SkillInfo2();
                assetInf.SkillId = asset.SkillId;
                assetInf.SkillName = asset.SkillName;
                assetInf.Des = asset.SkingDesp;
                assetInf.EffectDes = asset.SkingEffectDesp;
                assetInf.Branch = asset.Branch;
                assetInf.Level = asset.Level;
                assetInf.Requirements = asset.Requirements;
                assetInf.Rewards = asset.Rewards;

                OwnedSkills.Add(assetInf);
                if(!SkillAssetDict.ContainsKey(assetInf.SkillId))
                {
                    SkillAssetDict[asset.SkillId] = assetInf;
                }

                if(!SkillBranchDict.ContainsKey(assetInf.Branch))
                {
                    SkillBranchDict[assetInf.Branch] = new Dictionary<int, string>();
                }

                if(!SkillBranchDict[assetInf.Branch].ContainsKey(assetInf.Level)) {
                    SkillBranchDict[assetInf.Branch][assetInf.Level] = assetInf.SkillId;
                }
                
            }
        }
    }

    public SkillInfo2 GetSkillAsset(string skillId)
    {
        if (skillId == null || skillId.Length == 0) return null;
        Debug.Log("selected skill is " + skillId);
        if(!SkillAssetDict.ContainsKey(skillId))
        {
            return null;
        }
        return SkillAssetDict[skillId];
    }

    public bool learnSkill(string skillId)
    {

        SkillInfo2 skill = GetSkillAsset(skillId);
        if (skill == null)
        {
            return false;
        }
        
        if(skill.isLearned)
        {
            mUIMgr.ShowHint("这个技能已经学会了");
            return false;
        }
        
        if(!checkSkillRequirement(skill))
        {
            return false;
        }

        if(!checkBranch(skill.Branch, skill.Level))
        {
            mUIMgr.ShowHint("你需要先学习前一个技能");
            return false;
        }
        
        skill.isLearned = true;
        branchLevel[skill.Branch]++;
        // calculate requirement    
        mRoleMdl.AddSkillPoint(-skill.Requirements.reqSkillPointValue);

        // gainRewards
        for(int i = 0; i < skill.Rewards.bonus.Count; i++)
        {
            switch (skill.Rewards.bonus[i])
            {
                case SkillBonusType.addKoucai:
                    mRoleMdl.AddKoucai(skill.Rewards.rewValue[i]);
                    mUIMgr.ShowHint("口才 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addCaiyi:
                    mRoleMdl.AddCaiyi(skill.Rewards.rewValue[i]);
                    mUIMgr.ShowHint("才艺 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addJishu:
                    mRoleMdl.AddJishu(skill.Rewards.rewValue[i]);
                    mUIMgr.ShowHint("技术 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addKangya:
                    mRoleMdl.AddKangya(skill.Rewards.rewValue[i]);
                    mUIMgr.ShowHint("抗压 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addWaiguan:
                    mRoleMdl.AddWaiguan(skill.Rewards.rewValue[i]);
                    mUIMgr.ShowHint("外观 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addPower:
                    //加能量
                    break;
                case SkillBonusType.addMoney:
                    mUIMgr.ShowHint("金钱 + " + skill.Rewards.rewValue[i]);
                    break;
                case SkillBonusType.addFensi:
                    mUIMgr.ShowHint("粉丝 + " + skill.Rewards.rewValue[i]);
                    break;
            }
        }

        LearnedSkill.Add(skillId);

        return true;
    }

    public bool checkSkillRequirement(SkillInfo2 skill)
    {
        for(int i = 0; i<skill.Requirements.reqStats.Count; i++)
        {
            switch(skill.Requirements.reqStats[i])
            {
                case SkillReqType.koucai:
                    if(mRoleMdl.GetStats().koucai<skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("口才不足");
                        return false;
                    }
                    break;
                case SkillReqType.caiyi:
                    if (mRoleMdl.GetStats().caiyi < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("才艺不足");
                        return false;
                    }
                    break;
                case SkillReqType.jishu:
                    if (mRoleMdl.GetStats().jishu < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("技术不足");
                        return false;
                    }
                    break;
                case SkillReqType.kangya:
                    if (mRoleMdl.GetStats().kangya < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("抗压不足");
                        return false;
                    }
                    break;
                case SkillReqType.waiguan:
                    if (mRoleMdl.GetStats().waiguan < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("外观不足");
                        return false;
                    }
                    break;
                case SkillReqType.fensi:
                    if (mRoleMdl.GetStats().waiguan < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("好像还没有那么火");
                        return false;
                    }
                    break;
                case SkillReqType.money:
                    if (mRoleMdl.GetStats().waiguan < skill.Requirements.reqValues[i])
                    {
                        mUIMgr.ShowHint("目前还没有那么多钱");
                        return false;
                    }
                    break;
            }
        }
        if(mRoleMdl.GetSkillPoint() < skill.Requirements.reqSkillPointValue)
        {
            mUIMgr.ShowHint("技能点不足");
            return false;
        }
        return true;
    }

    public bool checkBranch(int branch, int level)
    {
        if(branch>4)
        {
            Debug.Log("Special Branch");
            return true;
        }

        if(branchLevel[branch] == level)
        {
            return true;
        }
        return false;
    }

}
