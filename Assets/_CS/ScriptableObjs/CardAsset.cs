using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public string x;
    public string y;
    public string z;
    public string w;
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

    public int cost=0;

	public bool IsConsume;

    //过期回合数
	public int OverDueTurn;

    //在手牌中存活时间
    public float ValidTime;

    //单局游戏中使用次数
    public int UseTime;

	public List<object> args = new List<object>();

    public List<CardEffect> effects = new List<CardEffect>();

}

