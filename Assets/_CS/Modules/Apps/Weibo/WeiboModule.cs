using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeiboReviewEffect
{
    none,
    AddKoucai,
    AddCaiyi,
    AddJishu,
    AddKangya,
    AddWaiguan,
    AddAllState,
    AddFensi
}

public class Review
{
    public string content;
    public WeiboReviewEffect effect;
    public int value;
}

public class Weibo
{
    public int index;
    public string name;
    public string description;
    public string content;
    public string time;
    public string avatar;
    public bool forwardable;
    public string gainCardId;
    public bool reviewable;
    public List<Review> reviews;

}

public class WeiboList
{
    public List<Weibo> weibos = new List<Weibo>();

    public void loadWeibo()
    {
        WeiboContentList weiboContent = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<WeiboContentList>("WeiboTxt/WeiboContentList", false);
        foreach(WeiboAsset w in weiboContent.Entities)
        {
            Weibo weibo = new Weibo();
            weibo.index = w.Index;
            weibo.name = w.Name;
            weibo.content = w.Content;
            weibo.time = w.Time;
            weibo.avatar = w.Avatar;

            weibo.forwardable = w.Forwardable == "Yes";
            weibo.gainCardId = w.GainCard;

            weibo.reviewable = w.Reviewable == "Yes";
            if(weibo.reviewable)
            {
                List<Review> reviews = new List<Review>();
                if (w.Review1 != "None" && w.Review1.Length > 0) { 
                    Review review1 = new Review();
                    review1.content = w.Review1;
                    review1.effect = (WeiboReviewEffect)System.Enum.Parse(typeof(WeiboReviewEffect), w.Bonus1);
                    review1.value = w.Value1;
                    reviews.Add(review1);
                }
                if (w.Review2 != "None" && w.Review2.Length > 0)
                {
                    Review review2 = new Review();
                    review2.content = w.Review2;
                    review2.effect = (WeiboReviewEffect)System.Enum.Parse(typeof(WeiboReviewEffect), w.Bonus2);
                    review2.value = w.Value2;
                    reviews.Add(review2);
                }
                if (w.Review3 != "None" && w.Review3.Length > 0)
                {
                    Review review3 = new Review();
                    review3.content = w.Review3;
                    review3.effect = (WeiboReviewEffect)System.Enum.Parse(typeof(WeiboReviewEffect), w.Bonus3);
                    review3.value = w.Value3;
                    reviews.Add(review3);
                }
                weibo.reviews = reviews;
            }
            weibos.Add(weibo);
        }
    }
}

public class WeiboModule : ModuleBase, IWeiboModule
{
    private const int shuaTimeLimit = 4;     //每回合只有3次刷到牌的机会
    private int curShuaTime = 0;
    private bool isRealRandom = true;

    private int randTime;
    private int randName;
    private int randDescription;

    private bool isShuable = true;

    public bool IsShuable
    {
        get
        {
            return isShuable;
        }
        set
        {
            isShuable = value;
        }
    }

    public WeiboList weiboList = new WeiboList();

    public override void Setup()
    {
        weiboList.loadWeibo();
    }

    public int GetCurrentTurnShuaTime()
    {
        return shuaTimeLimit - curShuaTime;
    }

    public void ReduceShuaTime()
    {
        curShuaTime++;
        if (curShuaTime == shuaTimeLimit)
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
        if (isRealRandom)
        {
            randTime = UnityEngine.Random.Range(0, 59);
        }
        if (randTime < 12)
        {
            int randTimeUnit = UnityEngine.Random.Range(0, 2);
            if (randTimeUnit == 0)
            {
                return randTime + " 分钟前";
            }
            else
            {
                return randTime + " 小时前";
            }
        }
        return randTime + " 分钟前";
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



//public class WeiboModule1 : ModuleBase, IWeiboModule
//{
//    private const int shuaTimeLimit = 4;     //每回合只有3次刷到牌的机会
//    private int curShuaTime = 0;
    
//    private bool isRealRandom = true;


//    //private bool isGetCard = true;
//    //public bool IsGetCard
//    //{
//    //    get { return isGetCard; }
//    //    set { isGetCard = value; }
//    //}

//    private int randTime;
//    private int randName;
//    private int randDescription;

//    //private List<string> nameArr = new List<string>
//    //{
//    //    {"铜教授" },
//    //    {"追风干部刘没有" },
//    //    {"老铁" },
//    //    {"老张" },
//    //    {"王老菊" },
//    //    {"老番茄" }
//    //};

//    private List<string> nameArr = new List<string>();

//    private List<string> DescriArr = new List<string>();
//    //private List<string> DescriArr = new List<string>
//    //{
//    //    {"来今儿个给大家搞个二斤地瓜烧" },
//    //    {"当朝大学士，总共有五位，朕不得不罢免三位" },
//    //    {"转发这条锦鲤，也没什么卵用" },
//    //    {"卧槽" },
//    //    {"又发新视频了" },
//    //    {"早上起来，拥抱太阳" }
//    //};

    

//    private bool isShuable = true;

//    public bool IsShuable
//    {
//        get {
//            return isShuable;
//        }
//        set {
//            isShuable = value;
//        }
//    }
    
//    public void LoadWeiboTxt()
//    {
//        //stage 1
//        WeiboContentList weiboContent1 = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<WeiboContentList>("WeiboTxt/WeiboContentList", false);
//        foreach (WeiboAsset c in weiboContent1.Entities)
//        {
//            nameArr.Add(c.Name);
//            DescriArr.Add(c.Desp);
//        }
//    }

//    public void LoadStage(int weiboStage)
//    {

//    }

//    public override void Setup()
//    {
//        LoadWeiboTxt();
//    }

//    public int GetCurrentTurnShuaTime()
//    {
//        return shuaTimeLimit - curShuaTime;    
//    }

//    public void ReduceShuaTime()
//    {
//        curShuaTime++;
//        if(curShuaTime==shuaTimeLimit)
//        {
//            IsShuable = false;
//        }
//    }

//    public void resetShua()
//    {
//        curShuaTime = 0;
//        isShuable = true;
//        isRealRandom = true;
//    }

//    public string randomTime()
//    {
//        if(isRealRandom)
//        {
//            randTime = UnityEngine.Random.Range(0, 59);
//        }
//        if(randTime < 12)
//        {
//            int randTimeUnit = UnityEngine.Random.Range(0, 2);
//            if(randTimeUnit == 0)
//            {
//                return randTime + " 分钟前";
//            }
//            else
//            {
//                return randTime + " 小时前";
//            }
//        }
//        return randTime + " 分钟前";
//    }

//    public string randomName()
//    {
//        if (isRealRandom)
//        {
//            randName = UnityEngine.Random.Range(0, nameArr.Count);   
//        }
//        return nameArr[randName];
//    }

//    public string randomDescription()
//    {
//        if (isRealRandom)
//        {
//            randDescription = UnityEngine.Random.Range(0, DescriArr.Count);
//        }
//        return DescriArr[randDescription];
//    }

//    public void enableRealRandom()
//    {
//        isRealRandom = true;
//    }

//    public void disableRealRandom()
//    {
//        isRealRandom = false;
//    }
//}
