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
    Attracted,
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
    public static int MAX_GEM_NUM = 16;
    public int Level = 1;
    public int HpModeIdx = 0;

    public int LastTurn = 3;

    //即时制属性
    public int TimeLeft;

    public List<ZhiboAudienceSkill> Skills = new List<ZhiboAudienceSkill>();

    //public List<ZhiboAudienceBonus> Bonus = new List<ZhiboAudienceBonus>();
    //public List<ZhiboAudienceAura> Aura = new List<ZhiboAudienceAura>();


    //run time property
    public eAudienceState state = eAudienceState.None; 
    public int BindViewIdx = -1;

    public int[] NowManyi = new int[(int)eAudienceHpType.Max];
    public int[] GemHp = new int[(int)eAudienceHpType.Max];
    public int[] GemMaxHp = new int[(int)eAudienceHpType.Max];
    public int BlackHp = 0;
    public int MaxBlackHp = 0;
    public int AttractLeftTurn = 0;
    public float NowScore = 0;
    public List<AudienceToken> Tokens = new List<AudienceToken>();


    public int[] preHp = new int[(int)eAudienceHpType.Max];
    public float preScore = 0;

    public float GetBaseBonus()
    {
        return Level * 10;
    }

    public void addExtraHp(int type, int amount)
    {
        int totalHp = 0;
        for(int i = 0; i < GemMaxHp.Length; i++)
        {
            totalHp += GemMaxHp[i];
        }
        if(totalHp >= MAX_GEM_NUM)
        {
            return;
        }
        GemMaxHp[type] += 1;
        GemHp[type] += 1;
        
    }

    public int HpChangeNum()
    {
        int ret = 0;
        for (int i = 0; i < GemHp.Length; i++)
        {
            ret += GemHp[i]-preHp[i];
        }
        return ret;
    }

    public float HpRate()
    {
        int maxHp = 0;
        for(int i=0;i< GemMaxHp.Length; i++)
        {
            maxHp += GemMaxHp[i];
        }

        int hp = 0;
        for (int i = 0; i < GemHp.Length; i++)
        {
            hp += GemHp[i];
        }
        if (hp == 0)
        {
            return 0;
        }

        return hp * 1.0f / maxHp;
    }

    public void AddScore(float amount)
    {
        preScore = NowScore;
        NowScore += amount;
    }

    public bool isDead()
    {
        bool ret = true;
        if(BlackHp > 0)
        {
            return false;
        }
        for (int i = 0; i < GemHp.Length; i++)
        {
            if (GemHp[i] > 0)
            {
                ret = false;
                break;
            }
        }
        return ret;
    }

    public bool ApplyDamage(int[] inputDamage)
    {
        if(Type == eAudienceType.Heizi)
        {
            return false;
        }

        int[] damage = new int[6];

        for(int i = 0; i < 6; i++)
        {
            damage[i] = inputDamage[i];
        }

        for (int i = 0; i < 6; i++)
        {
            preHp[i] = GemHp[i];
        }

        int damageOverflow = 0;
        bool CauseDamage = false;
        for(int i = 1; i < damage.Length; i++)
        {
            if (damage[i] > 0)
             {
                if(GemHp[i] > 0)
                {
                    CauseDamage = true;
                    GemHp[i] -= damage[ i];
                    if(GemHp[i] < 0)
                    {
                        damageOverflow += (-GemHp[i]);
                        GemHp[i] = 0;
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
                if (GemHp[i] > 0)
                {
                    CauseDamage = true;
                    if(damage[0] >= GemHp[i])
                    {
                        damage[0] -= GemHp[i];
                        GemHp[i] = 0;
                    }
                    else
                    {
                        GemHp[i] -= damage[0];
                        damage[0] = 0;
                    }

                }
            }
        }

        damageOverflow += damage[0];
        if (GemHp[0] > 0 )
        {
            if(damageOverflow > 0)
            {
                GemHp[0] -= damageOverflow;
                if(GemHp[0] < 0)
                {
                    GemHp[0] = 0;
                }
                CauseDamage = true;
            }
        }


        return CauseDamage;
    }

    public void Attracted()
    {
        if(state == eAudienceState.Attracted)
        {
            return;
        }

        state = eAudienceState.Attracted;
        AttractLeftTurn = 1;
    }

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
                if (GemHp[colors[i]] == 0)
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
                if (GemHp[colors[i]] > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}


public class ZhiboAudienceAsset
{

}