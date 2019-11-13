using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhiboEmergencyManager
{
    public ZhiboGameMode gameMode;

    public IResLoader mResLoader;
    public EmergencyAsset nowEmergency = null;

    private Dictionary<string, EmergencyAsset> EmergencyDict = new Dictionary<string, EmergencyAsset>();
    

    private void InitEmergency()
    {
        LoadEmergencies();
        List<string> emergencies = new List<string>();
        //初始化
    }
    public ZhiboEmergencyManager(ZhiboGameMode gameMode)
    {
        this.gameMode = gameMode;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        InitEmergency();
    }

    public void FakeEmergencies()
    {
        {
            EmergencyAsset ea = new EmergencyAsset();
            ea.EmId = "em01";
            ea.EmName = "老爸出现！";
            ea.EmDesp = "老爸突然闯了进来！";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "装傻";
                choice.Ret = "100";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "向观众介绍";
                choice.Ret = "100";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "喷走";
                choice.Ret = "100";
                ea.Choices.Add(choice);
            }

            EmergencyDict["em01"] = ea;
        }
    }

    private void LoadEmergencies(){
        FakeEmergencies();
    }

    public EmergencyAsset GetEmergencyAsset(string id)
    {
        return mResLoader.LoadResource<EmergencyAsset>("Emergencies/choufeng");
    }
}
