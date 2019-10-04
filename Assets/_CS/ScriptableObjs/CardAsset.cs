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

	public eCardType CardType;

	public bool IsConsume;

	public bool OverDueTurn;

	public List<object> args = new List<object>();

    public List<CardEffect> effects = new List<CardEffect>();

}

