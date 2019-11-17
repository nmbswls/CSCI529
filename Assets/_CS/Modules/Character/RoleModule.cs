using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlatformInfo
{
    public int Liuliang;
    public int[] FensiBili = new int[4];
    public int[] Xihao = new int[5];
    public string PlatformId;
    public string PlatformDesp;
    public List<string> PlatformEffect = new List<string>();
    public List<string> PlatformCards = new List<string>();
}

public class AppInfo
{
    public string ShowName;
    public string AppId;

    public AppInfo(string ShowName,string AppId)
    {
        this.ShowName = ShowName;
        this.AppId = AppId;
    }
}

public class ScheduleInfo
{
    public string SId;
    public string Name;
    public string Desp;

    public ScheduleInfo()
    {

    }

    public ScheduleInfo(string SId, string Name, string Desp)
    {
        this.SId = SId;
        this.Name = Name;
        this.Desp = Desp;
    }
}



public class RoleStats
{

    public float meili = 10;
    public float koucai = 10;
    public float tili = 10;
    public float fanying = 10;
    public float jiyi = 10;



    public RoleStats()
    {

    }
    public RoleStats(RoleStats stats)
    {
        this.meili = stats.meili;
        this.koucai = stats.koucai;
        this.tili = stats.tili;
        this.fanying = stats.fanying;
        this.jiyi = stats.jiyi;
    }
}

public class RoleModule : ModuleBase, IRoleModule
{

	public int RoleId;
    public int TurnNum = 0;

    public string[] Schedules;


    //test test
    public int resource;


    private ISpeEventMgr pEventMgr;
    private ICardDeckModule pCardMdl;
    private ICoreManager pCoreMgr;
    private ISkillTreeMgr pSkillTreeMgr;

    private RoleStats roleStats = new RoleStats();

    private float money;

    public float Money { get {return money; } protected set {money = value; } }


    //路人粉数量
    private float fen1Num;
    //
    private float fen2Num;
    private float fen3Num;

    private float fen4Num;

    private float BadPoint = 5;
    private int XinqingLevel = 10;

    private string NowPlatformId;

    private float ActionPoints;
    private float DefaultActionPoints;


    List<AppInfo> unlockedApps = new List<AppInfo>();

    private readonly List<Tezhi> TezhiList = new List<Tezhi>();

    private readonly Dictionary<string, ScheduleInfo> ScheduleDict = new Dictionary<string, ScheduleInfo>();

    private readonly Dictionary<string, PlatformInfo> Platforms = new Dictionary<string, PlatformInfo>();
    private Dictionary<string, int> TrackExps = new Dictionary<string, int>();
    private readonly List<string> choices = new List<string>();


    public int MaxItemNum { get { return 5 + TurnNum / 6; }}

    private int scheduleMax;
    public int ScheduleMax
    {
        get
        {
            return scheduleMax;
        }

        set
        {
            scheduleMax = value;
        }
    }
    private int overDueSchedule;

    public int OverDueSchedule { get { return overDueSchedule; } set { overDueSchedule = value; } }

    public void InitRole(string roleId)
    {
        RoleStoryAsset ret = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<RoleStoryAsset>("Roles/role" + roleId);
        roleStats.meili = ret.initProperties[0];
        roleStats.tili = ret.initProperties[1];
        roleStats.koucai = ret.initProperties[2];
        roleStats.jiyi = ret.initProperties[3];
        roleStats.fanying = ret.initProperties[4];

        money = ret.initMoney+1000;

        pCardMdl.AddCards(ret.initCards);
        pCardMdl.AddCards(ret.initOwning);

        BadPoint = 20;
        XinqingLevel = 10;

        FakePlatformInfo();
        NowPlatformId = "begin";
    }

    public int GetFensiReward(int extraLiuliang, float addrate)
    {
        float totalP = roleStats.fanying + roleStats.jiyi + roleStats.koucai + roleStats.meili + roleStats.tili;
        return (int)((30 + TurnNum * 50 + (totalP * 10 + fen1Num * 0.2f + extraLiuliang) * 0.2f) *addrate);
    }

    public void AddFensi(int type, int num)
    {
        if(type == 0)
        {
            fen1Num += num;
        }
    }

    public void GainMoney(int amount)
    {
        money += amount;
    }

    private void XinqingBodong()
    {
        GetXinqing(Random.Range(-2, 2));
    }

    public void GetXinqing(int amount)
    {

        XinqingLevel += amount;
        if(XinqingLevel > 20)
        {
            XinqingLevel = 20;
        }
        if(XinqingLevel < 0)
        {
            XinqingLevel = 0;
        }
    }

    public int GetXinqingLevel()
    {
        return XinqingLevel;
    }

    public override void Setup()
    {
        unlockedApps.Add(new AppInfo("微信","wechat"));
        unlockedApps.Add(new AppInfo("邮箱", "email"));
        unlockedApps.Add(new AppInfo("地图", "maps"));
        unlockedApps.Add(new AppInfo("购物", "taobao"));
        unlockedApps.Add(new AppInfo("微博", "weibo"));



        ScheduleDict.Add("fitness1",new ScheduleInfo("fitness1","fitness1", "健身达人1"));
        ScheduleDict.Add("fitness2", new ScheduleInfo("fitness2","fitness2", "健身达人2"));
        ScheduleDict.Add("fitness3", new ScheduleInfo("fitness3","fitness3", "健身达人3"));
        ScheduleDict.Add("fitness4", new ScheduleInfo("fitness4","fitness4", "健身达人4"));
        ScheduleDict.Add("fitness5", new ScheduleInfo("fitness5","fitness5", "健身达人5"));
        ScheduleDict.Add("fitness6", new ScheduleInfo("fitness6","fitness6", "健身达人6"));
        ScheduleDict.Add("fitness7", new ScheduleInfo("fitness7","fitness7", "健身达人7"));
        ScheduleDict.Add("fitness8", new ScheduleInfo("fitness8","fitness8", "健身达人8"));

        choices.Add("fitness1");
        choices.Add("fitness2");
        choices.Add("fitness3");
        choices.Add("fitness4");
        choices.Add("fitness8");

        ScheduleMax = 10;
        OverDueSchedule = 3;

        Schedules = new string[ScheduleMax];

        pEventMgr = GameMain.GetInstance().GetModule<SpeEventMgr>();
        pCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pCoreMgr = GameMain.GetInstance().GetModule<CoreManager>();
        pSkillTreeMgr = GameMain.GetInstance().GetModule<SkillTreeMgr>();
    }

    public RoleStats GetStats()
    {
        return roleStats;
    }

    public void NextTurn()
    {
        TurnNum++;
        RestoreActionPoints();
        XinqingBodong();
    }

    public void RestoreActionPoints()
    {
        ActionPoints = 1000;
    }


    public ScheduleInfo GetInfo(string sid)
    {
        if (sid == null)
        {
            return null;
        }
        return ScheduleDict[sid];
    }



    public List<AppInfo> GetApps()
    {
        return unlockedApps;
    }

    public List<ScheduleInfo> getAllScheduleChoises()
    {

        List<ScheduleInfo> ret = new List<ScheduleInfo>();
        for(int i = choices.Count - 1; i >= 0; i--)
        {
            if (!ScheduleDict.ContainsKey(choices[i]))
            {
                choices.RemoveAt(i);
                continue;
            }
            ret.Add(ScheduleDict[choices[i]]);
        }
        return ret;
    }

    public string[] getScheduled()
    {
        return Schedules;
    }

    public void UnlockChoosableSchedule(string ScheduleId)
    {
        if (ScheduleId==string.Empty || !ScheduleDict.ContainsKey(ScheduleId))
        {
            return;
        }
        if (choices.Contains(ScheduleId))
        {
            return;
        }
        choices.Add(ScheduleId);
    }

    

    public bool ChangeSchedule(int slotIdx, string scheduleId)
    {
        if(slotIdx < 0 || slotIdx >= Schedules.Length)
        {
            return false;
        }
        if (scheduleId != null && !ScheduleDict.ContainsKey(scheduleId))
        {
            return false;
        }
        Schedules[slotIdx] = scheduleId;
        return true;
    }

    public void InitEvent(){
		
	}

    public void UnlockApp(string appId)
    {

    }

    public void AddMeili(float v)
    {
        roleStats.meili += v;
    }
    public void AddTili(float v)
    {
        roleStats.tili += v;
    }
    public void AddKoucai(float v)
    {
        roleStats.koucai += v;
    }
    public void AddJiyi(float v)
    {
        roleStats.jiyi += v;
    }
    public void AddFanying(float v)
    {
        roleStats.fanying += v;
    }

    public void AddAllStatus(float v)
    {
        AddMeili(v);
        AddTili(v);
        AddKoucai(v);
        AddJiyi(v);
        AddFanying(v);
    }


    public void AddActionPoints(float v)
    {
        ActionPoints += v;
    }

    public void RestoreActionPoints(float v)
    {
        if(ActionPoints < DefaultActionPoints)
        {
            ActionPoints = DefaultActionPoints;
        }
    }

    public void AddTezhi(List<Tezhi> tezhis)
    {
        TezhiList.AddRange(tezhis);
        foreach(Tezhi tezhi in TezhiList)
        {
            foreach(string effectString in tezhi.Effects)
            {
                string[] args = effectString.Split(',');
                if (args[0] == "addM")
                {
                    roleStats.meili += int.Parse(args[1]);
                }else if (args[0] == "card")
                {
                    pCardMdl.GainNewCard(args[1]);
                }
            }
        }
    }

    public void AddTrackExp(string track, int num)
    {
        if (TrackExps.ContainsKey(track))
        {
            TrackExps[track] += num;
        }
        else
        {
            TrackExps[track] = num;
        }
    }

    public int GetTrackExp(string track)
    {
        if (TrackExps.ContainsKey(track))
        {
            return TrackExps[track];
        }
        return 0;
    }

    public void FakePlatformInfo()
    {
        {
            PlatformInfo info = new PlatformInfo();
            info.PlatformCards.Add("card9005");
            Platforms["begin"] = info;
        }

    }

    public PlatformInfo GetNowPlatformInfo()
    {
        return Platforms[NowPlatformId];
    }

    public int GetBadLevel()
    {
        int badLevel = (int)(BadPoint / 5);
        if(badLevel > 10)
        {
            badLevel = 10;
        }
        return badLevel;
    }

    public bool CanPractice()
    {

        MainGameMode mgm = pCoreMgr.GetGameMode() as MainGameMode;
        if(ActionPoints < mgm.GetPracticeCost())
        {
            return false;
        }
        return true;
    }
    public void Practive()
    {
        MainGameMode mgm = pCoreMgr.GetGameMode() as MainGameMode;
        if (ActionPoints < mgm.GetPracticeCost())
        {
            return;
        }
        ActionPoints -= mgm.GetPracticeCost();
        mgm.TurnPracticeNum++;
    }

}

