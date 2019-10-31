using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhiboEmergencyManager
{
    public ZhiboGameMode gameMode;

    public IResLoader mResLoader;
    public EmergencyAsset nowEmergency = null;


    

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

    private void LoadEmergencies(){

    }

    public EmergencyAsset GetEmergencyAsset(string id)
    {
        return mResLoader.LoadResource<EmergencyAsset>("Emergencies/choufeng");
    }
}
