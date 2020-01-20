using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

[System.Serializable]
public enum eCardType{
    None = 0,
	ABILITY = 0x01,
	GENG = 0x02,
	ITEM = 0x04,
	STATUS = 0x08
}



[System.Serializable]
public enum eCardStatusBonus
{
    None = 0,
    Meili = 1,
    Koucai = 2,
    Tili = 3,
    Jiyi = 4,
    Fanying = 5,
}

[System.Serializable]
public class CardEffectBranch
{
    //public string effect;
    //public string effectString;
    //public int value;
    public string condition;
    public List<CardEffect> BranchEffects = new List<CardEffect>();
}



[System.Serializable]
public class CardEffect
{
    public eEffectType effectType;
    public int Possibility = 100;

    [TextArea(1,2)]
    public string effectString;

    public bool isAddBuff;
    public List<ZhiboBuffInfo> buffInfo = new List<ZhiboBuffInfo>();

    //public string BranchType;
    //public List<CardEffectBranch> BranchEffectStrings;

    public CardEffect()
    {

    }

    public CardEffect(string effectId, string effectString)
    {
        this.effectType = (eEffectType)Enum.Parse(typeof(eEffectType), effectId);
        this.effectString = effectString;
    }
}

public enum eCardTurnEffectType
{
    None = 0,
    Jiyi,
    Meili,
    Tili,
    Koucai,
    Fanying,
    Shuxing,

    Fensi,
    Xingdongdian,


}
public class CardTurnEffect
{
    public eCardTurnEffectType type;
    public float value;
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

    [TextArea(3, 10)]
    public string CardBackDesp = "";

    public Sprite Picture;

    public eCardType CardType;

    public List<string> Tags = new List<string>();

    //消耗相关
    public bool isCostAll;
    public int cost = 0;

	


    public bool TriggerOnDiscard; //在被丢弃时发动效果

    public bool HasTurnEffect; //是否有单局外附加效果
    public List<CardTurnEffect> TurnEffects = new List<CardTurnEffect>(); 

    public int OverDueTurn; //持有超过一定回合数将被丢弃

    public bool IsConsume; //单局内是否消耗
    public int UseTime; // 消耗次数
    public int maxUseAmountPerGame; //单局同名卡使用次数上限


    public string ModelCard; //暂无使用

    public int StatusBonusNum; //
    public eCardStatusBonus StatusBonusType;

    public int SkillBonusType;
    public int SkillBonusNum;

    public string BaseSkillId;// is exist

    public bool canUseFace = true;
    public bool canUseBack = true;

    public string GemString = "0,0,0,0,0,0";
    //public int[] Gems = new int[6];


    //public List<CardEffect> UseConditions = new List<CardEffect>();

    public List<CardEffect> Effects = new List<CardEffect>();

    public eBranchType branchType;
    public List<CardEffectBranch> ExtraBranches = new List<CardEffectBranch>();


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
    None = 0,
    SpawnGift, // 刷新礼物 参数： 礼物类型 string, 礼物数量 int

    //SpeedUp,
    //GenGoodDanmu,
    //GenBadDanmu,
    //GenMixedDanmu,

    PickAndUse,
    GetScoreWithZengfu,
    GetScore,
    //GetChouka,
    GetTili,

    //AddTurnBuff,
    //AddRemoveAward,
    //AddNextCardsBuff,
    //ClearDanmu,
    AddCardToDeck,
    CostAll,
    Dual,
    GetArmor,
    GainCard,
    DiscardCards,
    //GetHot,

    ScoreMultiple,

    ExtraMoney,
    ExtraLiuliang,
    GetCertainCard,

    EndFollowingEffect,
    AddHp,

    HitGem,
    HitGemRandomly,
    PutScoreOnAudience,

    KillHeizi,
    HitHeizi,
    //前置条件
    //HavaCost,
    //MaxCount,

}

public enum eBranchType
{
    None = 0,
    Random,
    NextCardCost,
}