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

}

public class RoleModule : ModuleBase, IRoleModule
{

	public int RoleId;

    public int NormalScheduleSlot;
    public int OvertimeScheduleSlot;

    public string[] Schedules;

    List<AppInfo> unlockedApps = new List<AppInfo>();

    private readonly Dictionary<string, ScheduleInfo> ScheduleDict = new Dictionary<string, ScheduleInfo>();

    public override void Setup()
    {
        unlockedApps.Add(new AppInfo("微信","wechat"));
        unlockedApps.Add(new AppInfo("邮箱", "email"));
        unlockedApps.Add(new AppInfo("地图", "maps"));
        unlockedApps.Add(new AppInfo("购物", "taobao"));


        NormalScheduleSlot = 6;
        OvertimeScheduleSlot = 3;
        Schedules = new string[NormalScheduleSlot + OvertimeScheduleSlot];

    }
    public List<AppInfo> GetApps()
    {
        return unlockedApps;
    }

    public void ChangeSchedule()
    {

    }

    public bool ChangeSchedule(int slotIdx, string scheduleId)
    {
        if(slotIdx < 0 || slotIdx >= Schedules.Length)
        {
            return false;
        }
        if (!ScheduleDict.ContainsKey(scheduleId))
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

