using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardInfo{
	public uint InstId;
	public string CardId;
	public float GainTime;
    public int TurnLeft;

	public CardInfo(){
	}

	public CardInfo(uint InstId, string CardId, float GainTime){
		this.InstId = InstId;
		this.CardId = CardId;
		this.GainTime = GainTime;
	}
}

public class CardDeckModule : ModuleBase, ICardDeckModule
{

	uint InstId;

	Dictionary<uint, CardInfo> CardInstDict = new Dictionary<uint, CardInfo> ();

	List<CardInfo> cards = new List<CardInfo>();
	Dictionary<string, CardAsset> CardDict = new Dictionary<string, CardAsset>();


    public void GainNewCard (string cid)
	{
		CardAsset aset = null;
		CardDict.TryGetValue (cid, out aset);
		if (aset == null) {
			aset = Load (cid);
		}
		if (aset != null) {
			cards.Add (new CardInfo(InstId, cid,Time.realtimeSinceStartup));
			InstId += 1;
		}
	}

    public void fakeCards()
    {
        GainNewCard("card0001");
        GainNewCard("card0001");
        GainNewCard("card0001");
        GainNewCard("card0001");
        GainNewCard("card0002");
        GainNewCard("card0003");
        GainNewCard("card0004");
        GainNewCard("card0005");
        GainNewCard("card0006");
        GainNewCard("card0007");
    }


    public void CheckOverdue()
    {
        for(int i= cards.Count - 1; i >= 0; i--)
        {
            if (cards[i].TurnLeft > 0)
            {
                cards[i].TurnLeft -= 1;
                if (cards[i].TurnLeft <= 0)
                {
                    cards.RemoveAt(i);
                }
            }

        }
    }


    public List<CardInfo> GetAllCards()
    {
        return cards;
    }
    public override void Setup(){
		InstId = 0;
        fakeCards();

    }

	public CardAsset Load(string cid){
		CardAsset c = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<CardAsset> ("Cards/"+cid,false);
		if (c != null) {
			CardDict [cid] = c;
		}
		return c;
	}


    public CardAsset GetCardInfo(string cid){
		CardAsset ret = null;
		CardDict.TryGetValue (cid, out ret);
        if(ret == null)
        {
            ret = Load(cid);
        }
        return ret;
	}

	public List<CardInfo> SortAllCard(List<eCardType> FTypes){
		List<CardInfo> ret = new List<CardInfo> ();
		for (int i = 0; i < cards.Count; i++) {
			if (FTypes.Contains (CardDict[cards[i].CardId].CardType)) {
				ret.Add (cards[i]);
			}
		}

		ret.Sort (new Comparison<CardInfo>((CardInfo c1, CardInfo c2) =>
			{
				if(c1.GainTime < c2.GainTime){
					return -1;
				}else if(c1.GainTime > c2.GainTime){
					return 1;
				}
				return 0;
			}));
		return ret;
	}

	//public void RemoveCard(int idx){
	//	cards.RemoveAt (idx);
	//}
	//public void RemoveCard(CardInfo c){
	//	cards.Remove (c);
	//}

}

