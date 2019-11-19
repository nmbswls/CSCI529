using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AudienceToken
{

}

public class ZhiboAudienceMgr
{
    public ZhiboGameMode gameMode;

    List<ZhiboAudience> TargetList;

    List<ZhiboLittleTV> LittleTvList = new List<ZhiboLittleTV>();
    List<int> EmptyTVList = new List<int>();

    public IResLoader mResLoader;
    public List<ZhiboAudience> audienceSuq = new List<ZhiboAudience>();

    IRoleModule mRoleMgr;

    private int[] EachTurnMaxEnemyNum;
    private int EnemyIdx;

    GameObject tokenDetail;

    public ZhiboAudienceMgr(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        TargetList = gameMode.nowAudiences;
        EachTurnMaxEnemyNum = new int[gameMode.state.OriginTurn];
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        GenAudienceMode();
        GenAudienceSequence();
        InitUI();
    }

    public void InitUI()
    {
        Transform root = gameMode.mUICtrl.GetAudienceRoot();

        foreach (Transform tv in root)
        {
            ZhiboLittleTV vv = tv.GetComponent<ZhiboLittleTV>();
            vv.Init(this);
            LittleTvList.Add(vv);
        }
        for (int i = 0; i < LittleTvList.Count; i++)
        {
            EmptyTVList.Add(i);
        }


        tokenDetail = gameMode.mUICtrl.GetTokenDetailPanel();
    }

    public void Tick()
    {
        //for (int i = gameMode.nowAudiences.Count - 1; i >= 0; i--)
        //{
        //    if (gameMode.nowAudiences[i].AttractLeftTurn > 0)
        //    {
        //        gameMode.nowAudiences[i].AttractLeftTurn -= 1;
        //        if (gameMode.nowAudiences[i].AttractLeftTurn == 0)
        //        {
        //            //mUICtrl.remove audience
        //        }
        //    }

        //}

    }



    public void TickSec()
    {
        //for (int i = LittleTvList.Count - 1; i >= 0; i--)
        //{
        //    LittleTvList[i].TickSec();
        //    if (LittleTvList[i].TimeLeft <= 0)
        //    {
        //        if (!EmptyTVList.Contains(i))
        //        {
        //            EmptyTVList.Add(i);
        //        }
        //    }
        //}
            //view.TurnTimeValue.text = (int)gameMode.state.TurnTimeLeft+"";
    }


    public void ApplyAddScore(int idx, float score)
    {
        if(idx < 0 || idx >= TargetList.Count)
        {
            return;
        }
        TargetList[idx].AddScore(score);
        if(TargetList[idx].BindViewIdx != -1)
        {
            LittleTvList[TargetList[idx].BindViewIdx].UpdateScore();
        }

    }

    public void ApplyBlackHit(int damage)
    {
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if(TargetList[i].BlackHp > 0)
            {
                TargetList[i].BlackHp -= damage;
                TargetList[i].BlackHp = TargetList[i].BlackHp < 0 ? 0 : TargetList[i].BlackHp;
            }
        }
    }

    Queue<ZhiboAudience> KilledAudience = new Queue<ZhiboAudience>();



    public List<int> HandleGemHit(int[] damage, AudienceToken token = null)
    {
        bool hasQueue = false;
        if (KilledAudience.Count > 0)
        {
            hasQueue = true;
        }
        List<int> oritinAffectedList = ApplyGemHit(damage,null);

        if (!hasQueue)
        {
            GameMain.GetInstance().RunCoroutine(HandleKillingQueue());
        }

        return oritinAffectedList;
    }

    IEnumerator HandleKillingQueue()
    {
        while (KilledAudience.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            //if(KilledAudience.Count > 0)
            //{
                ZhiboAudience a = KilledAudience.Dequeue();
                if (a == null)
                {
                    Debug.Log("errrrrrr");
                }
                AudienceAttracted(a);
            //}
        }
    }

    public List<int> ApplyGemHit(int[] damage, AudienceToken token = null)
    {
        List<int> affectedAudiences = new List<int>();
        //List<ZhiboAudience> KilledAudience = new List<ZhiboAudience>();
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if (TargetList[i].state != eAudienceState.Normal)
            {
                continue;
            }
            bool ret = TargetList[i].ApplyDamage(damage);
            if (ret)
            {
                affectedAudiences.Add(i);
                ShowAudienceHit(TargetList[i]);
                PlaceToken(TargetList[i], token);
                UpdateAudienceHp(TargetList[i]);
            }
            if (TargetList[i].isDead())
            {
                //根据 观众血量类型 及 放置的代币 计算加成
                HandleGetScore(TargetList[i]);
                //根据 nowAudiences[i].GemMaxHp 获得热度
                //ShowAudienceEffect(TargetList[i]);
                KilledAudience.Enqueue(TargetList[i]);
                //AudienceAttracted(TargetList[i]);
                TargetList[i].Attracted();
                //LittleTvList[TargetList[i].BindViewIdx].Attracted();
            }
        }
        return affectedAudiences;
    }


    //IEnumerator HandleAudienceKilling(List<ZhiboAudience> killingList)
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    Queue<int[]> damageChain = new Queue<int[]>();
    //    for (int i = 0; i < killingList.Count; i++)
    //    {
    //        if (!TargetList.Contains(killingList[i]))
    //        {
    //            Debug.Log("err not contain thie audience");
    //            continue;
    //        }
    //        AudienceAttracted(killingList[i], damageChain);
    //    }


    //    int[] AllDamage = new int[6];
    //    int tDamage = 0;
    //    while (damageChain.Count > 0)
    //    {
    //        int[] d = damageChain.Dequeue();
    //        for (int j = 0; j < 6; j++)
    //        {
    //            AllDamage[j] += d[j];
    //            tDamage += d[j];
    //        }
    //    }
    //    if (tDamage > 0)
    //    {
    //        ApplyGemHit(AllDamage, null);
    //    }
    //}

    public void HandleGetScore(ZhiboAudience audience)
    {
        float totalScore = audience.NowScore;

        int baseScore = audience.Level * 10; 
        //totalScore += 
 
        for (int i = 1; i < 6; i++)
        {

            mRoleMgr.GetStats();
            //audience.GemMaxHp[i];
        }
        gameMode.GainScore(audience.Level * 10);
    }

    public void AudienceAttracted(ZhiboAudience audience)
    {



        if (audience.BindViewIdx != -1)
        {
            //
            gameMode.GainScore(audience.NowScore);

            // handle all audience.Tokens();
            for (int i = 0; i < audience.Bonus.Count; i++)
            {
                ZhiboAudienceBonus bonuw = audience.Bonus[i];


                switch (bonuw.Type)
                {
                    case eAudienceBonusType.AddHp:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语回血");
                        gameMode.AddHp(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Aoe:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语aoe");
                        string[] args = bonuw.effectString.Split(',');
                        if(args.Length != 6)
                        {
                            break;
                        }
                        int[] damaged = new int[6];
                        for(int jj = 0; jj < args.Length; jj++)
                        {
                            damaged[jj] = int.Parse(args[jj]);
                        }
                        ApplyGemHit(damaged);
                        //damageChain.Enqueue(damaged);
                        //gameMode.AddHp(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Damage:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语伤血");
                        gameMode.AddHp(-int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Score:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语加分");
                        gameMode.GainScore(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Dual:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语抽卡");
                        gameMode.AddCardFromDeck();
                        break;
                    case eAudienceBonusType.Discard:
                        GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语弃卡");
                        gameMode.DiscardRandomCards(1);
                        break;
                    default:
                        break;
                }

            }
        }
    }

    public void PutIntoChain(ZhiboAudience a)
    {

    }

    public void PlaceToken(ZhiboAudience audience, AudienceToken token)
    {
        if(token == null)
        {
            return;
        }

    }

    public void ShowAudienceHit(ZhiboAudience audience)
    {
        if(audience.BindViewIdx != -1)
        {
            LittleTvList[audience.BindViewIdx].Affected();
            //gameMode.mUICtrl.ShowAudienceHitEffect(LittleTvList[audience.BindViewIdx].transform.position);
        }
    }




    public void UpdateAudienceHp(ZhiboAudience audience)
    {

        if (audience.BindViewIdx != -1)
        {
            LittleTvList[audience.BindViewIdx].HpFadeOut();
        }
    }



    public void ShowAudienceKilledEffect(ZhiboAudience audience)
    {

        if (audience.BindViewIdx != -1)
        {
            gameMode.mUICtrl.ShowDanmuEffect(LittleTvList[audience.BindViewIdx].transform.position);
        }
    }





    public void GenAudienceSequence()
    {
        int turn = gameMode.state.OriginTurn;
        EachTurnMaxEnemyNum[0] = 2;
        EachTurnMaxEnemyNum[1] = 3;
        EachTurnMaxEnemyNum[2] = 3;
        EachTurnMaxEnemyNum[3] = 4;
        EachTurnMaxEnemyNum[4] = 4;
        EachTurnMaxEnemyNum[5] = 4;
        EachTurnMaxEnemyNum[6] = 5;

        int originLevel = 1;
        {
            for(int i = 0; i < 50; i++)
            {
                ZhiboAudience audience = new ZhiboAudience();
                audience.Level = originLevel + i / 3;
                if (i % 4 == 2)
                {
                    audience.Type = eAudienceType.Heizi;
                    audience.BlackHp = audience.Level;
                }
                else
                {
                    audience.Type = eAudienceType.Good;
                    int randI = Random.Range(0,rates.Count);
                    float[] rate = rates[randI];
                    int[] hp = GetHpTemplate(audience.Level, rate);
                    audience.GemMaxHp = new int[6];
                    audience.GemHp = new int[6];
                    for(int j = 0; j < 6; j++)
                    {
                        audience.GemMaxHp[j] = hp[j];
                        audience.GemHp[j] = hp[j];
                    }
                    if (Random.value < 0.5f)
                    {
                        ZhiboAudienceBonus bonus = new ZhiboAudienceBonus();
                        int randIdx = Random.Range(1, (int)eAudienceBonusType.Max);
                        bonus.Type = (eAudienceBonusType)randIdx;
                        switch (bonus.Type)
                        {
                            case eAudienceBonusType.AddHp:
                                bonus.effectString = 1 + (originLevel / 5) + "";
                                break;
                            case eAudienceBonusType.Aoe:
                                bonus.effectString = 1 + (originLevel / 5) + ",0,0,0,0,0";
                                break;
                            case eAudienceBonusType.Damage:
                                bonus.effectString = 1 + (originLevel / 3) + "";
                                break;
                            case eAudienceBonusType.Discard:
                                bonus.effectString = "1";
                                break;
                            case eAudienceBonusType.Dual:
                                bonus.effectString = "1";
                                break;
                            case eAudienceBonusType.Score:
                                bonus.effectString = "30";
                                break;
                            default:
                                break;
                        }
                        audience.Bonus.Add(bonus);
                    }
                }

                audienceSuq.Add(audience);
            }
        }
    }

    public int[] GetHpTemplate(int level, float[] rate)
    {
        int hpNum = 1 + level;
        int hpLeft = hpNum;
        int[] ret = new int[6];

        if (rate[0] > 0f)
        {
            ret[0] = (int)Mathf.Round(hpNum * rate[0]);
            if(ret[0]> hpNum)
            {
                ret[0] = hpNum;
            }
            hpLeft -= ret[0];
        }

        List<int> left = new List<int>();
        for(int i = 1; i <= 5; i++)
        {
            left.Add(i);
        }
        int rateIdx = 1;
        while(rateIdx<6)
        {
            if (rate[rateIdx] == 0)
            {
                break;
            }
            int nextAttr = left[Random.Range(0,left.Count)];
            int roundValue = (int)Mathf.Round(hpNum * rate[rateIdx]);
            if(hpLeft < roundValue)
            {
                ret[nextAttr] = hpLeft;
                break;
            }
            left.Remove(nextAttr);
            rateIdx++;
        }
        if (hpLeft > 0)
        {
            List<int> DistributedPos = new List<int>();
            for(int i = 1; i < 6; i++)
            {
                if (ret[i] > 0)
                {
                    DistributedPos.Add(i);
                }
            }
            if(DistributedPos.Count == 0)
            {
                for(int i = 1; i < 6; i++)
                {
                    DistributedPos.Add(i);
                }
            }

            for(int i=0;i< hpLeft; i++)
            {
                int rIdx = Random.Range(0, DistributedPos.Count);
                ret[DistributedPos[rIdx]] += 1;
            }

        }

        return ret;
    }

    public void FinishTurn()
    {
        CheckOverdue();

        AudienceCauseDamage();
    }

    public void NextTurn()
    {
        int maxNum = 0;

        if (gameMode.state.NowTurn - 1 >= EachTurnMaxEnemyNum.Length)
        {
            maxNum = EachTurnMaxEnemyNum[EachTurnMaxEnemyNum.Length - 1];
        }
        else
        {
            maxNum = EachTurnMaxEnemyNum[gameMode.state.NowTurn - 1];
        }

        int nowCount = 0;
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.Normal)
            {
                nowCount++;
            }
        }

        for (int i=0;i< maxNum - nowCount; i++)
        {
            ZhiboAudience a = audienceSuq[EnemyIdx];
            ShowNextAudience(a);
            EnemyIdx += 1;
        }

    }

    public void ShowNextAudience(ZhiboAudience audience)
    {
        int idx = ShowNewAudience(audience);
        audience.BindViewIdx = idx;
        TargetList.Add(audience);
    }

    public void ShowRandomAudience()
    {

        ZhiboAudience audience = new ZhiboAudience();
        {
            audience.Type = eAudienceType.Normal;
            audience.GemHp[0] = 1;
            audience.GemHp[1] = 0;
            audience.GemHp[2] = 0;
            audience.GemHp[3] = 0;
            audience.GemHp[4] = 0;
            audience.GemHp[5] = 0;
            audience.state = eAudienceState.Normal;
        }
        int idx = ShowNewAudience(audience);
        audience.BindViewIdx = idx;
        TargetList.Add(audience);
    }

    public void CheckOverdue()
    {
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if(TargetList[i].state == eAudienceState.Normal)
            {
                TargetList[i].LastTurn -= 1;
                if(TargetList[i].LastTurn <= 0)
                {
                    if (TargetList[i].BindViewIdx == -1)
                    {
                        return;
                    }
                    LittleTvList[TargetList[i].BindViewIdx].Disappear();
                    TargetList.RemoveAt(i);
                }
            }else if (TargetList[i].state == eAudienceState.Attracted)
            {
                TargetList[i].AttractLeftTurn -= 1;
                if (TargetList[i].AttractLeftTurn <= 0)
                {
                    if (TargetList[i].BindViewIdx == -1)
                    {
                        return;
                    }
                    LittleTvList[TargetList[i].BindViewIdx].Disappear();
                    TargetList.RemoveAt(i);
                }
            }
        }
    }

    public void AudienceCauseDamage()
    {
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.Attracted
                || TargetList[i].state == eAudienceState.None)
            {
                continue;
            }

            if(TargetList[i].BindViewIdx == -1)
            {
                continue;
            }

            int damage = 0;
            for(int j=0;j< TargetList[i].GemHp.Length; j++)
            {
                damage += TargetList[i].GemHp[j];
            }
            gameMode.AddHp(-damage);


            gameMode.mUICtrl.ShowDamageEffect(LittleTvList[TargetList[i].BindViewIdx].transform.position);
        }
    }

    public void ShowAudienceDamagedEffect()
    {

    }

    public int ShowNewAudience(ZhiboAudience audience)
    {
        if (EmptyTVList.Count == 0)
        {
            return -1;
        }

        
        int idx = Random.Range(0, EmptyTVList.Count);
        

        idx = EmptyTVList[idx];

        EmptyTVList.Remove(idx);

        LittleTvList[idx].Show(audience);

        audience.state = eAudienceState.Normal;
        return idx;
    }



    List<float[]> rates = new List<float[]>();

    public void GenAudienceMode()
    {

        {
            //硬核观众
            float[] rate = new float[] {0, 1f, 0f, 0f, 0f, 0f};
            rates.Add(rate);
        }
        {
            //不挑的观众
            float[] rate = new float[] { 1, 0f, 0f, 0f, 0f, 0f, 0f};
            rates.Add(rate);
        }
        {
            //喜爱较专一
            float[] rate = new float[] { 0, 0.8f, 0.2f, 0f, 0f, 0f};
            rates.Add(rate);
        }
        {
            //喜好一般专一
            float[] rate = new float[] { 0, 0.6f, 0.2f, 0.2f, 0f ,0f};
            rates.Add(rate);
        }
        {

            float[] rate = new float[] { 0, 0.6f, 0.1f, 0.1f, 0.1f, 0.1f};
            rates.Add(rate);
        }
        {
            float[] rate = new float[] { 0, 0.5f, 0.5f,  0f, 0f, 0f};
            rates.Add(rate);
        }
        {
            float[] rate = new float[] { 0.5f, 0.5f,0f, 0f,0f,0f};
            rates.Add(rate);
        }
        {
            float[] rate = new float[] { 0.2f, 0.6f, 0.2f, 0f,0f,0f};
            rates.Add(rate);
        }
        {
            float[] rate = new float[] { 0f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
            rates.Add(rate);
        }
        {
            float[] rate = new float[] { 0f, 0.4f, 0.2f, 0.2f, 0.2f, 0f };
            rates.Add(rate);
        }
    }

    public void ShowTokenDetail(ZhiboLittleTV tvView)
    {
        tokenDetail.SetActive(true);
        tokenDetail.transform.position = tvView.transform.position + new Vector3(0.4f, 0.4f, 0);
    }

    public void HideTokenDetail()
    {
        tokenDetail.SetActive(false);
    }
}
