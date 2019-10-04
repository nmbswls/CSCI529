using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

public class RoleModule : ModuleBase, IRoleModule
{

	public int RoleId;
    public int TurnNum = 0;

    public string[] Schedules;

    private ISpeEventMgr pEventMgr;
    private ICoreManager pCoreMgr;

    

    List<AppInfo> unlockedApps = new List<AppInfo>();

    private readonly Dictionary<string, ScheduleInfo> ScheduleDict = new Dictionary<string, ScheduleInfo>();
    private readonly List<string> choices = new List<string>();

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

    public override void Setup()
    {
        unlockedApps.Add(new AppInfo("微信","wechat"));
        unlockedApps.Add(new AppInfo("邮箱", "email"));
        unlockedApps.Add(new AppInfo("地图", "maps"));
        unlockedApps.Add(new AppInfo("购物", "taobao"));



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
        pCoreMgr = GameMain.GetInstance().GetModule<CoreManager>();
    }


    public void NextTurn()
    {
        TurnNum++;
        Debug.Log("现在回合数:" + TurnNum);
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
}

