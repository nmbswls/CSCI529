using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IRoleModule : IModule{

    List<AppInfo> GetApps();

    bool ChangeSchedule(int slotIdx, string scheduleId);

    List<ScheduleInfo> getAllScheduleChoises();

    string[] getScheduled();

    void NextTurn();
    int GetCurrentTurn();

    int ScheduleMax { get; set; }
    int OverDueSchedule { get; set; }

    ScheduleInfo GetInfo(string sid);

    RoleStats GetStats();

    int GetFensiReward(int extraLiuliang, float addrate);

    void AddWaiguan(float v);
    void AddKangya(float v);
    void AddKoucai(float v);
    void AddJishu(float v); 
    void AddCaiyi(float v);

    void AddAllStatus(float v);

    void AddSkillPoint(int v);

    int GetSkillPoint();


    void AddActionPoints(float v);
    void RestoreActionPoints(float v);

    void AddTezhi(List<Tezhi> tezhis);

    void AllocateStats(int[] extra);

    int[] LoadStates(string roleId);

    void InitRole(string roleId);
    void AddFensi(int type, int num);

    void GainMoney(int amount);

    float Money { get;  }
    float Fensi { get; }

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
