using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eFightDanmuSkillEffectType
{
    AddHp,
    GetArmor,
    GetScore,
    ClearDanmu,
    GetHot,
}
public class FightDanmuSkillEffect
{
    public eFightDanmuSkillEffectType type;
    public string effectString;
}

public class ZhiboMode2Skill
{
    public FightDanmuSkillEffect Se;
    public string Name;
    public float Cd;
    public float CdLeft;
    public string PictureUrl;
    public int EnegyCost;
    public string Desp;

    public ZhiboMode2Skill()
    {

    }

    public ZhiboMode2Skill(ZhiboMode2Skill other)
    {
        this.Se = new FightDanmuSkillEffect();
        this.Se.type = other.Se.type;
        this.Se.effectString = other.Se.effectString;
        this.Name = other.Name;
        this.Cd = other.Cd;
        this.CdLeft = 0f;
        this.EnegyCost = other.EnegyCost;
        this.Desp = other.Desp;
        this.PictureUrl = other.PictureUrl;
    }
}

public class ZhiboGameMode2State
{
    public float AccelerateRate = 1.0f;
    public float AccelerateDur = 0f;


    public float savedFreq;
    public float DanmuFreq { get { return danmuFreq /* * AccelerateRate*/; } set { danmuFreq = value; } }
    private float danmuFreq = 3f;

    public float DanmuSpd { get { return danmuSpd * AccelerateRate; } }
    private float danmuSpd = 160.0f;

    //public List<DanmuGroup> danmuGroups = new List<DanmuGroup>();

    public List<DanmuMode2> Danmus = new List<DanmuMode2>();
    public List<SuperDanmuMode2> SuperDanmus = new List<SuperDanmuMode2>();

    public List<KeyValuePair<int,int>> SuperDanmuShowTimeList = new List<KeyValuePair<int, int>>();
    public int NowSuperDanmuIdx;

    public int Hp;
    public int MaxHp;


    public float Score;
    public int TargetScore;

    public float Enegy;
    public float MaxEnegy;
    public float EnegyPerSec;

    public float OriginTime;
    public float TimeLeft;

    public string NowDanmuJiezou = string.Empty;
    public int BadNum = 0;

    public List<ZhiboMode2Skill> PresetActions = new List<ZhiboMode2Skill>();
    public ZhiboMode2Skill Passive;

    public float ArmorTimer;
}


public class FightDanmuGMInitData : GameModeInitData
{
    public List<string> SkillList;
}

public class FightingDanmuGameMode : GameModeBase
{
    public static int ActionNum = 4;

    IUIMgr mUIMgr;
    IResLoader mResLoader;
    IRoleModule pRoleMgr;
    ICardDeckModule mCardMdl;
    //ISkillTreeMgr mSkillMdl;

    public ZhiboGameMode2State state;

    public ZhiboMode2UICtrl mUICtrl;

    public float spdRate = 1.0f;

    public float lastTick = 0;
    public float nextTick = 0;

    private int bigOneNext = 3;
    private int bigOneCount = 0;

    private float SecTimer = 0;
    private int SecCount = 0;



    private Dictionary<string, List<string>> DanmuDict = new Dictionary<string, List<string>>();
    public override void Init(GameModeInitData initData)
    {
        fakeZhiboMode2Skill();





        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        //mSkillMdl = GameMain.GetInstance().GetModule<SkillTreeMgr>();

        state = new ZhiboGameMode2State();

        state.Hp = 100;
        state.MaxHp = 100;
        state.Enegy = 0;
        state.MaxEnegy = 100;
        state.EnegyPerSec = 1f;

        state.OriginTime = 100;
        state.TimeLeft = 100;
        state.Score = 0;
        state.TargetScore = 1000;

        state.DanmuFreq = 3f;
        state.savedFreq = state.DanmuFreq;

        state.NowSuperDanmuIdx = 0;

        state.ArmorTimer = 0f;

        spdRate = 1.0f;
        lastTick = 0;
        nextTick = 0;
        bigOneNext = 3;
        bigOneCount = 0;



        LoadDanmuDict();

        mUICtrl = mUIMgr.ShowPanel("ZhiboPanelMode2") as ZhiboMode2UICtrl;

        mUICtrl.UpdateTargetScore();

        InitSuperDanmu();


        FightDanmuGMInitData realData = initData as FightDanmuGMInitData;
        if(realData != null)
        {
            SetPresetInfo(realData.SkillList);
        }

    }

    public void SetPresetInfo(List<string> skillList,string passive="4")
    {
        
        for (int i = 0; i < skillList.Count; i++)
        {
            if (ZhiboMode2SkillDict.ContainsKey(skillList[i]))
            {
                ZhiboMode2Skill copied = new ZhiboMode2Skill(ZhiboMode2SkillDict[skillList[i]]);
                state.PresetActions.Add(copied);
            }
            else
            {
                state.PresetActions.Add(null);
            }
        }
        {
            if (ZhiboMode2SkillDict.ContainsKey(passive))
            {
                ZhiboMode2Skill copied = new ZhiboMode2Skill(ZhiboMode2SkillDict[passive]);
                state.Passive = copied;
            }
        }
        mUICtrl.UpdateActions();
        mUICtrl.UpdateActionCd();
    }

    private void LoadDanmuDict()
    {
        {
            List<string> ll = new List<string>();
            ll.Add("主播什么时候开播的");
            ll.Add("日常打卡");
            ll.Add("主播晚上好啊");
            DanmuDict.Add(string.Empty, ll);
        }
        {
            List<string> ll = new List<string>();
            ll.Add("主播什么时候开播的");
            ll.Add("日常打卡");
            ll.Add("主播晚上好啊");
            //ll.Add("Negative comments");
            DanmuDict.Add("1", ll);
        }
        {
            List<string> ll = new List<string>();
            ll.Add("主播什么时候开播的");
            ll.Add("日常打卡");
            ll.Add("主播晚上好啊");
            //ll.Add("Negative comments");
            DanmuDict.Add("2", ll);
        }
    }


    public override void Tick(float dTime)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FinishZhibo();
        }

        if (state.AccelerateDur > 0)
        {
            state.AccelerateDur -= spdRate * dTime;
            if (state.AccelerateDur < 0)
            {
                state.AccelerateRate = 1f;
            }
        }

        if(state.ArmorTimer > 0)
        {
            state.ArmorTimer -= spdRate * dTime;
            if(state.ArmorTimer < 0)
            {
                mUICtrl.UpdateShieldView();
            }
            mUICtrl.UpdateShieldTimer();
        }






        for (int i = state.Danmus.Count - 1; i >= 0; i--)
        {
            state.Danmus[i].Tick(dTime * spdRate);
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
                AddHp(-10);
                //super danmu 起效
                //if (state.SuperDanmus[i].Type == eSuperDanmuType.Jianpanxia)
                //{
                //    AddHp(-4);
                //}
                state.SuperDanmus[i].Disappear();
            }
        }


        for (int i = 0; i < state.PresetActions.Count; i++)
        {
            if (state.PresetActions[i].CdLeft > 0)
            {
                state.PresetActions[i].CdLeft -= dTime * spdRate;
            }
        }
        mUICtrl.UpdateActionCd();

        SecTimer += dTime * spdRate;
        if (SecTimer > 1f)
        {
            state.TimeLeft -= 1f;
            SecTimer -= 1f;
            SecCount += 1;

            if (state.NowSuperDanmuIdx < state.SuperDanmuShowTimeList.Count && state.SuperDanmuShowTimeList[state.NowSuperDanmuIdx].Key == SecCount)
            {
                ShowSuperDanmu(state.NowSuperDanmuIdx);
                state.NowSuperDanmuIdx++;
            }


            //mBuffManager.TickSec();
            if (state.TimeLeft <= 0)
            {
                FinishZhibo();
            }
            mUICtrl.UpdateTimeLeft();
            mUICtrl.UpdateActionCd();

            mUICtrl.UpdateSkillDetailCD();

            state.savedFreq = SecCount / 20 + 3;
        }

        GainEnegy(dTime * spdRate * state.EnegyPerSec);

        lastTick += dTime * spdRate;
        if (lastTick > nextTick)
        {


            if (state.BadNum == 0)
            {
                state.DanmuFreq = state.savedFreq;
                state.NowDanmuJiezou = string.Empty;
            }
            else
            {
                state.DanmuFreq = state.savedFreq+5;
            }
            GenDanmu();
            lastTick -= nextTick;
            nextTick = 1.0f / state.DanmuFreq;
        }

    }

    public void GainEnegy(float amount)
    {
        state.Enegy += amount;
        if(state.Enegy > state.MaxEnegy)
        {
            state.Enegy = state.MaxEnegy;
        }
        mUICtrl.UpdateEnegy();
    }

    public void FinishZhibo()
    {
        FightDanmuJiesuanUI p = mUIMgr.ShowPanel("FightDanmuJiesuanPanel", true,false) as FightDanmuJiesuanUI;
        spdRate = 0;
    }

    public void GenDanmu()
    {


        //DanmuGroup dd = null;
        //if (state.danmuGroups.Count > 0)
        //{
        //    int[] preSum = new int[state.danmuGroups.Count];
        //    preSum[0] = state.danmuGroups[0].TotalNum;
        //    for (int i = 1; i < state.danmuGroups.Count; i++)
        //    {
        //        preSum[i] = preSum[i - 1] + state.danmuGroups[i].TotalNum;
        //    }
        //    int randInt = Random.Range(0, preSum[state.danmuGroups.Count - 1]);
        //    int choose = 0;
        //    for (int i = 0; i < preSum.Length; i++)
        //    {
        //        if (randInt < preSum[i])
        //        {
        //            choose = i;
        //            dd = state.danmuGroups[i];
        //            break;
        //        }
        //    }
        //    state.danmuGroups[choose].TotalNum -= 1;
        //    if (state.danmuGroups[choose].TotalNum <= 0)
        //    {
        //        state.danmuGroups.RemoveAt(choose);
        //    }
        //}


        bool bad = true;
        if (state.BadNum > 0)
        {
            bad = true;
            state.BadNum -= 1;
        }
        else
        {
            if (Random.value < 0.5)
            {
                bad = false;
            }
        }

        DanmuMode2 danmu = mUICtrl.GenDanmu(bad);
        bigOneCount++;
        if (bigOneCount > bigOneNext)
        {
            danmu.SetAsSpecial();
            bigOneCount = 0;
            bigOneNext = Random.Range(5, 8);
        }
        state.Danmus.Add(danmu);
    }

    public void UseAbility(int idx)
    {
        if(idx < 0 || idx >= ActionNum)
        {
            return;
        }
        if(state.PresetActions[idx] == null)
        {
            return;
        }

        if(state.PresetActions[idx].EnegyCost > state.Enegy)
        {
            return;
        }
        UseEnegy(state.PresetActions[idx].EnegyCost);

        HandleEffect(state.PresetActions[idx].Se);

        state.PresetActions[idx].CdLeft = state.PresetActions[idx].Cd;
    }

    public void GetScore(float amount)
    {
        float amountReal = amount;
        if (amount < 0)
        {

        }
        else
        {
            amountReal *= 1.0f;
        }

        state.Score += amountReal;
        if(state.Score < 0)
        {
            state.Score = 0;
        }
        mUICtrl.UpdateScore();
    }

    public void SetArmor(float time)
    {
        state.ArmorTimer += time;
        mUICtrl.UpdateShieldView();
    }

    public void SlowDown(float time)
    {
        state.AccelerateDur += time;
        state.AccelerateRate = 0.75f;
    }

    public void Damaged(int amount)
    {
        if(state.ArmorTimer > 0)
        {
            return;
        }
        state.Hp -= amount;
        if(state.Hp > state.MaxHp)
        {
            state.Hp = state.MaxHp;
        }
        if (state.Hp <= 0)
        {
            FinishZhibo();
        }
        mUICtrl.UpdateHp();
    }

    public void AddHp(int amount)
    {
        state.Hp -= amount;
        if (state.Hp > state.MaxHp)
        {
            state.Hp = state.MaxHp;
        }
        mUICtrl.UpdateHp();
    }

    public void UseEnegy(float amount)
    {
        state.Enegy -= amount;
        if(state.Enegy < 0)
        {
            state.Enegy = 0;
        }
        mUICtrl.UpdateEnegy();
    }

    public void RecycleDanmu(DanmuMode2 danmu)
    {
        mResLoader.ReleaseGO("ZhiboMode2/Danmu2", danmu.gameObject);
    }

    private void AutoDisappear(DanmuMode2 danmu)
    {

        if (danmu.isBad)
        {
            Damaged(3);
        }
        RecycleDanmu(danmu);
        state.Danmus.Remove(danmu);
    }


    public void DanmuClicked(DanmuMode2 danmu)
    {
        if (danmu.isBad)
        {
            if (danmu.isBig)
            {
                GainEnegy(1);
                GetScore(3);
                mUICtrl.ShowDamageAmountEffect(danmu.transform.position, 3);
            }
            else
            {
                GainEnegy(2);
                GetScore(5);
                mUICtrl.ShowDamageAmountEffect(danmu.transform.position, 3);
            }
        }
        else
        {
            GetScore(-1);
            mUICtrl.ShowDamageAmountEffect(danmu.transform.position, -1);
        }

        danmu.OnDestroy();
        state.Danmus.Remove(danmu);
    }

    public string GetDanmuContent()
    {
        if (DanmuDict.ContainsKey(state.NowDanmuJiezou))
        {
            List<string> ll = DanmuDict[state.NowDanmuJiezou];
            int idx = Random.Range(0, ll.Count);
            return ll[idx];
        }
        return "主播长成这样也敢直播？";
    }

    public string GetHeiDanmuContent()
    {
        if (DanmuDict.ContainsKey("1"))
        {
            Debug.Log(state.NowDanmuJiezou);
            List<string> ll = DanmuDict["1"];
            int idx = Random.Range(0, ll.Count);
            return ll[idx];
        }
        return "Look yourself ***!";
    }

    public void RandomChangeJiezou()
    {
        List<string> keys = new List<string>(DanmuDict.Keys);
        int idx = Random.Range(0, keys.Count);
        state.NowDanmuJiezou = keys[idx];
    }


    public void HandleEffect(FightDanmuSkillEffect ce)
    {
        string[] args = ce.effectString.Split(',');
        switch (ce.type)
        {
            case eFightDanmuSkillEffectType.AddHp:
                AddHp(int.Parse(args[0]));
                break;
            case eFightDanmuSkillEffectType.GetArmor:
                SetArmor(int.Parse(args[0]));
                break;
            case eFightDanmuSkillEffectType.GetScore:
                GetScore(int.Parse(args[0]));
                AddBadDanmu(20);
                break;
            case eFightDanmuSkillEffectType.ClearDanmu:
                DestroyBadRandomly(int.Parse(args[0]));
                break;
            case eFightDanmuSkillEffectType.GetHot:
                SlowDown(int.Parse(args[0]));
                break;
            default:
                break;
        }
    }



    public void AddBadDanmu(int amount)
    {
        state.BadNum += amount;
    }

    public Dictionary<string, ZhiboMode2Skill> ZhiboMode2SkillDict = new Dictionary<string, ZhiboMode2Skill>();

    public void fakeZhiboMode2Skill()
    {
        {
            ZhiboMode2Skill skill = new ZhiboMode2Skill();
            skill.Name = "禁言套餐";
            skill.PictureUrl = "Image_Yongjiufengjin";
            skill.Cd = 10f;
            skill.EnegyCost = 30;
            skill.Desp = "都他妈闭嘴！\n消除30条负面弹，靠近屏幕左侧的将被优先消除";
            skill.Se = new FightDanmuSkillEffect();
            skill.Se.type = eFightDanmuSkillEffectType.ClearDanmu;
            skill.Se.effectString = "30";
            ZhiboMode2SkillDict["0"] = skill;
        }

        {
            ZhiboMode2Skill skill = new ZhiboMode2Skill();
            skill.Name = "回怼弹幕";
            skill.PictureUrl = "Image_Bangyigegezuibangla";
            skill.Cd = 10f;
            skill.EnegyCost = 30;
            skill.Desp = "和弹幕对。\n获得100点分数，但同时也会触怒粉丝，生成一批负面弹幕";
            skill.Se = new FightDanmuSkillEffect();
            skill.Se.type = eFightDanmuSkillEffectType.GetScore;
            skill.Se.effectString = "100";
            ZhiboMode2SkillDict["1"] = skill;
        }
        {
            ZhiboMode2Skill skill = new ZhiboMode2Skill();
            skill.Name = "休养生息";
            skill.PictureUrl = "Image_Fanxiangdunai";
            skill.Cd = 10f;
            skill.EnegyCost = 30;
            skill.Desp = "喘口气，喝点水，接着怼！\n回复10点体力";
            skill.Se = new FightDanmuSkillEffect();
            skill.Se.type = eFightDanmuSkillEffectType.AddHp;
            skill.Se.effectString = "10";
            ZhiboMode2SkillDict["2"] = skill;
        }
        {
            ZhiboMode2Skill skill = new ZhiboMode2Skill();
            skill.Name = "专注直播";
            skill.PictureUrl = "Image_Bujieshilianzhao";
            skill.Cd = 10f;
            skill.EnegyCost = 30;
            skill.Desp = "专注直播，外物不可侵也\n接下来的8秒免疫负面弹幕伤害";
            skill.Se = new FightDanmuSkillEffect();
            skill.Se.type = eFightDanmuSkillEffectType.GetArmor;
            skill.Se.effectString = "8";
            ZhiboMode2SkillDict["3"] = skill;
        }
        {
            ZhiboMode2Skill skill = new ZhiboMode2Skill();
            skill.Name = "被动!";
            skill.PictureUrl = "Image_Banka";
            skill.Cd = 10f;
            skill.EnegyCost = 30;
            skill.Desp = "消除弹幕时获得额外1点分数";
            skill.Se = new FightDanmuSkillEffect();
            skill.Se.type = eFightDanmuSkillEffectType.GetHot;
            skill.Se.effectString = "1";
            ZhiboMode2SkillDict["4"] = skill;
        }

    }

    public void DestroyBadRandomly(int num)
    {
        List<DanmuMode2> toClean = randomPickBadDanmu(num);
        for (int i = 0; i < toClean.Count; i++)
        {
            DanmuMode2 danmu = toClean[i];
            danmu.OnDestroy();
            state.Danmus.Remove(danmu);
        }
    }

    private List<DanmuMode2> randomPickBadDanmu(int n)
    {
        List<DanmuMode2> ret = new List<DanmuMode2>();
        for (int i = 0; i < state.Danmus.Count; i++)
        {
            if (state.Danmus[i].isBad)
            {
                ret.Add(state.Danmus[i]);
            }
        }

        if(ret.Count <= n)
        {
            return ret;
        }

        ret.Sort(delegate (DanmuMode2 o1, DanmuMode2 o2)
            {
                if(o1.transform.position.x < o2.transform.position.y)
                {
                    return -1;
                }else if (o1.transform.position.x > o2.transform.position.y)
                {
                    return 1;
                }
                return 0;
            }
        );

        return ret.GetRange(0,n);
    }


    public void InitSuperDanmu()
    {

        state.SuperDanmus.Clear();

        state.SuperDanmuShowTimeList = new List<KeyValuePair<int, int>>();
        List<int> timeList = PickRandomTime(10, (int)state.OriginTime - 10, 10);

        for(int i = 0; i < timeList.Count; i++)
        {
            int randI = Random.Range(0, (int)eSuperDanmuType.Max);
            state.SuperDanmuShowTimeList.Add(new KeyValuePair<int, int>(timeList[i], randI));
        }

    }
    string[] superContent = new string[] { "主播你今天吃屎了吗？", "主播你今天吃屎了吗2？", "主播你今天吃屎了吗3？", "主播你今天吃屎了吗4？", "主播你今天吃屎了吗5？" };
    public string GetSuperDanmuContent()
    {
        int randI = Random.Range(0, superContent.Length);
        return superContent[randI];
    }

    public void ShowSuperDanmu(int idx)
    {
        SuperDanmuMode2 danmu = mUICtrl.ShowSuperDanmu(state.SuperDanmuShowTimeList[idx].Value);

        state.SuperDanmus.Add(danmu);
        danmu.Activated = true;
    }

    public void DestroySuperDanmu(SuperDanmuMode2 danmu)
    {
        mResLoader.ReleaseGO("ZhiboMode2/SuperDanmu", danmu.gameObject);
    }

    public List<int> PickRandomTime(int from, int to, int timeNum)
    {
        List<int> ret = new List<int>();

        float inteval = (to - from) * 1.0f / (timeNum - 1);
        for (int i = 0; i < timeNum; i++)
        {
            int t = from + (int)(i * inteval + (Random.value * 0.8f - 0.4f) * inteval);
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

    //public void ClearAllDanmu(bool getScore)
    //{
    //    for (int i = state.Danmus.Count - 1; i >= 0; i--)
    //    {
    //        Danmu danmu = state.Danmus[i];
    //        danmu.OnDestroy();
    //        state.Danmus.Remove(danmu);
    //        mUICtrl.ShowDanmuEffect(danmu.transform.position);
    //        if (danmu.isBad)
    //        {
    //            GainScore(-2);
    //            //AddHp(-1);
    //        }
    //        else
    //        {
    //            GainScore(1);
    //        }
    //    }

    //    for (int i = state.SuperDanmus.Count - 1; i >= 0; i--)
    //    {
    //        if (state.SuperDanmus[i].HasDisapeared)
    //        {
    //            continue;
    //        }
    //        state.SuperDanmus[i].Disappear();
    //    }
    //    state.SuperDanmus.Clear();
    //    mUICtrl.ClearSuperDanmu();
    //}

}
