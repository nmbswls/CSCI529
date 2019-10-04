using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IRoleModule : IModule{

    List<AppInfo> GetApps();

    bool ChangeSchedule(int slotIdx, string scheduleId);

    List<ScheduleInfo> getAllScheduleChoises();

    string[] getScheduled();

    void NextTurn();

    int ScheduleMax { get; set; }
    int OverDueSchedule { get; set; }

    ScheduleInfo GetInfo(string sid);
}
