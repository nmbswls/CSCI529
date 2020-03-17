using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AudienceToken
{

}
public class AudienceAuraBuff
{
    public int ScoreLess;
}

public class AudienceProfixEffect
{
    public int[] hp = {0,0,0,0,0,0};
    public int waitTime = 0;
}

public class AudienceSuffixEffect
{
    public int[] hp = {0,0,0,0,0,0};
    public int waitTime = 0;
}

public class TokenDetailView
{
    public Transform root;
    public Text Content;

    public void BindView(Transform root)
    {
        this.root = root;
        this.Content = root.Find("Content").GetComponent<Text>();
    }
}

public class ZhiboAudienceMgr
{

    Dictionary<eZhiboAudienceSkillType, Dictionary<int, string>> SkillDespDict = new Dictionary<eZhiboAudienceSkillType, Dictionary<int, string>>
    {


        { eZhiboAudienceSkillType.Aura, new Dictionary<int,string>{ 
            {(int)eAudienceAuraType.LessScore, "观众存在时，获得分数减少{0}"} 


            }},
        { eZhiboAudienceSkillType.Bonus, new Dictionary<int,string>{
            {(int)eAudienceBonusType.None,"无"},
            {(int)eAudienceBonusType.AddHp,"恢复{0}点健康"},
            {(int)eAudienceBonusType.Aoe,"全场满足{0}点需求"},
            {(int)eAudienceBonusType.Damage,""},
            {(int)eAudienceBonusType.Dual,"抽取{0}张卡"},
            {(int)eAudienceBonusType.Score,"获得{0}点分数"},

            }},
        { eZhiboAudienceSkillType.Punish, new Dictionary<int,string>{
            {(int)eAudiencePunishType.None,"无"},
            {(int)eAudiencePunishType.Damage,"造成{0}点伤害"},
            {(int)eAudiencePunishType.Discard,"丢弃{0}张卡"},
            {(int)eAudiencePunishType.Score,"扣除{0}点分数"},

            }},
        { eZhiboAudienceSkillType.TurnEffect, new Dictionary<int,string>{
            {(int)eAudienceTurnEffectType.None,"无"},
            {(int)eAudienceTurnEffectType.AddKoucaiReq,"削减{0}点口才满意度"},

            }},

    };



    public ZhiboGameMode gameMode;

    List<ZhiboAudience> TargetList;

    List<ZhiboLittleTV> LittleTvList = new List<ZhiboLittleTV>();
    //空电视索引表
    List<int> EmptyTVList = new List<int>();
    //已归还的电视 将在下一回合再加回池中，除非当前已经占满必须立刻使用它们
    List<int> WaitingReturnList = new List<int>();

    public IResLoader mResLoader;
    public List<ZhiboAudience> audienceSuq = new List<ZhiboAudience>();

    IRoleModule mRoleMgr;

    private int[] EachTurnMaxEnemyNum;
    private int EnemyIdx;

    //GameObject tokenDetail;
    TokenDetailView tokenDetail;
    AudienceAuraBuff audienceAuraBuff = new AudienceAuraBuff();

    TVSuffixList SuffixList = new TVSuffixList();
    TVProfixList ProfixList = new TVProfixList();


    public ZhiboAudienceMgr(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        TargetList = gameMode.nowAudiences;
        EachTurnMaxEnemyNum = new int[gameMode.state.OriginTurn];
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        if (loadSuffix())
        {
            Debug.Log("后缀加载成功 Count = " + SuffixList.suffixs.Count);
        }
        if (loadProfix())
        {
            Debug.Log("前缀加载成功 Count = " + ProfixList.profixs.Count);
        }

        GenAudienceMode();
        GenAudienceSequence();
        //LoadSkillDespDict();

        
        InitUI();
    }

    private void LoadSkillDespDict()
    {

        TextAsset ta = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<TextAsset>("AudianceCfg/skillDesp", false);
        if(ta == null)
        {
            Debug.Log("load audience skill desp fail");
            return;
        }
        SkillDespDict = JsonConvert.DeserializeObject< Dictionary<eZhiboAudienceSkillType, Dictionary<int, string>>> (ta.text);

        Debug.Log("load desp");
    }

    float timeLeftChangeInterval = 0.2f;
    public void Tick(float dTime)
    {
        timeLeftChangeInterval += dTime;
        for (int i = TargetList.Count -1; i >= 0; i--)
        {
            if(TargetList[i].BlackHp > 0)
            {
                continue;
            }
            if (timeLeftChangeInterval > 0.2f)
            {
                if (TargetList[i].BindViewIdx != -1)
                {
                    LittleTvList[TargetList[i].BindViewIdx].UpdateTimeLeft();
                }
            }
            if (TargetList[i].TimeLeft > 0)
            {
                TargetList[i].TimeLeft -= dTime;
                if(TargetList[i].TimeLeft <= 0)
                {
                    AudienceLeave(TargetList[i]);
                }
            }
            
        }
        if (timeLeftChangeInterval > 0.2f)
        {
            timeLeftChangeInterval = 0;
        }
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

        GameObject go = gameMode.mUICtrl.GetTokenDetailPanel();
        tokenDetail = new TokenDetailView();
        tokenDetail.BindView(go.transform);
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

    public void KillHeizi(int num)
    {
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if (TargetList[i].BlackHp > 0)
            {
                //AudienceAttracted(TargetList[i]);

                LittleTvList[TargetList[i].BindViewIdx].Disappear();
                TargetList.RemoveAt(i);
            }
        }
    }

    public void ApplyBlackHit(int damage)
    {
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if (TargetList[i].BlackHp > 0)
            {
                TargetList[i].BlackHp -= damage;
                TargetList[i].BlackHp = TargetList[i].BlackHp < 0 ? 0 : TargetList[i].BlackHp;

                ShowAudienceHit(TargetList[i]);
                UpdateAudienceHp(TargetList[i]);
                if(TargetList[i].BlackHp == 0)
                {
                    //need fix fix fix 不优雅
                    TargetList[i].state = eAudienceState.None;
                    LittleTvList[TargetList[i].BindViewIdx].Disappear();
                    TargetList.Remove(TargetList[i]);
                    
                }
            }
        }
        CalculateAura();
    }

    Queue<ZhiboAudience> KilledAudience = new Queue<ZhiboAudience>();


    

    public List<int> HandleGemHit(int[] damage, int extra = 0, AudienceToken token = null)
    {
        bool hasQueue = false;
        if (KilledAudience.Count > 0)
        {
            hasQueue = true;
        }
        List<int> oritinAffectedList = ApplyGemHit(damage, extra, null);

        if (!hasQueue)
        {
            GameMain.GetInstance().RunCoroutine(HandleKillingQueue());
        }

        return oritinAffectedList;
    }

    public List<int> HandleGemRandomHit(int[] damage)
    {
        bool hasQueue = false;
        if (KilledAudience.Count > 0)
        {
            hasQueue = true;
        }
        List<int> oritinAffectedList = ApplyRandomGemHit(damage);

        if (!hasQueue)
        {
            GameMain.GetInstance().RunCoroutine(HandleKillingQueue());
        }

        return oritinAffectedList;
    }

    public List<int> ApplyRandomGemHit(int[] damage, int extra = 0)
    {
        //List<int> audienceToUpdateHp = new List<int>();

        List<int> affectedAudiences = new List<int>();
        for (int i = 0; i < damage.Length; i++)
        {
            int dAmount = damage[i];
            List<int> targes = new List<int>();
            for (int ti = TargetList.Count - 1; ti >= 0; ti--)
            {
                if (TargetList[ti].state != eAudienceState.Normal)
                {
                    continue;
                }
                if (TargetList[ti].MaxReq[i] > TargetList[ti].NowReq[i])
                {
                    targes.Add(ti);
                }
            }
            if (targes.Count == 0)
            {
                continue;
            }
            int idx = targes[Random.Range(0, targes.Count)];
            TargetList[idx].NowReq[i] += 1;



            if (!affectedAudiences.Contains(idx))
            {
                affectedAudiences.Add(idx);
            }
        }

        for (int i = 0; i < affectedAudiences.Count; i++)
        {
            int idx = affectedAudiences[i];

            //affectedAudiences.Add(idx);
            ShowAudienceHit(TargetList[idx]);
            UpdateAudienceHp(TargetList[idx]);

            if (TargetList[idx].isSatisfied())
            {

                KilledAudience.Enqueue(TargetList[idx]);
                //TargetList[idx].Attracted();
            }
        }
        //空伤害 触发队列

        return affectedAudiences;
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
                AudienceSatisfied(a);

            //}
        }
    }

    public List<int> ApplyGemHit(int[] damage, int extra = 0, AudienceToken token = null)
    {
        List<int> affectedAudiences = new List<int>();
        //List<ZhiboAudience> KilledAudience = new List<ZhiboAudience>();
        for (int i = TargetList.Count - 1; i >= 0; i--)
        {
            if (TargetList[i].state != eAudienceState.Normal)
            {
                continue;
            }
            int[] realDamage = new int[damage.Length];
            for(int d = 0; d < realDamage.Length; d++)
            {
                realDamage[d] = (int)(damage[d] * (1 + extra * 0.01f));
            }
            bool ret = TargetList[i].ApplyDamage(realDamage);
            if (ret)
            {
                affectedAudiences.Add(i);
                ShowAudienceHit(TargetList[i]);
                PlaceToken(TargetList[i], token);
                UpdateAudienceHp(TargetList[i]);
            }
            if (TargetList[i].isSatisfied())
            {
                //根据 观众血量类型 及 放置的代币 计算加成

                //根据 nowAudiences[i].GemMaxHp 获得热度
                //ShowAudienceEffect(TargetList[i]);
                KilledAudience.Enqueue(TargetList[i]);
                //AudienceAttracted(TargetList[i]);
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



    public void HandleTurnLeftBonus(ZhiboAudience audience)
    {
        if(audience.LastTurn == 3)
        {
            gameMode.AddHp(3);
        }
        else if (audience.LastTurn == 2)
        {
            gameMode.AddHp(3);
        }
        else
        {
            gameMode.AddHp(3);
        }
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
        float buffRate = (1 - audienceAuraBuff.ScoreLess * 0.01f);
        if (buffRate < 0)
        {
            buffRate = 0;
        }
        gameMode.GainScore(audience.Level * 10 * buffRate);
    }


    private void HandleAudiennceBonus(ZhiboAudience audience)
    {
        for (int i = 0; i < audience.Skills.Count; i++)
        {
            if (audience.Skills[i].skillType != eZhiboAudienceSkillType.Bonus)
            {
                continue;
            }
            ZhiboAudienceSkill bonuw = audience.Skills[i];
            eAudienceBonusType type = (eAudienceBonusType)bonuw.effectId;

            switch (type)
            {
                case eAudienceBonusType.AddHp:
                    GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语回血");
                    gameMode.AddHp(int.Parse(bonuw.effectString));
                    break;
                case eAudienceBonusType.Aoe:
                    GameMain.GetInstance().GetModule<UIMgr>().ShowHint("亡语aoe");
                    string[] args = bonuw.effectString.Split(',');
                    if (args.Length != 6)
                    {
                        break;
                    }
                    int[] damaged = new int[6];
                    for (int jj = 0; jj < args.Length; jj++)
                    {
                        damaged[jj] = int.Parse(args[jj]);
                    }
                    ApplyGemHit(damaged, 0);
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

    public void AudienceSatisfied(ZhiboAudience audience)
    {
        if (audience.BindViewIdx == -1)
        {
            return;
        }




        HandleGetScore(audience);
        HandleTurnLeftBonus(audience);
        HandleAudiennceBonus(audience);
        if (audience.BindViewIdx != -1)
        {

            // handle all audience.Tokens();
            LittleTvList[audience.BindViewIdx].Disappear();
            TargetList.Remove(audience);
        }
        CalculateAura();

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
            LittleTvList[audience.BindViewIdx].UpdateHp();
        }
    }



    public void ShowAudienceKilledEffect(ZhiboAudience audience)
    {

        if (audience.BindViewIdx != -1)
        {
            gameMode.mUICtrl.ShowDanmuEffect(LittleTvList[audience.BindViewIdx].transform.position);
        }
    }



    public void PutScoreOnAudience(float score, bool and = true, List<int> target = null)
    {
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.Normal)
            {
                if (TargetList[i].ApplyColorFilter(and, target))
                {
                    TargetList[i].AddScore(score);
                    LittleTvList[TargetList[i].BindViewIdx].UpdateScore();
                    
                }
            }
        }
    }

    public void addExtraHp(int idx)
    {
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.Normal)
            {
                TargetList[i].addExtraHp(idx,1);
                UpdateAudienceHp(TargetList[i]);
            }
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
                //4040
                audience.OriginTimeLast = 40f;
                audience.TimeLeft = 40f;
                //if (i % 4 == 2)
                {
                    //audience.Type = eAudienceType.Heizi;
                    //audience.BlackHp = audience.Level;
                }
                //else
                {
                    audience.Type = eAudienceType.Good;
                    int randI = Random.Range(0,rates.Count);
                    float[] rate = rates[randI];
                    int[] req = GetHpTemplate(audience.Level, rate);
                    audience.MaxReq = new int[6];
                    audience.NowReq = new int[6];

                    //add prefix & surfix here

                    int[] tmpSufHp = { 0, 0, 0, 0, 0, 0 };
                    int[] tmpProHp = { 0, 0, 0, 0, 0, 0 };
                    int tmpWaitTime = 0;
                    
                    TVSuffix appliedSuf = applySuffixEffect();
                    tmpSufHp = loadSuffixEffect(appliedSuf);
                    audience.tvSuffix = appliedSuf;

                    TVProfix appliedPro = applyProfixEffect();
                    if(appliedPro!=null)
                    {
                        AudienceProfixEffect audienceProfixEffect = loadProfixEffect(appliedPro, req);
                        tmpProHp = audienceProfixEffect.hp;
                        tmpWaitTime = audienceProfixEffect.waitTime;
                        audience.tvProfix = appliedPro;
                    }
                   
                    for (int j = 0; j < 6; j++)
                    {
                        int tmpMaxReq = req[j] + tmpProHp[j] + tmpSufHp[j];
                        if(audience.MaxReq[j] == 0 &&tmpMaxReq <=0)
                        {
                            audience.MaxReq[j] = 0;
                        } else if(audience.MaxReq[j] == -tmpMaxReq)
                        {
                            //如果之前颜色的需求不为0禁止将同一色完全归0;
                        } else
                        {
                            audience.MaxReq[j] = tmpMaxReq;
                        }
                        //audience.NowReq[j] = hp[j] + tmpProHp[j] + tmpSufHp[j];
                    }
                    if (Random.value < 0.5f)
                    {
                        ZhiboAudienceSkill bonus = new ZhiboAudienceSkill(eZhiboAudienceSkillType.Bonus);
                        int randIdx = Random.Range(0, (int)eAudienceBonusType.Max);
                        bonus.effectId = randIdx;
                        switch ((eAudienceBonusType)bonus.effectId)
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
                        audience.Skills.Add(bonus);
                    }
                    else
                    {
                        ZhiboAudienceSkill aura = new ZhiboAudienceSkill(eZhiboAudienceSkillType.Aura);
                        aura.effectId = (int)eAudienceAuraType.LessScore;
                        aura.effectString = "20";
                        audience.Skills.Add(aura);
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
        //先还再check 顺序重要
        ReturnWaitingTVs();

        CheckOverdue();

        AudienceCauseDamage();

        AudienceTurnEffect();
    }

    private int GetMaxReqNum()
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
        return maxNum;
    }

    private int GetNowReqNum()
    {
        int nowCount = 0;
        for (int i = 0; i < TargetList.Count; i++)
        {
            if (TargetList[i].state == eAudienceState.Normal)
            {
                nowCount++;
            }
        }
        return nowCount;
    }

    public void NextTurn()
    {
        int maxNum = GetMaxReqNum();

        int nowCount = GetNowReqNum();

        List<ZhiboLittleTV> targetLittleTv = new List<ZhiboLittleTV>();

        for (int i=0;i< maxNum - nowCount; i++)
        {
            //ZhiboAudience a = audienceSuq[EnemyIdx];
            int slotIdx = ShowNextAudience();
            if(slotIdx == -1)
            {
                Debug.Log("满怪了 异常");
                return;
            }
            targetLittleTv.Add(LittleTvList[slotIdx]);
            //EnemyIdx += 1;
        }

        gameMode.mZhiboDanmuMgr.ShowImportantDanmu(maxNum - nowCount, targetLittleTv);

    }

    public bool canAddNewReq()
    {
        int maxReq = GetMaxReqNum();
        int nowReq = GetNowReqNum();
        return nowReq < maxReq;
    }

    public int ShowNextAudience()
    {

        ZhiboAudience audience = audienceSuq[EnemyIdx];
        EnemyIdx += 1;
        int idx = ShowNewAudience(audience);
        audience.BindViewIdx = idx;
        TargetList.Add(audience);
        return idx;
    }

    public void ShowRandomAudience(bool proAndSuf = false)
    {
        
        ZhiboAudience audience = new ZhiboAudience();
        int[] sufTmpHp = { 0, 0, 0, 0, 0, 0 };
        int[] proTmpHp = { 0, 0, 0, 0, 0, 0 };
        int tmpWaitTime = 0;
        int[] oriHp = { 1, 0, 0, 0, 0, 0 };
        if (proAndSuf)
        {
            TVSuffix appliedSuf = applySuffixEffect();
            TVProfix appliedPro = applyProfixEffect();
            sufTmpHp = loadSuffixEffect(appliedSuf);

            if(appliedPro!=null)
            {
                AudienceProfixEffect audienceProfixEffect = loadProfixEffect(appliedPro, oriHp);
                proTmpHp = audienceProfixEffect.hp;
                tmpWaitTime = audienceProfixEffect.waitTime;
                audience.tvProfix = appliedPro;
            }
            
            audience.tvSuffix = appliedSuf;   
        }
        {
            audience.Type = eAudienceType.Normal;
            audience.MaxReq[0] = oriHp[0] + proTmpHp[0] + sufTmpHp[0];
            audience.MaxReq[1] = oriHp[1] + proTmpHp[1] + sufTmpHp[1];
            audience.MaxReq[2] = oriHp[2] + proTmpHp[2] + sufTmpHp[2];
            audience.MaxReq[3] = oriHp[3] + proTmpHp[3] + sufTmpHp[3];
            audience.MaxReq[4] = oriHp[4] + proTmpHp[4] + sufTmpHp[4];
            audience.MaxReq[5] = oriHp[5] + proTmpHp[5] + sufTmpHp[5];
            audience.state = eAudienceState.Normal;
        }
        
        int idx = ShowNewAudience(audience);
        audience.BindViewIdx = idx;
        TargetList.Add(audience);
    }



    public void AudienceLeave(ZhiboAudience audience)
    {


        if(audience.BlackHp > 0)
        {
            audience.state = eAudienceState.None;
            LittleTvList[audience.BindViewIdx].Disappear();
            TargetList.Remove(audience);
            CalculateAura();
            return;
        }

        float stfRate = audience.ReqRate();

        int r = Random.Range(0, 100);
        if (r >  (1 - stfRate) * 100)
        //if(stfRate < 0.6f)
        {
            //LittleTvList[audience.BindViewIdx].ConvertToHeizi();
            //audience.ConvertToHeizi();
            //return;
        }

        audience.state = eAudienceState.None;
        LittleTvList[audience.BindViewIdx].Disappear();
        TargetList.Remove(audience);
        if (stfRate >= 0.9f)
        {
            addExtraHp(1);
        }
        else
        {
            gameMode.AddHp(-3);
        }

        for(int i = 0; i < audience.Skills.Count; i++)
        {
            if(audience.Skills[i].skillType == eZhiboAudienceSkillType.Punish)
            {
                eAudiencePunishType type = (eAudiencePunishType)audience.Skills[i].effectId;
                switch (type)
                {
                    case eAudiencePunishType.Damage:
                        int damage = int.Parse(audience.Skills[i].effectString);
                        gameMode.AddHp(damage);
                        break;
                    default:
                        break;
                }
            }
        }
        CalculateAura();
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
                    AudienceLeave(TargetList[i]);

                }
                else
                {
                    LittleTvList[TargetList[i].BindViewIdx].UpdateTurnLeft();
                }
            }
        }
    }


    public void AudienceBecomeHeizi()
    {
        
    }


    public void AudienceTurnEffect()
    {
        List<ZhiboAudienceSkill> skillsToHandle = new List<ZhiboAudienceSkill>();
        for (int i = 0; i < TargetList.Count; i++)
        {
            if (TargetList[i].state == eAudienceState.None)
            {
                continue;
            }

            if (TargetList[i].BindViewIdx == -1)
            {
                continue;
            }
            for(int j = 0; j < TargetList[i].Skills.Count; j++)
            {
                if(TargetList[i].Skills[j].skillType == eZhiboAudienceSkillType.TurnEffect)
                {
                    eAudienceTurnEffectType type = (eAudienceTurnEffectType)TargetList[i].Skills[j].effectId;
                    switch (type)
                    {
                        case eAudienceTurnEffectType.AddKoucaiReq:
                            //决定怎么显示需求
                            break;
                        default:
                            break;

                    }
                    skillsToHandle.Add(TargetList[i].Skills[j]);
                }
             }
        }
        
    }

    

    public void AudienceCauseDamage()
    {
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.None)
            {
                continue;
            }

            if(TargetList[i].BindViewIdx == -1)
            {
                continue;
            }

            int damage = 0;
            for(int j=0;j< TargetList[i].NowReq.Length; j++)
            {
                damage += TargetList[i].MaxReq[i] - TargetList[i].NowReq[j];
            }
            gameMode.AddHp(-damage);


            gameMode.mUICtrl.ShowDamageEffect(LittleTvList[TargetList[i].BindViewIdx].transform.position);
        }
    }

    public void ShowAudienceDamagedEffect()
    {

    }

    public void EmptimizeLittleTV(ZhiboLittleTV tv)
    {
        int idx = LittleTvList.IndexOf(tv);
        if (idx < 0)
        {
            return;
        }
        if (EmptyTVList.Contains(idx))
        {
            return;
        }
        WaitingReturnList.Add(idx);
    }

    public void ReturnWaitingTVs()
    {
        EmptyTVList.AddRange(WaitingReturnList);
        WaitingReturnList.Clear();
    }


    //public void GenHeifen(int hp, int idx = -1)
    //{
    //    ZhiboAudience audience = new ZhiboAudience();
    //    {
    //        audience.Type = eAudienceType.Heizi;
    //        audience.BlackHp = hp;
    //        audience.state = eAudienceState.Normal;
    //    }
    //    int realIdx = -1;
    //    if(idx >= 0)
    //    {
    //        realIdx = ShowNewAudienceAtPos(audience,idx);
    //    }
    //    if (realIdx == -1)
    //    {
    //        realIdx = ShowNewAudience(audience);
    //    }

    //    audience.BindViewIdx = idx;
    //    TargetList.Add(audience);
    //}


    public int ShowNewAudienceAtPos(ZhiboAudience audience, int slotIdx)
    {
        if (LittleTvList[slotIdx].TargetAudience != null)
        {
            return -1;
        }
        if (EmptyTVList.Contains(slotIdx))
        {
            EmptyTVList.Remove(slotIdx);
        }
        LittleTvList[slotIdx].InitLittleTvView(audience);

        audience.state = eAudienceState.Normal;
        return slotIdx;
    }

    public int ShowNewAudience(ZhiboAudience audience)
    {
        if (EmptyTVList.Count == 0)
        {
            ReturnWaitingTVs();

        }
        if (EmptyTVList.Count == 0)
        {
            return -1;
        }

        int idx = Random.Range(0, EmptyTVList.Count);
        

        idx = EmptyTVList[idx];

        EmptyTVList.Remove(idx);

        LittleTvList[idx].InitLittleTvView(audience);

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
        if(tvView.TargetAudience == null)
        {
            return;
        }
        tokenDetail.root.gameObject.SetActive(true);
        string txt = "";
        for(int i=0;i< tvView.TargetAudience.Skills.Count; i++)
        {
            txt += GenSkillDesp(tvView.TargetAudience.Skills[i]);
            //txt += tvView.TargetAudience.Skills[i].type.ToString();
            //txt += "effect: " + tvView.TargetAudience.Aura[i].level;
            txt += "\n";
        }

        if (txt == "")
        {
            txt = "无";
        }
        tokenDetail.Content.text = txt;

        tokenDetail.root.position = tvView.view.TokenInfo.transform.position + new Vector3(0.15f, 0f, 0);
    }

    private string GenSkillDesp(ZhiboAudienceSkill skill)
    {

        if(skill.effectString == null || skill.effectString == "")
        {
            return "";
        }
        Dictionary<int,string> subDict = SkillDespDict[skill.skillType];
        if (subDict.ContainsKey(skill.effectId))
        {
            string f = subDict[skill.effectId];
            string desp = string.Format(f,skill.effectString);
            return desp;
        }
        return skill.skillType.ToString() + skill.effectId + skill.effectString;
        //string desp = skill.skillType.ToString() + skill.effectId + skill.effectString;
        //return desp;

    }

    public void HideTokenDetail()
    {
        tokenDetail.root.gameObject.SetActive(false);
    }

    public void CalculateAura()
    {
        int totalLess = 0;
        for(int i = 0; i < TargetList.Count; i++)
        {
            if(TargetList[i].state == eAudienceState.Normal && TargetList[i].Skills.Count > 0)
            {
                for(int j=0;j< TargetList[i].Skills.Count; j++)
                {
                    if(TargetList[i].Skills[j].skillType != eZhiboAudienceSkillType.Aura)
                    {
                        continue;
                    }
                    eAudienceAuraType type = (eAudienceAuraType)TargetList[i].Skills[j].effectId;
                    switch (type)
                    {
                        case eAudienceAuraType.LessScore:
                            int nowLevel = int.Parse(TargetList[i].Skills[j].effectString);
                            totalLess = Mathf.Max(nowLevel, totalLess);
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        audienceAuraBuff.ScoreLess = totalLess;
    }

    //Surfix
    public bool loadSuffix()
    {
        
        SuffixList.loadSuffix();
        return SuffixList.suffixs.Count > 0;
    }

    public TVSuffix applySuffixEffect(int idx = 0, bool rand = true)
    {
        TVSuffix appliedSuf;
        if (SuffixList == null || SuffixList.suffixs.Count == 0)
        {
            return null;
        }

        if (rand)
        {
            appliedSuf = randomSuffixEffect();
        } else
        {
            appliedSuf = setSuffixEffect(idx);
        }
        return appliedSuf;
    }
    public int[] loadSuffixEffect(TVSuffix appliedSuf) {
        int[] SuffixHpTmp = { 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < appliedSuf.effects.Count; i++) { 
            int[] tmpHp = handOneSuffixEffect(appliedSuf.effects[i],appliedSuf.values[i]);
            for (int t = 0; t<6; t++)
            {
                SuffixHpTmp[t] += tmpHp[t];
            }
        }
        return SuffixHpTmp;
    }

    public TVSuffix randomSuffixEffect()
    {
        int idx = Random.Range(0, SuffixList.suffixs.Count);
        return setFixedSuffixEffect(idx);
    }

    public TVSuffix setSuffixEffect(int idx)
    {
        int prop = SuffixList.suffixs[idx].probability;
        int randValue = Random.Range(0, 100);
        if (randValue <= prop)
        {
            return SuffixList.suffixs[idx];
        }
        return null;
    }

    public TVSuffix setFixedSuffixEffect(int idx)
    {
        return SuffixList.suffixs[idx];
    }

    public int[] handOneSuffixEffect(TVSuffixEffect efct, int value)
    {
        int[] hpTmp = { 0, 0, 0, 0, 0, 0 };
        switch (efct)
        {
            case TVSuffixEffect.none:
                break;
            case TVSuffixEffect.requestKoucai:
                hpTmp[1] += value;
                //Koucai;
                break;
            case TVSuffixEffect.requestCaiyi:
                hpTmp[2] += value;
                //Koucai;
                break;
            case TVSuffixEffect.requestKangya:
                hpTmp[3] += value;
                //Koucai;
                break;
            case TVSuffixEffect.requestWaiguan:
                hpTmp[4] += value;
                //Koucai;
                break;
            case TVSuffixEffect.requestJishu:
                hpTmp[5] += value;
                //Koucai;
                break;
            case TVSuffixEffect.requestAll:
                for(int t = 1; t<6; t++)
                {
                    hpTmp[t] += value;
                }
                break;
        }
        return hpTmp;
    }

    //Profix
    public bool loadProfix()
    {

        ProfixList.loadProfix();
        return ProfixList.profixs.Count > 0;
    }

    public TVProfix applyProfixEffect(int idx = 0, bool rand = true)
    {
        TVProfix appliedPro;
        if (ProfixList == null || ProfixList.profixs.Count == 0)
        {
            return null;
        }

        if (rand)
        {
            appliedPro = randomProfixEffect();
        }
        else
        {
            appliedPro = setFixedProfixEffect(idx);
        }
        return appliedPro;
    }
    public AudienceProfixEffect loadProfixEffect(TVProfix appliedPro, int[] curReq)
    {
        
        AudienceProfixEffect audienceProfixEffect = new AudienceProfixEffect();
        for (int i = 0; i < appliedPro.effects.Count; i++)
        {
            AudienceProfixEffect ape = handOneProfixEffect(appliedPro.effects[i], appliedPro.values[i], curReq);
            for(int t = 0; t<6; t++)
            {
                audienceProfixEffect.hp[t] += ape.hp[t];
            }
            audienceProfixEffect.waitTime += ape.waitTime;
        }
        return audienceProfixEffect;
    }

    public TVProfix randomProfixEffect()
    {
        int idx = Random.Range(0, ProfixList.profixs.Count);
        return setProfixEffect(idx);
    }

    public TVProfix setProfixEffect(int idx)
    {
        int prop = ProfixList.profixs[idx].probability;
        int randValue = Random.Range(0, 100);
        if(randValue<=prop)
        {
            return ProfixList.profixs[idx];
        }
        return null;   
    }

    public TVProfix setFixedProfixEffect(int idx)
    {
        return ProfixList.profixs[idx];
    }

    public AudienceProfixEffect handOneProfixEffect(TVProfixEffect efct, int value, int[] curReq)
    {
        AudienceProfixEffect audienceProfixEffect = new AudienceProfixEffect();
        int[] hpTmp = { 0, 0, 0, 0, 0, 0 };
        int waitTimeTmp = 0;
        switch (efct)
        {
            case TVProfixEffect.none:
                break;
            case TVProfixEffect.addRequest:
                //需求翻倍或减半;
                for(int i = 0; i<6; i++)
                {
                    if(value>=1)
                    {
                        hpTmp[i] = curReq[i] * (value - 1);
                    }
                    else
                    {
                        hpTmp[i] = -curReq[i] * (1 - value);
                    }
                }
                break;
            case TVProfixEffect.extraRequest:
                int idx = Random.Range(0, 6);
                hpTmp[idx] += value;
                //Koucai;
                break;
            case TVProfixEffect.addWaitTime:
                waitTimeTmp += value;
                break;
        }
        for(int i = 0; i<6; i++)
        {
            //Debug.Log("cur->"+i+":"+curHp[i]);
            //Debug.Log("then->"+i+":"+hpTmp[i]);
        }

        audienceProfixEffect.hp = hpTmp;
        audienceProfixEffect.waitTime = waitTimeTmp;

        return audienceProfixEffect;
    }

    

}
