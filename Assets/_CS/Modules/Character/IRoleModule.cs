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

    RoleStats GetStats();


    void AddMeili(float v);
    void AddTili(float v);
    void AddKoucai(float v);
    void AddJiyi(float v); 
    void AddFanying(float v);

    void AddTezhi(List<Tezhi> tezhis);

    void InitRole(string roleId);
    void AddFensi(int type, int num);

    void GetMoney(int amount);

    void AddTrackExp(string type, int num);
    int GetTrackExp(string track);

    PlatformInfo GetNowPlatformInfo();

    int GetBadLevel();
}
