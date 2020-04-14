using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SkillReqType
{
    none,
    koucai,
    caiyi,
    jishu,
    kangya,
    waiguan
}

public enum SkillBonusType
{
    none,
    addKoucai,
    addCaiyi,
    addJishu,
    addKangya,
    addWaiguan,
    addPower
}

[System.Serializable]
public class SkillReq
{
    public List<SkillReqType> reqStats;
    public List<int> reqValues;
    public int reqSkillPointValue = 1;
}

[System.Serializable]
public class SkillReward
{
    public List<SkillBonusType> bonus;
    public List<int> rewValue;
}


public class SkillInfo2
{
    public string SkillId;
    public string Name;
    public string EffectDes;
    public string Des;
    public bool isLearned = false;
    public int Branch;
    public int Level;
    SkillReq Requirements;
    SkillReward Rewards;
}



public class SkillTreeMgr2 : ModuleBase
{

    IResLoader mResLoader;
    ICardDeckModule mCardMgr;
    IRoleModule mRoleMdl;

    public List<SkillInfo2> OwnedSkills = new List<SkillInfo2>();


    //private readonly static Dictionary<string, SkillAsset> SkillAssetDict = new Dictionary<string, SkillAsset>();

    

    public override void Setup()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        mRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
    }

    public void loadSkill2Asset()
    {
        
    }
}
