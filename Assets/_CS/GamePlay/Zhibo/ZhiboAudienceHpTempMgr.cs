using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OuterTurnAudeinceReqTempControl
{
    public int OuterTurn = 1;
    public OuterTurnAudeinceReqTempControl(int OuterTurn)
    {
        this.OuterTurn = OuterTurn;
    }

    public Dictionary<int, AudienceReqDistributionInfo> inTurnAdd = new Dictionary<int, AudienceReqDistributionInfo>();
    public Dictionary<int, AudienceReqDistributionInfo> inTurnRemove = new Dictionary<int, AudienceReqDistributionInfo>();

    public void InsertAudienceReqToBeAdded(AudienceReqDistributionInfo AddIn)
    {
        if (!inTurnAdd.ContainsKey(AddIn.Turn))
        {
            inTurnAdd.Add(AddIn.Turn, AddIn);
        }
        else
        {
            inTurnAdd[AddIn.Turn].append(AddIn);
        }

    }

    public void InsertAudienceReqToBeRemoved(AudienceReqDistributionInfo RemovedOut)
    {
        if (!inTurnRemove.ContainsKey(RemovedOut.Turn))
        {
            inTurnRemove.Add(RemovedOut.Turn, RemovedOut);
        }
        else
        {
            inTurnRemove[RemovedOut.Turn].append(RemovedOut);
        }
    }

}

public class ZhiboAudienceHpTempMgr
{
    Dictionary<int, OuterTurnAudeinceReqTempControl> OuterTurnAddedAudiReqList = new Dictionary<int, OuterTurnAudeinceReqTempControl>();
    Dictionary<int, OuterTurnAudeinceReqTempControl> OuterTurnRemovedAudiReqList = new Dictionary<int, OuterTurnAudeinceReqTempControl>();

    public AudienceReqDistributionInfo loadedReqDistributions = new AudienceReqDistributionInfo(0);

    Dictionary<int, Dictionary<int, AudienceReqDistributionInfo>> RoundDistributions;

    bool isLoaded = false;

    public ZhiboGameMode gameMode;

    public IResLoader mResLoader;

    public ZhiboAudienceHpTempMgr(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        InitRes();
    }

    public void InitRes()
    {
        loadHpTemplate();
    }

    public void loadHpTemplate()
    {
        AudienceReqContentList audienceReqExcel = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<AudienceReqContentList>("AudienceReq/AudienceReqContentList", false);

        //load HpTemplate
        {
            List<AudienceHpTemplateLoader> loader = audienceReqExcel.HpTemplates;
            foreach (AudienceHpTemplateLoader auhp in loader)
            {
                float[] tmpHp = {
                    (float)(auhp.Gem1) / 100,
                    (float)(auhp.Gem2) / 100,
                    (float)(auhp.Gem3) / 100,
                    (float)(auhp.Gem4) / 100,
                    (float)(auhp.Gem5) / 100,
                    (float)(auhp.Gem6) / 100,
                };
                AudienceReqDistributionInfo tmpARD = new AudienceReqDistributionInfo(auhp.EffectInnerTurnlUnlock);
                tmpARD.Distributions.Add(tmpHp);

                //放到对应外回合 + 对应内回合 要加入 的模板list中
                if (!OuterTurnAddedAudiReqList.ContainsKey(auhp.EffectOuterTurnlUnlock))
                {
                    OuterTurnAddedAudiReqList.Add(auhp.EffectOuterTurnlUnlock, new OuterTurnAudeinceReqTempControl(auhp.EffectOuterTurnlUnlock));
                }
                OuterTurnAddedAudiReqList[auhp.EffectOuterTurnlUnlock].InsertAudienceReqToBeAdded(tmpARD);

                //放到对应外回合 + 对应内回合 要退出 的模板list中
                if (!OuterTurnRemovedAudiReqList.ContainsKey(auhp.EffectOuterTurnlRemove))
                {
                    OuterTurnRemovedAudiReqList.Add(auhp.EffectOuterTurnlRemove, new OuterTurnAudeinceReqTempControl(auhp.EffectOuterTurnlRemove));
                }
                OuterTurnRemovedAudiReqList[auhp.EffectOuterTurnlRemove].InsertAudienceReqToBeRemoved(tmpARD);
            }

            //OuterTurnAddedAudiReqList
        }
        //sortTemplatePool();
        recordLoadedHpTemplate();
    }

    public void sortTemplatePool()
    {
        foreach (KeyValuePair<int, OuterTurnAudeinceReqTempControl> otaar in OuterTurnAddedAudiReqList)
        {
            otaar.Value.inTurnAdd = otaar.Value.inTurnAdd.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            otaar.Value.inTurnRemove = otaar.Value.inTurnRemove.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }

        foreach (KeyValuePair<int, OuterTurnAudeinceReqTempControl> otrar in OuterTurnRemovedAudiReqList)
        {
            otrar.Value.inTurnAdd = otrar.Value.inTurnAdd.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            otrar.Value.inTurnRemove = otrar.Value.inTurnRemove.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }
        OuterTurnAddedAudiReqList = OuterTurnAddedAudiReqList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        OuterTurnRemovedAudiReqList = OuterTurnRemovedAudiReqList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
    }

    public void recordLoadedHpTemplate()
    {
        AudienceReqDistributionInfo ARDI = new AudienceReqDistributionInfo(0);
        for (int i = 0; i < 30; i++)
        {
            Dictionary<int, AudienceReqDistributionInfo> InnerReqDistribution = new Dictionary<int, AudienceReqDistributionInfo>();
            for (int j = 0; j < 12; j++)
            {
                ARDI.append(OuterTurnAddedAudiReqList[i].inTurnAdd[j]);
                ARDI.remove(OuterTurnRemovedAudiReqList[i].inTurnRemove[j]);
                InnerReqDistribution[j] = new AudienceReqDistributionInfo(j, ARDI);
            }
            RoundDistributions[i] = InnerReqDistribution;
        }
    }

    public AudienceReqDistributionInfo GetTurnBaseReq(int outer, int inner)
    {
        return RoundDistributions[outer][inner];
    }


    ////外部血量template 直接由现有的空间里边判断
    //public int[] GetLoadedBaseReq(int level)
    //{
    //    float[] rate = GetLoadedReqDistribution();
    //}

    ////外部血量template比例
    //public float[] GetLoadedReqDistribution(bool innerTurnBased = false)
    //{
    //    //
    //    if (innerTurnBased)
    //    {

    //    }
    //    else
    //    {
    //        List<float[]> rates = loadedReqDistributions.Distributions;
    //        int randI = Random.Range(0, rates.Count);
    //        float[] rate = rates[randI];
    //        return rate;
    //    }
    //}

    public int[] GetHpTemplate(int level, float[] rate)
    {
        int hpNum = 1 + level;
        int hpLeft = hpNum;
        int[] ret = new int[6];

        if (rate[0] > 0f)
        {
            ret[0] = (int)Mathf.Round(hpNum * rate[0]);
            if (ret[0] > hpNum)
            {
                ret[0] = hpNum;
            }
            hpLeft -= ret[0];
        }

        List<int> left = new List<int>();
        for (int i = 1; i <= 5; i++)
        {
            left.Add(i);
        }
        int rateIdx = 1;
        while (rateIdx < 6)
        {
            if (rate[rateIdx] == 0)
            {
                break;
            }
            int nextAttr = left[Random.Range(0, left.Count)];
            int roundValue = (int)Mathf.Round(hpNum * rate[rateIdx]);
            if (hpLeft < roundValue)
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
            for (int i = 1; i < 6; i++)
            {
                if (ret[i] > 0)
                {
                    DistributedPos.Add(i);
                }
            }
            if (DistributedPos.Count == 0)
            {
                for (int i = 1; i < 6; i++)
                {
                    DistributedPos.Add(i);
                }
            }

            for (int i = 0; i < hpLeft; i++)
            {
                int rIdx = Random.Range(0, DistributedPos.Count);
                ret[DistributedPos[rIdx]] += 1;
            }

        }

        return ret;
    }
}

