using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

[System.Serializable]
public enum eCardType{
	ABILITY = 0x01,
	GENG = 0x02,
	ITEM = 0x04,
	STATUS = 0x08
}

[System.Serializable]
public enum eCardEffectMode
{
    SIMPLE=0,
    BRANCHES=1,
}

[System.Serializable]
public class CardEffectBranch
{
    public string effect;
    public string effectString;
    public int value;
}



[System.Serializable]
public class CardEffect
{
    public eCardEffectMode EMode;
    public eEffectType effectType;

    public bool isNegEffect = false;
    public int Possibility = 100;

    public string turnEffect;
    [TextArea(1,2)]
    public string effectString;

    public bool isAddBuff;
    public ZhiboBuffInfo buffInfo;

    public string BranchType;
    public List<CardEffectBranch> BranchEffectStrings;

    public CardEffect()
    {

    }

    public CardEffect(string effectId, string effectString)
    {
        this.effectType = (eEffectType)Enum.Parse(typeof(eEffectType), effectId);
        this.effectString = effectString;
    }
}

public class CardFilter
{
    public string NameContain = string.Empty;
    public int TypeMask = 0;
    public List<string> Tags = new List<string>();

    public CardFilter()
    {

    }
    public static CardFilter parseFilterFromString(string filterString)
    {
        CardFilter ret = new CardFilter();
        try
        {
            ret = JsonUtility.FromJson<CardFilter>(filterString);
        }catch(Exception e)
        {
            Debug.Log(e.StackTrace);
        }
        return ret;
    }
}

[CreateAssetMenu(fileName="new card",menuName="Ctm/card")]
[System.Serializable]
public class CardAsset : ScriptableObject
{

	public string CardId = "";

	public string CardName = "";

	[TextArea(3,10)]
	public string CardDesp = "";

    public string CatdImageName = "";

    [TextArea(3, 10)]
    public string CardEffectDesp = "";

    public Sprite Picture;

    public eCardType CardType;

    public List<string> Tags = new List<string>();

    public int cost = 0;

	

    //public bool WillOverdue;

    public bool UseOnDiscard;

    public bool HasTurnEffect;

    //过期回合数
	public int OverDueTurn;

    public bool IsConsume;
    //单局游戏中使用次数
    public int UseTime;

	public List<object> args = new List<object>();

    public string ModelCard;

    public int StatusBonusNum;
    public int StatusBonusType;

    public int SkillBonusType;
    public int SkillBonusNum;

    public string BaseSkillId;// is exist

    public List<CardEffect> UseConditions = new List<CardEffect>();
    public List<CardEffect> Effects = new List<CardEffect>();
    public List<CardEffect> TurnEffects = new List<CardEffect>();

    public string ReplaceWithAmountInEffect()
    {
        MatchCollection matches = Regex.Matches(CardEffectDesp,"\\{[a-z0-9A-Z\\+\\.]+\\}");

        for (int i= 0;i< matches.Count; i++)
        {
            //Debug.Log(CardEffectDesp.Substring(matches[i].Index,matches[i].Length));
        }

        return CardEffectDesp;
    }

    public string ReplaceWithTextInEffect()
    {
        return CardEffectDesp;
    }

    public bool ApplyFilter(CardFilter filter)
    {
        bool ret = true;
        if(filter.NameContain != null && filter.NameContain != string.Empty)
        {
            if (!CardName.Contains(filter.NameContain))
            {
                return false;
            }
        }
        if(filter.TypeMask > 0)
        {
            if(((int)(CardType) & filter.TypeMask) == 0)
            {
                return false;
            }
        }
        if(filter.Tags.Count > 0)
        {

            for(int i = 0; i < filter.Tags.Count; i++)
            {
                if (Tags.Contains(filter.Tags[i]))
                {
                    return true;
                }
            }
            return false;
        }
        return true;
    }
}


public enum eEffectType
{
    SpawnGift = 0, // 刷新礼物 参数： 礼物类型 string, 礼物数量 int

    SpeedUp,
    GenGoodDanmu,
    GenBadDanmu,
    GenMixedDanmu,

    PickAndUse,
    GetScoreWithZengfu,
    GetScore,
    GetChouka,
    GetTili,

    GetStatus,

    AddTurnBuff,
    AddRemoveAward,
    AddNextCardsBuff,
    ClearDanmu,
    AddCardToDeck,
    CostAll,
    Dual,
    GetArmor,
    GainCard,
    DiscardCards,
    GetHot,

    ScoreMultiple,

    ExtraMoney,
    ExtraLiuliang,
    GetCertainCard,

    EndFollowingEffect,
    AddHp,

    //前置条件
    HavaCost,
    MaxCount,

}

public enum eBranchType
{
    Random,
    NextCardCost,
}