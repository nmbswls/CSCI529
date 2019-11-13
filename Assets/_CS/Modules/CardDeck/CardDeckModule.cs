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
    public bool isDisabled;

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

    List<uint> UsedItemList = new List<uint>();



    public override void Setup()
    {

        InstId = 0;
        pRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
        GenFakeCards();
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
            CardInstDict.Add(InstId,info);
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
                    switch (effect.turnEffect)
                    {
                        case "jiyi+":
                            pRoleMdl.AddJiyi(float.Parse(effect.effectString));
                            break;

                        case "meili+":
                            pRoleMdl.AddMeili(float.Parse(effect.effectString));
                            break;
                        case "fanying+":
                            pRoleMdl.AddFanying(float.Parse(effect.effectString));
                            break;
                        case "tili+":
                            pRoleMdl.AddTili(float.Parse(effect.effectString));
                            break;
                        case "koucai+":
                            pRoleMdl.AddKoucai(float.Parse(effect.effectString));
                            break;
                        case "shuxing+":
                            pRoleMdl.AddJiyi(float.Parse(effect.effectString));
                            pRoleMdl.AddMeili(float.Parse(effect.effectString));
                            pRoleMdl.AddFanying(float.Parse(effect.effectString));
                            pRoleMdl.AddTili(float.Parse(effect.effectString));
                            pRoleMdl.AddKoucai(float.Parse(effect.effectString));
                            break;
                        case "fensi+":
                            pRoleMdl.AddFensi(0,int.Parse(effect.effectString));
                            break;
                        case "action+":
                            pRoleMdl.AddActionPoints(int.Parse(effect.effectString));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void ChangeSkillCard(string skillId, string fromCard, string toCard)
    {
        if (!SkillCardDict.ContainsKey(skillId))
        {
            return;
        }

        if (fromCard == null)
        {
            AddSkillCards(skillId,new List<string>() { toCard});
            return;
        }



        CardInfo info = null;
        for(int i=0;i< SkillCardDict[skillId].Count; i++)
        {
            if(SkillCardDict[skillId][i].CardId == fromCard)
            {
                info = SkillCardDict[skillId][i];
                break;
            }
        }

        if (info == null)
        {
            return;
        }

        if (toCard == null)
        {
            RemoveCard(info);
            SkillCardDict[skillId].Remove(info);
        }
        else
        {
            info.CardId = toCard;
            info.ca = GetCardInfo(toCard);
            info.InstId = InstId;
            InstId++;
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
        CardInstDict.Remove(info.InstId);
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

    public List<CardInfo> GetTypeCards(eCardType type)
    {
        List<CardInfo> ret = new List<CardInfo>();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].ca.CardType == type)
            {
                ret.Add(cards[i]);
            }
        }
        return ret;
    }

    public List<CardInfo> GetAllEnabledCards()
    {
        List<CardInfo> ret = new List<CardInfo>();
        for(int i = 0; i < cards.Count; i++)
        {
            if (!cards[i].isDisabled)
            {
                ret.Add(cards[i]);
            }
        }
        return ret;
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

    public CardInfo GetCardByInstId(uint instId)
    {
        if (CardInstDict.ContainsKey(instId))
        {
            return CardInstDict[instId];
        }
        return null;
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

    public bool ChangeEnable(uint instId, bool enable)
    {
        CardInfo target = GetCardByInstId(instId);
        if(target == null)
        {
            return false;
        }
        target.isDisabled = !enable;

        if(target.ca.CardType == eCardType.ITEM)
        {
            if (enable)
            {
                if(UsedItemList.Count >= pRoleMdl.MaxItemNum)
                {
                    return false;
                }
                UsedItemList.Add(target.InstId);
            }
            else
            {
                UsedItemList.Remove(target.InstId);
            }
        }
        return true;
    }

    //public void RemoveCard(int idx){
    //	cards.RemoveAt (idx);
    //}
    //public void RemoveCard(CardInfo c){
    //	cards.Remove (c);
    //}


    public void GenFakeCards()
    {
       

        for (int i = 0; i < 30; i++)
        {
            CardAsset ca = new CardAsset();
            ca.CardName = "技能卡";
            ca.CardType = eCardType.ABILITY;
            ca.CardId = string.Format("test_{0:00}", i + 1);
            ca.CardEffectDesp = "等级" + (i + 1) + "的卡";
            ca.CatdImageName = "Image_Bangyigegezuibangla";
            ca.BaseSkillId = string.Format("test_{0:00}", (i + 1)/5); ;
            ca.cost = 2;
            {
                CardEffect ce = new CardEffect();
                ce.EMode = eCardEffectMode.SIMPLE;
                ce.effectType = eEffectType.GetScore;
                ce.effectString = ((i + 1) * 5) + "";
                ca.Effects.Add(ce);
            }
            CardDict.Add(ca.CardId,ca);
        }
        for (int i = 0; i < 30; i++)
        {
            CardAsset ca = new CardAsset();
            ca.CardName = "回血卡";
            ca.CardType = eCardType.ABILITY;
            ca.CardId = string.Format("test_xue_{0:00}", i + 1);
            ca.CardEffectDesp = "等级" + (i + 1) + "的回血卡";
            ca.CatdImageName = "Image_Kongqibanfan";
            ca.BaseSkillId = string.Format("test_{0:00}", (i + 1) / 5); ;
            ca.cost = 2;
            {
                CardEffect ce = new CardEffect();
                ce.EMode = eCardEffectMode.SIMPLE;
                ce.effectType = eEffectType.AddHp;
                ce.effectString = ((i + 1) * 1) + "";
                ca.Effects.Add(ce);
            }
            CardDict.Add(ca.CardId, ca);
        }
        for (int i = 0; i < 30; i++)
        {
            CardAsset ca = new CardAsset();
            ca.CardName = "防御卡";
            ca.CardType = eCardType.ABILITY;
            ca.CardId = string.Format("test_armor_{0:00}", i + 1);
            ca.CardEffectDesp = "等级" + (i + 1) + "的防御卡";

            ca.CatdImageName = "Image_Zhaohuanshuijun";
            ca.BaseSkillId = string.Format("test_{0:00}", (i + 1) / 5); ;
            ca.cost = 2;
            {
                CardEffect ce = new CardEffect();
                ce.EMode = eCardEffectMode.SIMPLE;
                ce.effectType = eEffectType.GetArmor;
                ce.effectString = ((i + 1) * 3) + "";
                ca.Effects.Add(ce);
            }
            CardDict.Add(ca.CardId, ca);
        }

    }

}

