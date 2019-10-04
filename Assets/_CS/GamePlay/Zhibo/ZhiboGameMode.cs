using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eReactorType
{
    PRESENT,
    CAUTION,
    TUHAO
}







public class ZhiboGameState
{
    public List<ZhiboBuff> ZhiboBuffs = new List<ZhiboBuff>();

    public List<Danmu> Danmus = new List<Danmu>();

    public List<CardInfo> Cards = new List<CardInfo>();

    public List<ZhiboSpecial> Specials = new List<ZhiboSpecial>();

    public int Score = 0;
    public int MaxHot = 100;

    public int ChoukaValue = 0;
    public int ChoukaYuzhi = 100;
    public int Tili = 0;

    public float DanmuFreq { get { return danmuFreq * AccelerateRate; } }
    private float danmuFreq = 5f;

    public float DanmuSpd { get { return danmuSpd * AccelerateRate; } }

    private float danmuSpd = 160.0f;

    public float AccelerateRate = 1.0f;
    public float AccelerateDur = 1.0f;
}
public class ZhiboGameMode : GameModeBase
{


    IUIMgr mUIMgr;
    IResLoader mResLoader;
    ICardDeckModule mCardMdl;
    public ZhiboUI mUICtrl;

    public ZhiboGameState state;

    public string info;
    public float spdRate = 1.0f;


    public float lastTick = 0;
    public float nextTick = 0;

    private int bigOneNext = 3;
    private int bigOneCount = 0;


    public override void Init()
    {
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();

        state = new ZhiboGameState();

        state.Cards = mCardMdl.GetAllCards();

        mUIMgr.ShowPanel("ZhiboPanel");
        mUICtrl = mUIMgr.GetCtrl("ZhiboPanel") as ZhiboUI;

        state.ZhiboBuffs.Clear();
        state.Cards.Clear();
        state.Danmus.Clear();

        state.Score = 0;
        state.ChoukaValue = 0;
        state.Tili = 0;

        spdRate = 1.0f;
        lastTick = 0;
        nextTick = 0;
        bigOneNext = 3;
        bigOneCount = 0;
    }

    public override void Tick(float dTime)
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddNewCard();
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
            mUIMgr.showHint("这是一段提示测试行");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenBuff("e");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            spdRate = 0.1f;
            mUIMgr.ShowPanel("ActBranch");
            ActBranchCtrl actrl = mUIMgr.GetCtrl("ActBranch") as ActBranchCtrl;
            actrl.ActBranchEvent += delegate(int idx) {
                spdRate = 1f;
                Debug.Log(idx);
            };
        }



        if (state.ChoukaValue > 100)
        {
            state.ChoukaValue = 0;
            mUICtrl.ChangeXianChange(state.ChoukaValue + "");
            AddNewCard();
        }


        for (int i = state.Danmus.Count - 1; i >= 0; i--)
        {
            state.Danmus[i].Tick(dTime* spdRate);
            if (state.Danmus[i].NeedDestroy)
            {
                AutoDisappear(state.Danmus[i]);

            }
        }

        for (int i = state.Specials.Count - 1; i >= 0; i--)
        {
            state.Specials[i].Tick(dTime * spdRate);
        }

        bool changed = false;
        for (int i = state.ZhiboBuffs.Count -1; i >= 0; i--)
        {
            state.ZhiboBuffs[i].Tick(dTime * spdRate);

            if (state.ZhiboBuffs[i].leftTime <= 0)
            {
                RemoveBuff(state.ZhiboBuffs[i]);
                changed = true;
            }
        }

        if (changed)
        {
            CalculateBuffEff();
        }


        lastTick += dTime * spdRate;
        if (lastTick > nextTick)
        {
            GenDanmu();
            lastTick = 0;
            nextTick = 1.0f / state.DanmuFreq * Random.Range(0.7f, 1.3f);
            nextTick = Random.Range(0.1f, 0.3f);
        }




    }

    private void CalculateBuffEff()
    {
        int tiliAdd = 0;
        int tiliAddp = 0;
        foreach(ZhiboBuff buff in state.ZhiboBuffs)
        {
            if(buff.buffId == "tili")
            {
                tiliAdd += 10;
            }
        }

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
        GetChoukaValue(3);
    }

    public void GetChoukaValue(int v)
    {
        state.ChoukaValue += v;
        mUICtrl.ChangeXianChange(state.ChoukaValue + "");
    }



    public void DestroyRandomly(int num)
    {
        List<Danmu> toClean = randomPickDanmu(num);
        foreach (Danmu danmu in toClean)
        {
            danmu.OnDestroy();
            state.Danmus.Remove(danmu);
            GainHot(10);
        }
    }

    public void GainHot(int v)
    {
        state.Score += v;

    }




    
    IEnumerator GenMultiDanmu(int num)
    {
        int nn = num;
        while (nn > 0)
        {
            Danmu danmu = mUICtrl.GenDanmu();
            state.Danmus.Add(danmu);
            yield return null;
        }
    }


    public void AddNewCard()
    {
        mUICtrl.AddNewCard();
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
        mUIMgr.CloseCertainPanel(mUICtrl);
        GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main",delegate {
        

        });
    }

    public void UseCard(CardInfo card)
    {
        CardAsset cardAsset = mCardMdl.GetCardInfo(card.CardId);
        if (cardAsset != null)
        {
            if(cardAsset.CardType == eCardType.GENG)
            {
                mUICtrl.ShowGengEffect();
            }

            foreach(CardEffect ce in cardAsset.effects)
            {
                switch (ce.effect)
                {
                    case  "SpawnGift":
                        GenSpecial(ce.x);
                        break;
                    case "SpeedUp":
                        state.AccelerateRate = 1.3f;
                        state.AccelerateDur = 5f;
                        break;
                    case "GenGoodDanmu":
                        GameMain gm = (GameMain)GameMain.GetInstance();
                        gm.StartCoroutine(GenMultiDanmu(10));
                        break;
                    case "GetScore":
                        GainHot(100);
                        break;
                    case "GetChouka":
                        GetChoukaValue(10);
                        break;
                    case "GetTili":
                        GenEnegy(10);
                        break;
                    case "AddStatus":
                        GenBuff("x");
                        break;
                    case "AddRemoveAward":
                        GenBuff("x");
                        break;
                    case "ClearDanmu":
                        DestroyRandomly(5);
                        break;
                    default:
                        break;
                }
            }

        }
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


    public void GenEnegy(int v)
    {

    }

    public void GenBuff(string BuffId)
    {

        ZhiboBuff buff = mUICtrl.GenBuff();
        state.ZhiboBuffs.Add(buff);

    }

    public void GenSpecial(string specialType)
    {
        ZhiboSpecial spe = mUICtrl.GenSpecial("Special");
        state.Specials.Add(spe);
    }


    public void GenDanmu()
    {
        Danmu danmu = mUICtrl.GenDanmu();
        bigOneCount++;
        if (bigOneCount > bigOneNext)
        {
            danmu.view.textField.fontSize = 38;
            bigOneCount = 0;
            bigOneNext = Random.Range(4, 8);
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

}
