using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eSkillType
{
    None,
    Koucai,
    Game,
}

[System.Serializable]
public class SkillPrerequist
{
    public string skillId;
    public int level;
}

public enum eCardOperatorMode
{
    Add,
    Replace,
}

[System.Serializable]
public class CardOperator
{
    public eCardOperatorMode opt;
    public string from;
    public string to;
}

[System.Serializable]
public class AttachCardsInfo
{
    public List<CardOperator> operators = new List<CardOperator>();
}

[CreateAssetMenu(fileName = "new base skill", menuName = "Ctm/BaseSkill")]
[System.Serializable]
public class BaseSkillAsset : SkillAsset
{
    //基础固定卡片
    public List<string> BaseCardList = new List<string>();

    //基础技能 升级靠学习 所以每个等级的技能还有额外的属性要求
    public List<string> PrerequistStatus = new List<string>();

    //每个等级 都有固定数量的可配置卡片
    public List<int> FreeCardNum = new List<int>();

    //每个等级 都有对应固定bonus加成
    public List<float[]> StatusBonus = new List<float[]>();



}

[CreateAssetMenu(fileName = "new extend skill", menuName = "Ctm/ExtendSkill")]
[System.Serializable]
public class ExtentSkillAsset : SkillAsset
{
    public string BaseSkillId = null;

    //每个等级的难度
    public List<int> Difficulties = new List<int>();

    //每个等级都会有相应的附加卡片
    public List<AttachCardsInfo> AttachCardInfos = new List<AttachCardsInfo>();



}

[System.Serializable]
public class SkillAsset : ScriptableObject
{
    public string SkillId;

    public string SkillName;

    [TextArea(3, 10)]
    public string SkingDesp;

    public int MaxLevel;

    public eSkillType SkillType = eSkillType.Game;

    //与等级无关的前置条件，即前置技能
    public List<SkillPrerequist> PrerequistSkills = new List<SkillPrerequist>();

    //每级
    public List<int> LevelStatusAdd = new List<int>();

    //每级效果说明
    public List<string> LevelDesp = new List<string>();

    //每个等级 升到该等级需要的开销 普通技能只有第一级有开销
    public List<int> Prices = new List<int>();

}
