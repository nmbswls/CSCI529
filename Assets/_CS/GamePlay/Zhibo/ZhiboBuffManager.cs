using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class ZhiboBuffManager
{
    //存储所有按卡计次的buff
    public List<ZhiboBuff> CardCountedBuffs = new List<ZhiboBuff>();


    public int[] BuffAddValue = new int[5];
    public int[] BuffAddPercent = new int[5];

    public float GenScoreExtraRate = 0;
    
    public float DanmuExtraReward = 0;
    public float GainScorePerSec = 0;
    public float ExtraSuccessPossibility = 0;
    public int BadRateDiff = 0;
    public bool AvoidBadDanmu = false;
    public float ExtraChenggonglv = 0;

    public float GenStatusExtraRate = 0;

    private IResLoader mResLoader;
    private ZhiboGameMode gameMode;

    //(Colors)Enum.Parse(typeof(Colors), "Red")
    private Dictionary<string, string> BuffDesp = new Dictionary<string, string>();


    public void TickSec()
    {
        bool changed = false;
        for(int i = gameMode.state.ZhiboBuffs.Count - 1; i >= 0; i--)
        {
            if (gameMode.state.ZhiboBuffs[i].isBasedOn(eBuffLastType.TIME_BASE))
            {
                gameMode.state.ZhiboBuffs[i].LeftTime -= 1f;
                if(gameMode.state.ZhiboBuffs[i].LeftTime < 0)
                {
                    changed = true;
                    RemoveBuff(gameMode.state.ZhiboBuffs[i]);
                }
            }
        }
        if (changed)
        {
            CalculateBuffExtras();
        }
        if (GainScorePerSec > 0)
        {
            gameMode.GainScore(GainScorePerSec);
        }
    }


    public string GetBuffDesp(eBuffType buffType)
    {
        if (BuffDesp.ContainsKey(buffType.ToString()))
        {
            return BuffDesp[buffType.ToString()];
        }
        return "丢失";
    }
    public ZhiboBuffManager(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        LoadBuff();
    }

    private void LoadBuff()
    {
        BuffDesp.Add(eBuffType.Meili_Add.ToString(), "增加{0}点魅力");
        BuffDesp.Add(eBuffType.Tili_Add.ToString(), "增加{0}点体力");
        BuffDesp.Add(eBuffType.Koucai_Add.ToString(), "增加{0}点口才");
        BuffDesp.Add(eBuffType.Jiyi_Add.ToString(), "增加{0}点技艺");
        BuffDesp.Add(eBuffType.Fanying_Add.ToString(), "增加{0}点反应");

        BuffDesp.Add(eBuffType.Meili_Add_P.ToString(), "增加百分比{0}的魅力");
        BuffDesp.Add(eBuffType.Tili_Add_P.ToString(), "增加百分比{0}的体力");
        BuffDesp.Add(eBuffType.Koucai_Add_P.ToString(), "增加百分比{0}的口才");
        BuffDesp.Add(eBuffType.Jiyi_Add_P.ToString(), "增加百分比{0}的技艺");
        BuffDesp.Add(eBuffType.Fanying_Add_P.ToString(), "增加百分比{0}的反应");
    }


    public void GenBuff(ZhiboBuffInfo cinfo)
    {
        ZhiboBuff buff = gameMode.mUICtrl.GenBuff();
        buff.Init(gameMode, cinfo);
        gameMode.state.ZhiboBuffs.Add(buff);
        CalculateBuffExtras();
    }

    public List<ZhiboBuff> CheckValidBuff(CardInZhibo card)
    {
        List<ZhiboBuff> ret = new List<ZhiboBuff>();
        for (int i = 0; i < CardCountedBuffs.Count; i++)
        {
            if (CardCountedBuffs[i].WillAffectCard(card))
            {
                ret.Add(CardCountedBuffs[i]);
            }
        }
        return ret;
    }

    public void RemoveBuff(ZhiboBuff obj)
    {
        gameMode.state.ZhiboBuffs.Remove(obj);
        mResLoader.ReleaseGO("Zhibo/Buff", obj.gameObject);
        //CalculateBuffExtras();
    }

    public void CalculateBuffExtras()
    {
        for (int i = 0; i < 5; i++)
        {
            BuffAddValue[i] = 0;
            BuffAddPercent[i] = 0;
        }
        CardCountedBuffs.Clear();
        GenScoreExtraRate = 0;
        DanmuExtraReward = 0;
        GainScorePerSec = 0;
        ExtraSuccessPossibility = 0;
        BadRateDiff = 0;

        AvoidBadDanmu = false;

        for (int i=0;i< gameMode.state.ZhiboBuffs.Count; i++)
        {
            ZhiboBuff buff = gameMode.state.ZhiboBuffs[i];
            switch (buff.bInfo.BuffType)
            {
                case eBuffType.Meili_Add:
                    BuffAddValue[0] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Meili_Add_P:
                    BuffAddPercent[0] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Tili_Add:
                    BuffAddValue[1] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Tili_Add_P:
                    BuffAddPercent[1] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Koucai_Add:
                    BuffAddValue[2] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Koucai_Add_P:
                    BuffAddPercent[2] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Fanying_Add:
                    BuffAddValue[3] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Fanying_Add_P:
                    BuffAddPercent[3] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Jiyi_Add:
                    BuffAddValue[4] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Jiyi_Add_P:
                    BuffAddPercent[4] += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Extra_Score_Rate:
                    GenScoreExtraRate += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Score_Per_Sce:
                    GainScorePerSec += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Extra_Neg_Level:
                    BadRateDiff += buff.bInfo.BuffLevel;
                    break;
                
                case eBuffType.Success_Rate_Multi:
                    ExtraChenggonglv += buff.bInfo.BuffLevel;
                    break;
                case eBuffType.Success_Rate_Max:
                    ExtraChenggonglv += 100;
                    break;
                default:
                    break;
            }
            if (buff.bInfo.AffectCard)
            {
                CardCountedBuffs.Add(buff);
            }
        }
    }
}
