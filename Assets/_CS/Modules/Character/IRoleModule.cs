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

    int GetFensiReward(int extraLiuliang, float addrate);

    void AddMeili(float v);
    void AddTili(float v);
    void AddKoucai(float v);
    void AddJiyi(float v); 
    void AddFanying(float v);

    void AddAllStatus(float v);

    void AddActionPoints(float v);
    void RestoreActionPoints(float v);

    void AddTezhi(List<Tezhi> tezhis);

    void InitRole(string roleId);
    void AddFensi(int type, int num);

    void GainMoney(int amount);

    float Money { get;  }

    void AddTrackExp(string type, int num);
    int GetTrackExp(string track);

    PlatformInfo GetNowPlatformInfo();

    int GetBadLevel();

    bool CanPractice();
    void Practive();

    int MaxItemNum { get;}

    int GetXinqingLevel();
    void GetXinqing(int amount);
}
