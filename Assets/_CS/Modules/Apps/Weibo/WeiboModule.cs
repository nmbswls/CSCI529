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
        {"铜教授" },
        {"追风干部刘没有" },
        {"老铁" },
        {"老张" },
        {"王老菊" },
        {"老番茄" }
    };

    private List<string> DescriArr = new List<string>
    {
        {"来今儿个给大家搞个二斤地瓜烧" },
        {"当朝大学士，总共有五位，朕不得不罢免三位" },
        {"转发这条锦鲤，也没什么卵用" },
        {"卧槽" },
        {"又发新视频了" },
        {"早上起来，拥抱太阳" }
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
        return randTime + " 分钟前";
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
