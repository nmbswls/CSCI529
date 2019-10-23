using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardInfo{
	public uint InstId;
	public string CardId;
	public float GainTime;
    public CardAsset ca;
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
    IRoleModule pRoleMdl;

	Dictionary<uint, CardInfo> CardInstDict = new Dictionary<uint, CardInfo> ();

	List<CardInfo> cards = new List<CardInfo>();
	Dictionary<string, CardAsset> CardDict = new Dictionary<string, CardAsset>();


    Dictionary<string, List<CardInfo>> SkillCardDict = new Dictionary<string, List<CardInfo>>();
    HashSet<CardInfo> CardsWithTurnEffect = new HashSet<CardInfo>();


    public override void Setup()
    {

        InstId = 0;
        pRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
    }

    public CardInfo GainNewCard (string cid)
	{
		CardAsset aset = null;
		CardDict.TryGetValue (cid, out aset);
		if (aset == null) {
			aset = Load (cid);

		}
		if (aset != null) {
            aset.ReplaceWithAmountInEffect();
            CardInfo info = new CardInfo(InstId, cid, Time.realtimeSinceStartup);
            info.ca = aset;
            cards.Add (info);
			InstId += 1;

            if (aset.HasTurnEffect || aset.TurnEffects.Count > 0)
            {
                CardsWithTurnEffect.Add(info);
            }

            return info;
		}
        return null;
	}


    public void CheckTurnBonux()
    {
        foreach(CardInfo info in cards)
        {
            if (info.ca.HasTurnEffect)
            {
                foreach (CardEffect effect in info.ca.TurnEffects)
                {
                    switch (effect.effect)
                    {
                        case "jiyi+":
                            pRoleMdl.AddJiyi(int.Parse(effect.effectString));
                            break;
                        case "meili+":
                            pRoleMdl.AddMeili(int.Parse(effect.effectString));
                            break;
                        case "fanying+":
                            pRoleMdl.AddFanying(int.Parse(effect.effectString));
                            break;
                        case "tili+":
                            pRoleMdl.AddTili(int.Parse(effect.effectString));
                            break;
                        case "koucai+":
                            pRoleMdl.AddKoucai(int.Parse(effect.effectString));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void RemoveSkillCards(string skillId)
    {
        if (!SkillCardDict.ContainsKey(skillId))
        {
            return;
        }
        foreach(CardInfo info in SkillCardDict[skillId])
        {
            RemoveCard(info);
        }
        SkillCardDict[skillId].Clear();
    }

    public void RemoveCard(CardInfo info)
    {
        cards.Remove(info);
        CardsWithTurnEffect.Remove(info);
    }

    public void AddSkillCards(string skillId, List<string> cid)
    {
        if (!SkillCardDict.ContainsKey(skillId))
        {
            SkillCardDict.Add(skillId, new List<CardInfo>());
        }

        foreach (string id in cid)
        {
            CardInfo info = GainNewCard(id);
            SkillCardDict[skillId].Add(info);
        }
    }


    public void AddCards(List<string> cards)
    {
        foreach (string id in cards)
        {
            CardInfo info = GainNewCard(id);
        }
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
                    RemoveCard(cards[i]);
                }
            }

        }
    }


    public List<CardInfo> GetAllCards()
    {
        return cards;
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

