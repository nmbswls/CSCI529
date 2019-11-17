using UnityEngine;
using System.Collections;

public enum eAudienceType
{
    Normal,
    Good,
}


public class ZhiboAudience
{
    public eAudienceType Type;
    public int Level = 1;

    public int state = 0; //0 normal 1 attracted
    public int[] GemHp = new int[6];
    public int[] GemMaxHp = new int[6];

    public int AttractLeftTurn = 0;

    public float GetBaseBonus()
    {
        return Level * 10;
    }


    public bool isDead()
    {
        bool ret = true;
        for(int i = 0; i < GemHp.Length; i++)
        {
            if (GemHp[i] > 0)
            {
                ret = false;
                break;
            }
        }
        return ret;
    }

    public bool ApplyDamage(int[] damage)
    {
        int damageOverflow = 0;
        bool CauseDamage = false;
        for(int i = 1; i < damage.Length; i++)
        {
            if (damage[i] > 0)
            {
                if(GemHp[i] > 0)
                {
                    CauseDamage = true;
                    GemHp[i] -= damage[i];
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
                        GemHp[i] -= damage[i];
                        damage[0] = 0;
                    }

                }
            }
        }

        damageOverflow += damage[0];
        if (GemHp[0] > 0)
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
        if(state == 1)
        {
            return;
        }
        Debug.Log("attracted");
        state = 1;
        AttractLeftTurn = 2;
    }
}


public class ZhiboAudienceAsset
{

}