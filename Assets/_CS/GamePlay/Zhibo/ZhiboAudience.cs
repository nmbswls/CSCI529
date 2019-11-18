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

[System.Serializable]
public class ZhiboAudienceBonus
{
    public bool isGood;
    public eAudienceBonusType Type;
    public string effectString;
}

[System.Serializable]
public class ZhiboAudienceAura
{
    public eAudienceAuraType type;
    public int level;
}

public class ZhiboAudience
{
    //static property
    public eAudienceType Type;
    public int Level = 1;
    public int HpModeIdx = 0;

    public int LastTurn = 0;

    public List<ZhiboAudienceBonus> Bonus = new List<ZhiboAudienceBonus>();
    public List<ZhiboAudienceAura> Aura = new List<ZhiboAudienceAura>();


    //run time property
    public eAudienceState state = eAudienceState.None; 
    public int BindViewIdx = -1;
    public int[] GemHp = new int[6];
    public int[] GemMaxHp = new int[6];
    public int BlackHp = 0;
    public int MaxBlackHp = 0;
    public int AttractLeftTurn = 0;
    public float NowScore = 0;
    public List<AudienceToken> Tokens = new List<AudienceToken>();


    public int[] preHp = new int[6];

    public float GetBaseBonus()
    {
        return Level * 10;
    }


    public void AddScore(float amount)
    {
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
}


public class ZhiboAudienceAsset
{

}