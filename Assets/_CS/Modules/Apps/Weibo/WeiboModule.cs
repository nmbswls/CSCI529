using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeiboModule : ModuleBase, IWeiboModule
{
    private const int shuaTimeLimit = 3;     //每回合只有3次刷到牌的机会
    private int curShuaTime = 0;

    private bool isRealRandom = true;
    //private bool isGetCard = true;
    //public bool IsGetCard
    //{
    //    get { return isGetCard; }
    //    set { isGetCard = value; }
    //}

    private int randTime;
    private int randName;
    private int randDescription;

    private List<string> nameArr = new List<string>
    {
        {"Diliboli_sk" },
        {"Dreamed guy" },
        {"IronMan_2333" },
        {"Brother Zhang" },
        {"Wang Lao Ju" },
        {"Old Tomato" }
    };

    private List<string> DescriArr = new List<string>
    {
        {"It's so great today" },
        {"Always feel bad" },
        {"WTF???" },
        {"OMG" },
        {"New video Already!!" },
        {"Morning guys!" }
    };

    private bool isShuable = true;

    public bool IsShuable
    {
        get {
            return isShuable;
        }
        set {
            isShuable = value;
        }
    }
    

    public int GetCurrentTurnShuaTime()
    {
        return shuaTimeLimit - curShuaTime;    
    }

    public void ReduceShuaTime()
    {
        curShuaTime++;
        if(curShuaTime==shuaTimeLimit)
        {
            IsShuable = false;
        }
    }

    public void resetShua()
    {
        curShuaTime = 0;
        isShuable = true;
        isRealRandom = true;
    }



    public string randomTime()
    {
        if(isRealRandom)
        {
            randTime = UnityEngine.Random.Range(0, 80);
        }
        return randTime + " minitus ago";
    }

    public string randomName()
    {
        if (isRealRandom)
        {
            randName = UnityEngine.Random.Range(0, 5);   
        }
        return nameArr[randName];
    }

    public string randomDescription()
    {
        if (isRealRandom)
        {
            randDescription = UnityEngine.Random.Range(0, 5);
        }
        return DescriArr[randDescription];
    }

    public void enableRealRandom()
    {
        isRealRandom = true;
    }

    public void disableRealRandom()
    {
        isRealRandom = false;
    }
}
