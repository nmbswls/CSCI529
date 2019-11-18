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

    public List<int> ApplyGemHit(int[] damage, AudienceToken token = null)
    {
        List<int> affectedAudiences = new List<int>();
        List<ZhiboAudience> KilledAudience = new List<ZhiboAudience>();
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
                KilledAudience.Add(TargetList[i]);
                //AudienceAttracted(TargetList[i]);
                TargetList[i].Attracted();
                //LittleTvList[TargetList[i].BindViewIdx].Attracted();
            }

        }

        Queue<int[]> damageChain = new Queue<int[]>();
        for (int i = 0; i < KilledAudience.Count; i++)
        {
            AudienceAttracted(KilledAudience[i], damageChain);
        }

        int[] AllDamage = new int[6];
        int tDamage = 0;
        while(damageChain.Count > 0)
        {
            int[] d = damageChain.Dequeue();
            for(int j = 0; j < 6; j++)
            {
                AllDamage[j] += d[j];
                tDamage += d[j];
            }
        }
        if(tDamage > 0)
        {
            ApplyGemHit(AllDamage,null);

        }
        return affectedAudiences;
    }


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

    public void AudienceAttracted(ZhiboAudience audience, Queue<int[]> damageChain)
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
                        gameMode.AddHp(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Aoe:
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
                        damageChain.Enqueue(damaged);
                        //gameMode.AddHp(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Damage:
                        gameMode.AddHp(-int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Score:
                        gameMode.GainScore(int.Parse(bonuw.effectString));
                        break;
                    case eAudienceBonusType.Dual:
                        gameMode.AddCardFromDeck();
                        break;
                    case eAudienceBonusType.Discard:
                        gameMode.DiscardRandomCards(1);
                        break;
                    default:
                        break;
                }
                Debug.Log("attracted");

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
            gameMode.mUICtrl.ShowAudienceHitEffect(LittleTvList[audience.BindViewIdx].transform.position);
        }
    }




    public void UpdateAudienceHp(ZhiboAudience audience)
    {

        if (audience.BindViewIdx != -1)
        {
            LittleTvList[audience.BindViewIdx].HpFadeOut();
        }
    }

    public void ShowAudienceEffect(ZhiboAudience audience)
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
                        //audience.
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
            bool distribured = false;
            for(int i = 1; i < 6; i++)
            {
                if (ret[i] > 0)
                {
                    ret[i] += hpLeft;
                    distribured = true;
                    break;
                }
            }
            if (!distribured)
            {
                ret[Random.Range(0, 6)] += hpLeft;
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
                    LittleTvList[TargetList[i].BindViewIdx].Disappear();
                    TargetList.RemoveAt(i);
                }
            }else if (TargetList[i].state == eAudienceState.Attracted)
            {
                TargetList[i].AttractLeftTurn -= 1;
                if (TargetList[i].AttractLeftTurn <= 0)
                {
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
}
