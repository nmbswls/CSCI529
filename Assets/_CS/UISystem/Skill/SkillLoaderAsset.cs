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
    public string SkillName;
    public string EffectDes;
    public string Des;
    public bool isLearned = false;
    public int Branch;
    public int Level;
    public SkillReq Requirements;
    public SkillReward Rewards;

}