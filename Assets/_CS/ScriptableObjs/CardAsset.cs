using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public enum eCardType{
	ABILITY,
	GENG,
	ITEM,
	STATUS
}

[System.Serializable]
public class CardEffect
{
    public string effect;

    [TextArea(1,2)]
    public string effectString;

    public CardEffect()
    {

    }

    public CardEffect(string effect, string effectString)
    {
        this.effect = effect;
        this.effectString = effectString;
    }
}

[CreateAssetMenu(fileName="new card",menuName="Ctm/card")]
[System.Serializable]
public class CardAsset : ScriptableObject
{

	public string CardId;

	public string CardName;

	[TextArea(3,10)]
	public string CardDesp;

    [TextArea(3, 10)]
    public string CardEffectDesp;

    public eCardType CardType;

    public int cost = 0;

	public bool IsConsume;

    //public bool WillOverdue;

    public bool UseOnDiscard;

    public bool HasTurnEffect;

    //过期回合数
	public int OverDueTurn;

    //单局游戏中使用次数
    public int UseTime;

	public List<object> args = new List<object>();

    public string ModelCard;

    public int StatusBonusNum;
    public int StatusBonusType;

    public int SkillBonusType;
    public int SkillBonusNum;

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
}

