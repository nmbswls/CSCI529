using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eAudienceType
{
    Normal,
    Good,
    Heizi,
}

public enum eAudienceState {
    None,
    Normal,
    //Attracted,
}

public enum eAudienceBonusType
{
    None = 0,
    AddHp,
    Damage,
    Dual,
    Discard,
    Aoe,
    Score,
    Max,
}

public enum eAudienceAuraType
{
    LessScore,
}
public enum eAudiencePunishType
{
    None = 0,
    Damage,
    Discard,
    Score,
    Max,
}
public enum eAudienceTurnEffectType
{
    None = 0,
    AddKoucaiReq,
}

public enum eAudienceHpType
{
    Any = 0,
    T1,
    T2,
    T3,
    T4,
    T5,
    Max
}

public enum eZhiboAudienceSkillType
{
    Bonus,
    Aura,
    Punish,
    TurnEffect,
}
//[System.Serializable]
//public class ZhiboAudienceBonus
//{
//    public bool isGood;
//    public eAudienceBonusType Type;
//    public string effectString;
//}

//[System.Serializable]
//public class ZhiboAudienceAura
//{
//    public eAudienceAuraType type;
//    public int level;
//}
[System.Serializable]
public class ZhiboAudienceSkill
{
    public eZhiboAudienceSkillType skillType;
    public int effectId;
    public string effectString;

    public ZhiboAudienceSkill(int type)
    {
        this.skillType = (eZhiboAudienceSkillType)type;
    }

    public ZhiboAudienceSkill(eZhiboAudienceSkillType type)
    {
        this.skillType = type;
    }

}


public class ZhiboAudience
{
    //static property
    public eAudienceType Type;
    public static int MAX_REQ_NUM = 16;
    public int Level = 1;
    public int HpModeIdx = 0;

    public int LastTurn = 3;

    //即时制属性
    public float OriginTimeLast;
    public float TimeLeft;

    public List<ZhiboAudienceSkill> Skills = new List<ZhiboAudienceSkill>();

    //public List<ZhiboAudienceBonus> Bonus = new List<ZhiboAudienceBonus>();
    //public List<ZhiboAudienceAura> Aura = new List<ZhiboAudienceAura>();


    //run time property
    public eAudienceState state = eAudienceState.None; 
    public int BindViewIdx = -1;

    public int[] NowReq = new int[(int)eAudienceHpType.Max];
    public int[] MaxReq = new int[(int)eAudienceHpType.Max];
    //public int[] GemHp = new int[(int)eAudienceHpType.Max];
    //public int[] GemMaxHp = new int[(int)eAudienceHpType.Max];
    public int BlackHp = 0;
    public int MaxBlackHp = 0;
    public int AttractLeftTurn = 0;
    public float NowScore = 0;
    public List<AudienceToken> Tokens = new List<AudienceToken>();


    public int[] preReq = new int[(int)eAudienceHpType.Max];
    public float preScore = 0;

    //prefix & Suffix

    public TVPrefix tvPrefix;
    public TVSuffix tvSuffix;

    public int probabilityOfPrefix = 0;
    public int probabilityOfSuffix = 0;


    public float GetBaseBonus()
    {
        return Level * 10;
    }

    public void ConvertToHeizi()
    {
        int heiziHp = 0;
        for(int i = 0; i < (int)eAudienceHpType.Max; i++)
        {
            int cha = MaxReq[i] - NowReq[i];
            MaxReq[i] = 0;
            NowReq[i] = 0;
            heiziHp += cha;
        }
        BlackHp = heiziHp;
        OriginTimeLast = -1;
        TimeLeft = -1;
        LastTurn = 2;
    }
    public void addExtraHp(int type, int amount)
    {
        int totalReq = 0;
        for(int i = 0; i < MaxReq.Length; i++)
        {
            totalReq += MaxReq[i];
        }
        if(totalReq >= MAX_REQ_NUM)
        {
            return;
        }
        MaxReq[type] += amount;
    }

    public int ReqChangeNum()
    {
        int ret = 0;
        for (int i = 0; i < NowReq.Length; i++)
        {
            ret += NowReq[i]-preReq[i];
        }
        return ret;
    }

    public float ReqRate()
    {
        int totalReq = 0;
        for(int i=0;i< MaxReq.Length; i++)
        {
            totalReq += MaxReq[i];
        }

        int nowReq = 0;
        for (int i = 0; i < NowReq.Length; i++)
        {
            nowReq += NowReq[i];
        }
        if (nowReq == 0)
        {
            return 0;
        }

        return nowReq * 1.0f / totalReq;
    }

    public void AddScore(float amount)
    {
        preScore = NowScore;
        NowScore += amount;
    }

    public bool isSatisfied()
    {
        if(BlackHp > 0)
        {
            return false;
        }
        
        for (int i = 0; i < NowReq.Length; i++)
        {
            if (NowReq[i] < MaxReq[i])
            {
                return false;
            }
        }
        return true;
    }

    public bool ApplyDamage(int[] inputDamage)
    {
        if(Type == eAudienceType.Heizi)
        {
            return false;
        }

        int[] damage = new int[6];

        //profixlike
        int audienceLike = -1;
        int audienceDislike = -1;
        if (this.tvPrefix!=null)
        {
            audienceLike = (int)this.tvPrefix.like;
            audienceDislike = (int)this.tvPrefix.dislike;
            if (audienceLike == 0) audienceLike = -1;
            if (audienceDislike == 0) audienceDislike = -1;
        }
         

        for(int i = 0; i < 6; i++)
        {
            damage[i] = inputDamage[i];

            //prefix like and dislike check
            if(audienceLike == i)
            {
                if(damage[i] != 0)damage[i]++;
            } 
            if(audienceDislike == i)
            {
                if (damage[i] > 1) damage[i]--;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            preReq[i] = NowReq[i];
        }

        int damageOverflow = 0;
        bool CauseDamage = false;
        for(int i = 1; i < damage.Length; i++)
        {
            if (damage[i] > 0)
             {
                if(NowReq[i] < MaxReq[i])
                {
                    CauseDamage = true;
                    NowReq[i] += damage[ i];
                    if(NowReq[i] > MaxReq[i])
                    {
                        damageOverflow += (NowReq[i] - MaxReq[i]);
                        NowReq[i] = MaxReq[i];
                    }
                }
                else
                {
                    damageOverflow += damage[i];
                }
            }
        }
        for (int i = 1; i < damage.Length; i++)
        {
            if (damage[0] > 0)
            {
                if (NowReq[i] < MaxReq[i])
                {
                    CauseDamage = true;
                    if(damage[0] >= MaxReq[i] - NowReq[i])
                    {
                        damage[0] -= MaxReq[i] - NowReq[i];
                        NowReq[i] = MaxReq[i];
                    }
                    else
                    {
                        NowReq[i] += damage[0];
                        damage[0] = 0;
                    }

                }
            }
        }

        damageOverflow += damage[0];
        if (NowReq[0] < MaxReq[0])
        {
            if(damageOverflow > 0)
            {
                NowReq[0] += damageOverflow;
                if(NowReq[0] > MaxReq[0])
                {
                    NowReq[0] = MaxReq[0];
                }
                CauseDamage = true;
            }
        }


        return CauseDamage;
    }



    //public void Attracted()
    //{
    //    if(state == eAudienceState.Attracted)
    //    {
    //        return;
    //    }

    //    state = eAudienceState.Attracted;
    //    AttractLeftTurn = 1;
    //}

    public bool ApplyColorFilter(bool isAnd, List<int> colors)
    {
        if(colors == null)
        {
            return true;
        }
        if (isAnd)
        {

            for (int i=0;i< colors.Count; i++)
            {
                if (NowReq[colors[i]] == MaxReq[colors[i]])
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            for (int i = 0; i < colors.Count; i++)
            {
                if (NowReq[colors[i]] < MaxReq[colors[i]])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public string showProfixName()
    {
        if(tvPrefix==null)
        {
            return "某位";
        }
        return tvPrefix.name;
    }

    public string showSuffixName()
    {
        if (tvSuffix == null)
        {
            return "";
        }
        return tvSuffix.name;
    }
}


public class ZhiboAudienceAsset
{

}