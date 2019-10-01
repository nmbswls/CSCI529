using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eReactorType
{
    PRESENT,
    CAUTION,
    TUHAO
}

public class ZhiboBuff
{

}

public class ZhiboGameMode : GameModeBase
{

    public List<ZhiboBuff> zhiboBuffs = new List<ZhiboBuff>();

    public List<Danmu> danmus = new List<Danmu>();

    public List<CardInfo> Cards = new List<CardInfo>();
    
    public string info;
    public float spdRate = 1.0f;

    IUIMgr mUIMgr;
    IResLoader mResLoader;
    ICardDeckModule mCardMdl;

    public ZhiboUI mUICtrl;

    public int hot = 0;
    public int maxHot = 100;

    public int xianchangzhi = 0;

    public float lastTick = 0;
    public float nextTick = 0;

    private int bigOneNext = 3;
    private int bigOneCount = 0;


    public override void Init()
    {
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();

        Cards = mCardMdl.GetAllCards();

        mUIMgr.ShowPanel("ZhiboPanel");
        mUICtrl = mUIMgr.GetCtrl("ZhiboPanel") as ZhiboUI;

        zhiboBuffs.Clear();
        Cards.Clear();
        danmus.Clear();

        hot = 0;
        xianchangzhi = 0;

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

        if (xianchangzhi > 100)
        {

            xianchangzhi = 0;
            mUICtrl.ChangeXianChange(xianchangzhi + "");
            AddNewCard();
        }

        for (int i = danmus.Count - 1; i >= 0; i--)
        {
            danmus[i].Tick(dTime* spdRate);
            if (danmus[i].NeedDestroy)
            {
                AutoDisappear(danmus[i]);

            }
        }

        lastTick += dTime * spdRate;
        if (lastTick > nextTick)
        {
            GenDanmu();
            lastTick = 0;
            nextTick = Random.Range(0.1f, 0.3f);
        }


    }

    private void AutoDisappear(Danmu danmu)
    {
        RecycleDanmu(danmu);
        danmus.Remove(danmu);

        xianchangzhi += 3;
        mUICtrl.ChangeXianChange(xianchangzhi + "");
    }

    public void useSpecial()
    {
        List<Danmu> toClean = randomPickDanmu(10);
        foreach (Danmu danmu in toClean)
        {
            danmu.OnDestroy();
            danmus.Remove(danmu);
            gainHot(10);
        }

    }

    public void gainHot(int v)
    {
        hot += v;

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
        danmus.Add(danmu);
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
        GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main");
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
        }
    }

    private void GenReactorObj(eReactorType type)
    {

    }

    private List<Danmu> randomPickDanmu(int n)
    {
        if (danmus.Count <= n)
        {
            return new List<Danmu>(danmus);
        }
        List<Danmu> ret = new List<Danmu>();
        List<int> choosed = new List<int>();
        int nowC = 0;
        while (nowC < n)
        {
            int randIdx = Random.Range(0, danmus.Count);
            if (!choosed.Contains(randIdx))
            {
                choosed.Add(randIdx);
                nowC++;
            }
        }
        foreach (int idx in choosed)
        {
            ret.Add(danmus[idx]);
        }
        return ret;
    }


    public void GenEnegy(int v)
    {

    }

    public void GenBuff()
    {

    }

}
