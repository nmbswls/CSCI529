using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eReactorType
{
    PRESENT,
    CAUTION,
    TUHAO
}

public class DanmuGroup
{
    public int TotalNum;
    public string key;
    public int BadNum;

    public DanmuGroup(string key, int num, int BadNum)
    {
        this.key = key;
        this.TotalNum = num;
        this.BadNum = BadNum;
    }
}

public class CardInZhibo
{
    public string CardId;
    public float TimeLeft;
    public int UseLeft;
    public CardAsset ca;
    public bool NeedDiscard;

    public CardInZhibo()
    {

    }

    public CardInZhibo(string CardId, int UseLeft)
    {
        this.CardId = CardId;
        this.UseLeft = UseLeft;
    }
    public CardInZhibo(CardAsset ca)
    {
        this.CardId = ca.CardId;
        //this.TimeLeft = ca.ValidTime;
        this.UseLeft = ca.UseTime;
        this.ca = ca;
    }
}

public class ZhiboGameState
{
    public RoleStats stats;

    public int TurnLeft = 12;

    public List<ZhiboBuff> ZhiboBuffs = new List<ZhiboBuff>();

    public List<Danmu> Danmus = new List<Danmu>();
    public List<SuperDanmu> SuperDanmus = new List<SuperDanmu>();
    public List<int> SuperDanmuShowTimeList;
    public int NowSuperDanmuIdx;


    public List<CardInZhibo> Cards = new List<CardInZhibo>();
    public List<CardInZhibo> CardDeck = new List<CardInZhibo>();
    public List<CardInZhibo> CardUsed = new List<CardInZhibo>();

    public List<ZhiboSpecial> Specials = new List<ZhiboSpecial>();

    public float Score = 0;
    public int MaxScore = 100;

    public float ScoreArmor = 0;

    public float TurnTimeLeft = 30;
    public float Qifen = 0;
    //public int ChoukaYuzhi = 100;
    public float Tili = 0;

    public float DanmuFreq { get { return danmuFreq * AccelerateRate; } set { danmuFreq = value; } }
    private float danmuFreq = 5f;

    public float DanmuSpd { get { return danmuSpd * AccelerateRate; } }

    private float danmuSpd = 160.0f;


    public float AccelerateRate = 1.0f;
    public float AccelerateDur = 0f;

    public int[] BuffAddValue = new int[5];
    public int[] BuffAddPercent = new int[5];

    public List<string> ComingEmergencies = new List<string>();

    public List<DanmuGroup> danmuGroups = new List<DanmuGroup>();
    public List<string> TurnSpecials = new List<string>();

    public List<string> UsedCardsToGetBonus = new List<string>();
}
public class ZhiboGameMode : GameModeBase
{


    IUIMgr mUIMgr;
    IResLoader mResLoader;
    IRoleModule pRoleMgr;
    ICardDeckModule mCardMdl;
    public ZhiboUI mUICtrl;

    public ZhiboGameState state;

    public float spdRate = 1.0f;


    public float lastTick = 0;
    public float nextTick = 0;

    private int bigOneNext = 3;
    private int bigOneCount = 0;

    public int CardMax = 10;
    private float SecTimer = 0;
    private int SecCount = 0;

    private EmergencyAsset nowEmergency = null;

    private Dictionary<string, List<string>> DanmuDict = new Dictionary<string, List<string>>();

    private Dictionary<string, string> BuffDesp = new Dictionary<string, string>();

    private Dictionary<int, float> WeisuijiDict = new Dictionary<int, float> { 
        { 5, 0.0038f }, 
        { 10, 0.01475f },
        { 15,0.03321f },
        { 20,0.0557f },
        { 25,0.08475f },
        { 30,0.11895f },
        { 35,0.14628f },
        { 40,0.18128f }
        };
    public override void Init()
    {
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();

        state = new ZhiboGameState();

        state.stats = new RoleStats(pRoleMgr.GetStats());


        state.ZhiboBuffs.Clear();
        state.Cards.Clear();
        state.Danmus.Clear();

        state.TurnLeft = 12;
        state.TurnTimeLeft = 30f;

        state.Score = 0;
        state.MaxScore = 100;

        state.Qifen = 300;
        state.Tili = 10;

        spdRate = 1.0f;
        lastTick = 0;
        nextTick = 0;
        bigOneNext = 3;
        bigOneCount = 0;

        LoadDanmuDict();
        LoadCard();
        LoadBuff();
        InitEmergency();
        mUICtrl = mUIMgr.ShowPanel("ZhiboPanel") as ZhiboUI;

        NextTurn();

    }

    public void NextTurn()
    {


        for (int i= state.Cards.Count-1;i>=0;i--)
        {
            DiscardCard(state.Cards[i], true, false);
            mUICtrl.GetCardContainer().RemoveCard(i);
        }
        ClearAllDanmu(true);
        state.TurnLeft -= 1;
        state.TurnTimeLeft = 30f;
        state.Tili = 10;
        int cardNum = (int)(state.Qifen / 100);
        for (int i = 0; i < cardNum; i++)
        {
            AddCardFromDeck();
        }
        if(state.TurnLeft <= 0)
        {
            FinishZhibo();
        }
        for(int i = state.ZhiboBuffs.Count - 1; i >= 0; i--)
        {
            state.ZhiboBuffs[i].leftTurn -= 1;
            if (state.ZhiboBuffs[i].leftTurn <= 0)
            {
                RemoveBuff(state.ZhiboBuffs[i]);
            }
        }
        CalculateBuffExtras();

        state.TurnSpecials.Clear();

        InitSuperDanmu();
        //生成
        mUICtrl.UpdateTurnLeft(state.TurnLeft);
        mUICtrl.ChangeTurnTime(state.TurnTimeLeft);
        mUICtrl.LockNextTurnBtn();
        mUICtrl.UpdateDeckLeft();

        SecCount = 0;
        SecTimer = 0;
        state.NowSuperDanmuIdx = 0;
    }


    public void ShowSuperDanmu(int idx)
    {
        if(idx<0 || idx >= state.SuperDanmus.Count)
        {
            return;
        }
        state.SuperDanmus[idx].Activated = true;

        mUICtrl.MoveSuperDanmu(state.SuperDanmus[idx]);
    }

    public List<int> PickRandomTime(int from, int to, int timeNum)
    {
        List<int> ret = new List<int>();

        float inteval = (to - from) * 1.0f / (timeNum-1);
        for(int i=0;i< timeNum; i++)
        {
            int t = from + (int)(i * inteval + (Random.value*0.8f-0.4f) * inteval);
            if (t < from)
            {
                t = from;
            }
            if (t > to)
            {
                t = to;
            }
            ret.Add(t);
        }
        return ret;
    }

    public void InitSuperDanmu()
    {
        //fix number
        for(int i=0;i< state.SuperDanmus.Count; i++)
        {
            if (state.SuperDanmus[i].HasDisapeared)
            {
                continue;
            }
            mResLoader.ReleaseGO("Zhibo/SuperDanmu", state.SuperDanmus[i].gameObject);
        }
        state.SuperDanmus.Clear();
        mUICtrl.ClearSuperDanmu();


        state.SuperDanmuShowTimeList = PickRandomTime(3,25,5);


        for(int i = 0; i < state.SuperDanmuShowTimeList.Count; i++)
        {
            SuperDanmu sDanmu = mUICtrl.ShowSuperDanmu();
            if(sDanmu != null)
            {
                state.SuperDanmus.Add(sDanmu);

            }
        }
        mUICtrl.AdjustSuperDanmuOrder();

    }

    private void LoadBuff()
    {
        BuffDesp.Add("m+", "增加{0}点魅力");
        BuffDesp.Add("t+", "增加{0}点体力");
        BuffDesp.Add("k+", "增加{0}点口才");
        BuffDesp.Add("j+", "增加{0}点技艺");
        BuffDesp.Add("f+", "增加{0}点反应");

        BuffDesp.Add("m+%", "增加百分比{0}的魅力");
        BuffDesp.Add("t+%", "增加百分比{0}的体力");
        BuffDesp.Add("k+%", "增加百分比{0}的口才");
        BuffDesp.Add("j+%", "增加百分比{0}的技艺");
        BuffDesp.Add("f+%", "增加百分比{0}的反应");
    }

    public string GetBuffDesp(string buffname)
    {
        return BuffDesp[buffname];
    }
    private void LoadCard()
    {
        List<CardInfo> infoList = mCardMdl.GetAllCards();
        state.CardDeck.Clear();
        foreach (CardInfo info in infoList)
        {
            string eid = info.CardId;
            CardAsset ca = mCardMdl.GetCardInfo(eid);
            CardInZhibo card = new CardInZhibo(eid, ca.UseTime);
            card.ca = ca;
            state.CardDeck.Add(card);
        }

        shuffle<CardInZhibo>(state.CardDeck);
    }


    public void shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int idx = Random.Range(0, list.Count - 1 - i);
            T tmp = list[idx];
            list[idx] = list[list.Count - 1 - i];
            list[list.Count - 1 - i] = tmp;
        }
    }

    private void InitEmergency()
    {

    }

    private void LoadDanmuDict()
    {
        {
            List<string> ll = new List<string>();
            ll.Add("主播什么时候开播的");
            ll.Add("日常打卡");
            ll.Add("主播晚上好啊");
            DanmuDict.Add("common", ll);
        }
    }
    public override void Tick(float dTime)
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddCardFromDeck();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            FinishZhibo();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GenSpecial("Special");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            mUIMgr.ShowHint("这是一段提示测试行");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            //ShowSuperDanmu(2);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShowEmergency();
        }






        state.AccelerateDur -= spdRate * dTime;
        if(state.AccelerateDur < 0)
        {
            state.AccelerateRate = 1f;
        }

        //if (state.Qifen > 100)
        //{
        //    int cardNum = (int)(state.Qifen / 100);
        //    state.Qifen -= cardNum * 100f;
        //    mUICtrl.ChangeTurnTime(state.Qifen);
        //    for(int i = 0; i < cardNum; i++)
        //    {
        //        AddCardFromDeck();
        //    }
        //}


        for (int i = state.Danmus.Count - 1; i >= 0; i--)
        {
            state.Danmus[i].Tick(dTime* spdRate);
            if (state.Danmus[i].NeedDestroy)
            {
                AutoDisappear(state.Danmus[i]);
            }
        }

        for (int i = state.SuperDanmus.Count - 1; i >= 0; i--)
        {
            if (!state.SuperDanmus[i].Activated || state.SuperDanmus[i].HasDisapeared)
            {
                continue;
            }
            state.SuperDanmus[i].Tick(dTime * spdRate);
            if (state.SuperDanmus[i].NeedDestroy)
            {
                DestroySuperDanmu(state.SuperDanmus[i]);
            }
        }

        for (int i = state.Specials.Count - 1; i >= 0; i--)
        {
            state.Specials[i].Tick(dTime * spdRate);
        }

        //bool buffChanged = false;
        //for (int i = state.ZhiboBuffs.Count -1; i >= 0; i--)
        //{
        //    state.ZhiboBuffs[i].Tick(dTime * spdRate);

        //    if (state.ZhiboBuffs[i].leftTime <= 0)
        //    {
        //        RemoveBuff(state.ZhiboBuffs[i]);
        //        buffChanged = true;
        //    }
        //}

        //if (buffChanged)
        //{
        //    CalculateBuffExtras();
        //}

        SecTimer += dTime * spdRate;
        if(SecTimer > 1f)
        {
            SecTimer -= 1f;
            SecCount += 1;
            if(state.NowSuperDanmuIdx< state.SuperDanmuShowTimeList.Count&& state.SuperDanmuShowTimeList[state.NowSuperDanmuIdx] == SecCount)
            {
                ShowSuperDanmu(state.NowSuperDanmuIdx);
                state.NowSuperDanmuIdx++;
            }
        }

        //for (int i = state.Cards.Count - 1; i >= 0; i--)
        //{
        //    if (state.Cards[i].ca.WillOverdue)
        //    {
        //        state.Cards[i].TimeLeft -= dTime * spdRate;
        //        if(state.Cards[i].TimeLeft <= 0)
        //        {
        //            state.Cards[i].NeedDiscard = true;
        //        }
        //    }

        //}
        //if (DiscardTimer > 1f)
        //{

        //    for (int i = state.Cards.Count - 1; i >= 0; i--)
        //    {
        //        if (state.Cards[i].NeedDiscard)
        //        {
        //            DiscardCard(state.Cards[i],true,false);
        //            mUICtrl.GetCardContainer().RemoveCard(i);
        //        }
        //        else
        //        {
        //            mUICtrl.GetCardContainer().UpdateCard(i, state.Cards[i]);
        //        }
        //    }

        //    DiscardTimer -= 1;
        //}




        lastTick += dTime * spdRate;
        if (lastTick > nextTick)
        {

            GenDanmu();
            lastTick = 0;
            nextTick = 1.0f / state.DanmuFreq;
            if (state.danmuGroups.Count == 0)
            {
                state.DanmuFreq = 3f;
            }
            else
            {
                state.DanmuFreq = 15f;
            }
            //nextTick = Random.Range(0.1f, 0.3f);
        }



        //GetChoukaValue(choukaPerSec * dTime * spdRate);

        state.TurnTimeLeft -= dTime * spdRate;
        if(state.TurnTimeLeft <= 0)
        {
            NextTurn();
        }
    }


    public void ShowEmergency()
    {
        spdRate = 0f;
        mUIMgr.ShowPanel("ActBranch");
        ActBranchCtrl actrl = mUIMgr.GetCtrl("ActBranch") as ActBranchCtrl;
        EmergencyAsset ea = mResLoader.LoadResource<EmergencyAsset>("Emergencies/choufeng");
        actrl.SetEmergency(ea);
        actrl.ActBranchEvent += delegate (int idx) {
            spdRate = 1f;
            EmergencyChoice c = ea.Choices[idx];
            Debug.Log(c.Content);
            if(c.NextEmId != null && c.NextEmId != string.Empty)
            {

            }
            if (c.Ret=="Hot")
            {

            }
        };
    }

    public void GetQifenValue(float v)
    {
        state.Qifen += v;
        mUICtrl.UpdateQifen();
        //mUIMgr.showHint("获得抽卡值" + v);
    }

    public float GetScoreFromFormulation(string formulation)
    {
        string[] comps = formulation.Split('+');
        float finalValue = 0;
        foreach(string comp in comps)
        {
            if (comp.Contains("*"))
            {
                string[] ss = comp.Split('*');
                float rate = float.Parse(ss[0]);
                string pname = ss[1];
                switch (pname)
                {
                    case "m":
                        finalValue += state.stats.meili * rate;
                        break;
                    case "k":
                        finalValue += state.stats.koucai * rate;
                        break;
                    case "t":
                        finalValue += state.stats.tili * rate;
                        break;
                    case "f":
                        finalValue += state.stats.fanying * rate;
                        break;
                    case "":
                        finalValue += state.stats.jiyi * rate;
                        break;
                    default:
                        Debug.Log("unknown property");
                        break;
                }
            }
            else
            {
                finalValue += int.Parse(comp);
            }
        }
        return finalValue;

    }

    public void GainScore(float score)
    {
        float scoreReal = score;
        if (score < 0)
        {
            float absScore = -score;
            if (state.ScoreArmor > 0)
            {
                state.ScoreArmor -= absScore;
                if(state.ScoreArmor < 0)
                {
                    state.ScoreArmor = 0;
                }
                scoreReal = 0;
            }
        }

        state.Score += scoreReal;

        //mUIMgr.ShowHint("获得热度" + (int)score);
        mUICtrl.UpdateScore(state.Score);



    }
    public void GenTili(int v)
    {
        state.Tili += v;
        mUIMgr.ShowHint("获得热度" + v);
        mUICtrl.UpdateScore(state.Score);
    }



    private void RemoveBuff(ZhiboBuff obj)
    {
        state.ZhiboBuffs.Remove(obj);
        mResLoader.ReleaseGO("Zhibo/Buff", obj.gameObject);
    }

    private void AutoDisappear(Danmu danmu)
    {
        RecycleDanmu(danmu);
        state.Danmus.Remove(danmu);
        if (danmu.isBad)
        {
            GainScore(-2);
        }
        else
        {
            GainScore(1);
        }
    }

    private void AutoDisappear(SuperDanmu danmu)
    {
        mResLoader.ReleaseGO("Zhibo/SuperDanmu",danmu.gameObject);
    }


    public void ClearAllDanmu(bool getScore)
    {
        for(int i = state.Danmus.Count - 1; i >= 0; i--)
        {
            Danmu danmu = state.Danmus[i];
            danmu.OnDestroy();
            state.Danmus.Remove(danmu);
            mUICtrl.ShowDanmuEffect(danmu.transform.position);
            if (danmu.isBad)
            {
                GainScore(-2);
            }
            else
            {
                GainScore(1);
            }
        }

        for (int i = state.SuperDanmus.Count-1; i >=0 ; i--)
        {
            if (state.SuperDanmus[i].HasDisapeared)
            {
                continue;
            }
            mResLoader.ReleaseGO("Zhibo/SuperDanmu", state.SuperDanmus[i].gameObject);
            Debug.Log("des one");
        }
        state.SuperDanmus.Clear();
        mUICtrl.ClearSuperDanmu();
    }

    public void DestroyRandomly(int num)
    {
        List<Danmu> toClean = randomPickDanmu(num);
        foreach (Danmu danmu in toClean)
        {
            danmu.OnDestroy();
            state.Danmus.Remove(danmu);
            GainScore(10);
        }
    }






    
    //IEnumerator GenMultiDanmu(int num)
    //{
    //    int nn = num;
    //    while (nn > 0)
    //    {
    //        Danmu danmu = mUICtrl.GenDanmu();
    //        state.Danmus.Add(danmu);
    //        yield return null;
    //    }
    //}


    private void RefreshUsedCards()
    {
        for (int i = state.CardUsed.Count - 1; i >= 0; i--)
        {
            state.CardDeck.Add(state.CardUsed[i]);
            state.CardUsed.RemoveAt(i);
        }
        //shuffle
        shuffle<CardInZhibo>(state.CardDeck);
    }

    public void GainNewCard(string cardId)
    {
        CardAsset ca = mCardMdl.GetCardInfo(cardId);
        if (ca == null)
        {
            return;
        }
        CardInZhibo info = new CardInZhibo(ca);


        bool ret = mUICtrl.AddNewCard(info);
        state.Cards.Add(info);
    }

    public void GainNewCardWithPossiblity(string cardId, int possibility)
    {
        int randInt = Random.Range(0, 100);
        if(randInt >= possibility)
        {
            return;
        }
        GainNewCard(cardId);
    }

    public void AddCardFromDeck()
    {

        if(state.CardDeck.Count == 0)
        {
            RefreshUsedCards();
        }
        if(state.CardDeck.Count == 0)
        {
            return;
        }

        if(state.Cards.Count >= CardMax)
        {
            return;
        }
        CardInZhibo info = state.CardDeck[0];
        bool ret = mUICtrl.AddNewCard(info);
        if (!ret)
        {
            Debug.Log("add card Fail");
            return;
        }

        state.CardDeck.RemoveAt(0);
        state.Cards.Add(info);

        if(state.CardDeck.Count == 0)
        {
            RefreshUsedCards();
        }
        mUICtrl.UpdateDeckLeft();
        //mUICtrl.GetCardContainer().UpdateCard(state.Cards.Count-1,info);
    }

    public void RecycleDanmu(Danmu danmu)
    {
        mResLoader.ReleaseGO("Zhibo/Danmu", danmu.gameObject);
    }

    public string getRandomDanmu()
    {
        return "你麻痹死了";
    }

    public void FinishZhibo()
    {
        ZhiboJiesuanUI p = mUIMgr.ShowPanel("ZhiboJiesuanPanel") as ZhiboJiesuanUI;
        spdRate = 0;
        if(true||state.Score > state.MaxScore)
        {
            pRoleMgr.AddFensi(0,100);
            pRoleMgr.GetMoney(100);
            //根据打过的卡牌 增加主属性 和 经验值
            int[] bonus = new int[5];
            for(int i = 0; i < state.UsedCardsToGetBonus.Count; i++)
            {
                string cid = state.UsedCardsToGetBonus[i];
                CardAsset ca = mCardMdl.GetCardInfo(cid);
                if(ca == null)
                {
                    continue;
                }
                if(ca.StatusBonusType > 0)
                {
                    bonus[ca.StatusBonusType - 1] += ca.StatusBonusNum;
                }
                if(ca.SkillBonusType > 0)
                {
                    //
                }

            }
            string bonusString = "";
            for(int i = 0; i < 5; i++)
            {
                if (bonus[i] > 0)
                {
                    bonusString += "p" + i + " add" + bonus[i] + "\n";
                }
            }
            p.SetContent(bonusString);
        }
    }



    private void DiscardCard(CardInZhibo cinfo, bool useOnDiscard, bool costUseTime)
    {

        if (useOnDiscard && cinfo.ca.UseOnDiscard)
        {
            ExcuteUseCard(cinfo);
            costUseTime = true;
        }

        state.Cards.Remove(cinfo);
        if (costUseTime && cinfo.UseLeft  > 0)
        {
            cinfo.UseLeft -= 1;
            if (cinfo.UseLeft == 0)
            {
                return;
            }
        }

        state.CardUsed.Add(cinfo);
        cinfo.TimeLeft = 0;
        cinfo.NeedDiscard = false;
    }

    public bool TryUseCard(int cardIdx)
    {
        if(cardIdx < 0|| cardIdx >= state.Cards.Count)
        {
            return false;
        }
        CardInZhibo cinfo = state.Cards[cardIdx];

        CardAsset ca = cinfo.ca;
        if (state.Tili < ca.cost)
        {
            mUIMgr.ShowHint("体力不足");
            return false;
        }
        state.Tili -= ca.cost;
        mUICtrl.UpdateTili();
        ExcuteUseCard(cinfo);
        DiscardCard(cinfo,false,true);
        return true;
    }


    private void GenSpeedUp(float duration = 5f)
    {
        state.AccelerateRate = 2f;
        state.AccelerateDur = (state.AccelerateDur < 0 ? 0 : state.AccelerateDur) + duration;
    }

    private CardInZhibo NowExecuteCard;

    public void ExcuteUseCard(CardInZhibo card)
    {

        CardAsset cardAsset = card.ca;
        if (cardAsset != null)
        {
            NowExecuteCard = card;
            if (cardAsset.CardType == eCardType.GENG)
            {
                mUICtrl.ShowGengEffect();
            }

            //几率触发或条件触发的效果 将放入该列表中后处理
            List<CardEffect> extraEffects = new List<CardEffect>();

            foreach(CardEffect ce in cardAsset.Effects)
            {
                HandleOneCardEffect(ce, extraEffects);
            }

            for(int i=0;i< extraEffects.Count;i++)
            {
                HandleOneCardEffect(extraEffects[i], extraEffects);
            }
            NowExecuteCard = null;

            if(cardAsset.StatusBonusType != 0 || cardAsset.SkillBonusType != 0)
            {
                state.UsedCardsToGetBonus.Add(cardAsset.CardId);
            }

        }
    }

    public void AddArmor(float armor)
    {
        state.ScoreArmor += armor;
    }

    private void HandleOneCardEffect(CardEffect ce, List<CardEffect> extraEffects)
    {
        string[] args = ce.effectString.Split(',');
        switch (ce.effect)
        {
            case "SpawnGift":
                for (int i = 0; i < 3; i++)
                {
                    GenSpecial(args[0]);
                }
                break;
            case "SpeedUp":
                GenSpeedUp(float.Parse(args[0]));
                break;
            case "GenGoodDanmu":
                AddDanmuGroup(args[0],20);
                break;
            case "GenBadDanmu":
                AddDanmuGroup(args[0],20);
                break;
            case "GetScore":

                GainScore(GetScoreFromFormulation(args[0]));
                mUICtrl.ShowNewAudience();


                
                mUICtrl.ShowDanmuEffect(mUICtrl.GetCardContainer().cards[state.Cards.IndexOf(NowExecuteCard)].transform.position);
                break;
            case "GetChouka":
                GetQifenValue(int.Parse(args[0]));
                break;
            case "GetTili":
                GenTili(int.Parse(args[0]));
                break;
            case "AddStatus":
                GenBuff(args[0], int.Parse(args[1]), 2);
                break;
            case "AddRemoveAward":

                GenBuff(args[0], int.Parse(args[1]), 2);
                break;
            case "ClearDanmu":

                DestroyRandomly(int.Parse(args[0]));
                break;
            case "AddCardToDeck":
                AddCardToDeck(args[0], int.Parse(args[1]));
                break;
            case "Chongzhu":
                AddCardFromDeck();
                break;
            case "Armor":
                AddArmor(int.Parse(args[0]));
                break;
            case "GainCardWithPossibility":
                GainNewCardWithPossiblity(args[0], int.Parse(args[1]));
                break;
            case "Branches":
                int randNum = Random.Range(0, 100);
                int baseNum = 0;
                string[] cmds = ce.effectString.Split(';');
                foreach (string cmd in cmds)
                {
                    if (cmd == "")
                    {
                        continue;
                    }
                    string s = cmd;
                    int p = int.Parse(s.Substring(0, s.IndexOf(',')));
                    if (randNum <= baseNum + p)
                    {
                        //生效
                        s = s.Substring(s.IndexOf(',') + 1);
                        string effect = s.Substring(0, s.IndexOf(','));
                        string effectString = s.Substring(s.IndexOf(',') + 1);
                        extraEffects.Add(new CardEffect(effect, effectString));
                        break;
                    }
                    else
                    {
                        baseNum += p;
                    }

                }

                break;
            default:
                break;
        }

    }

    private void AddCardToDeck(string cardId, int level)
    {

        string eid = cardId;
        CardAsset ca = mCardMdl.GetCardInfo(eid);
        CardInZhibo card = new CardInZhibo(eid, ca.UseTime);
        card.ca = ca;
        state.CardDeck.Add(card);
    }

    private List<Danmu> randomPickDanmu(int n)
    {
        if (state.Danmus.Count <= n)
        {
            return new List<Danmu>(state.Danmus);
        }
        List<Danmu> ret = new List<Danmu>();
        List<int> choosed = new List<int>();
        int nowC = 0;
        while (nowC < n)
        {
            int randIdx = Random.Range(0, state.Danmus.Count);
            if (!choosed.Contains(randIdx))
            {
                choosed.Add(randIdx);
                nowC++;
            }
        }
        foreach (int idx in choosed)
        {
            ret.Add(state.Danmus[idx]);
        }
        return ret;
    }


    public void AddDanmuGroup(string key, int num = 50)
    {
        state.danmuGroups.Add(new DanmuGroup(key,num,(int)(num*0.1f)));
    }

    public void GenBuff(string BuffId, int value, int duration)
    {

        ZhiboBuff buff = mUICtrl.GenBuff();
        buff.Init(BuffId, value, duration, this);
        state.ZhiboBuffs.Add(buff);
        CalculateBuffExtras();
    }

    private void CalculateBuffExtras()
    {
        for(int i = 0; i < 5; i++)
        {
            state.BuffAddValue[i] = 0;
            state.BuffAddPercent[i] = 0;
        }
        foreach (ZhiboBuff buff in state.ZhiboBuffs)
        {
            switch (buff.buffId)
            {
                case "m+":
                    state.BuffAddValue[0] += buff.buffLevel;
                    break;
                case "m+%":
                    state.BuffAddPercent[0] += buff.buffLevel;
                    break;
                case "t+":
                    state.BuffAddValue[1] += buff.buffLevel;
                    break;
                case "t+%":
                    state.BuffAddPercent[1] += buff.buffLevel;
                    break;
                case "k+":
                    state.BuffAddValue[2] += buff.buffLevel;
                    break;
                case "k+%":
                    state.BuffAddPercent[2] += buff.buffLevel;
                    break;
                case "f+":
                    state.BuffAddValue[3] += buff.buffLevel;
                    break;
                case "f+%":
                    state.BuffAddPercent[3] += buff.buffLevel;
                    break;
                case "j+":
                    state.BuffAddValue[4] += buff.buffLevel;
                    break;
                case "j+%":
                    state.BuffAddPercent[4] += buff.buffLevel;
                    break;
                default:
                    break;
            }
        }
    }


    public void GenSpecial(string specialType)
    {
        ZhiboSpecial spe = mUICtrl.GenSpecial(specialType);
        state.Specials.Add(spe);
    }


    float pNow = 0;
    public void GenDanmu()
    {

        DanmuGroup dd = null;
        if (state.danmuGroups.Count > 0)
        {
            int[] preSum = new int[state.danmuGroups.Count];
            preSum[0] = state.danmuGroups[0].TotalNum;
            for (int i = 1; i < state.danmuGroups.Count; i++)
            {
                preSum[i] = preSum[i - 1] + state.danmuGroups[i].TotalNum;
            }
            int randInt = Random.Range(0,preSum[state.danmuGroups.Count-1]);
            int choose = 0;
            for(int i=0; i < preSum.Length; i++)
            {
                if(randInt < preSum[i])
                {
                    choose = i;
                    dd = state.danmuGroups[i];
                    break;
                }
            }
            state.danmuGroups[choose].TotalNum -= 1;
            if(state.danmuGroups[choose].TotalNum <= 0)
            {
                state.danmuGroups.RemoveAt(choose);
            }
        }

        //固定10几率刷新
        bool bad = false;
        int rand = Random.Range(0, 100);
        if(rand < pNow*100)
        {
            bad = true;
            pNow = 0.03321f;
        }
        else
        {
            pNow += 0.03321f;
            if (pNow > 1)
            {
                pNow = 1;
            }
        }
        //if(dd != null)
        //{
        //    badLine = dd.badRate;
        //}
        //int rand = Random.Range(0, 100);
        //if (rand < badLine)
        //{
        //    bad = true;
        //}


        Danmu danmu = mUICtrl.GenDanmu(bad);
        bigOneCount++;
        if (bigOneCount > bigOneNext)
        {
            danmu.SetAsBig();
            bigOneCount = 0;
            bigOneNext = Random.Range(5, 8);
        }
        state.Danmus.Add(danmu);
    }





    public void HitSpecial(ZhiboSpecial spe)
    {
        if (spe.type == "gift")
        {
            state.Score += 100;
            mUICtrl.UpdateScore(state.Score);
        }
        state.Specials.Remove(spe);
        mResLoader.ReleaseGO("Zhibo/Special/Special" , spe.gameObject);
    }


    public override void OnRelease()
    {
        base.OnRelease();
        if (GameFinishedCallback != null)
        {
            GameFinishedCallback();
        }
    }

    public void DestroyDanmu(Danmu danmu)
    {
        if (!danmu.isBad)
        {
            GainScore(danmu.isBig ? 3 : 1);
            danmu.view.Content.color = Color.gray;
            danmu.view.Hengfu.raycastTarget = false;
            mUICtrl.ShowDanmuEffect(danmu.transform.position);
        }
        else
        {
            danmu.OnDestroy();
            state.Danmus.Remove(danmu);
        }
    }

    public void DestroySuperDanmu(SuperDanmu danmu)
    {
        //if (!danmu.isBad)
        //{
        //    GainScore(danmu.isBig ? 3 : 1);
        //    danmu.view.textField.color = Color.gray;
        //    danmu.view.textField.raycastTarget = false;
        //    mUICtrl.ShowDanmuEffect(danmu.transform.position);
        //}
        //else
        //{
        //    danmu.OnDestroy();
        //    state.Danmus.Remove(danmu);
        //}
        AutoDisappear(danmu);
    }
}
