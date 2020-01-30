using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhiboEmergencyManager
{
    public ZhiboGameMode gameMode;

    public IResLoader mResLoader;
    public IUIMgr mUIMgr;
    public EmergencyAsset nowEmergency = null;

    private Dictionary<string, EmergencyAsset> EmergencyDict = new Dictionary<string, EmergencyAsset>();


    private int emergencyIdx;

    //private int EmergencyShowTime;

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
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        InitEmergency();
    }

    public void NewTurn()
    {

        if (emergencyIdx < gameMode.state.ComingEmergencies.Count && gameMode.state.ComingEmergencies[emergencyIdx].Key == gameMode.state.NowTurn)
        {
            ShowEmergency(gameMode.state.ComingEmergencies[emergencyIdx].Value);
            //EmergencyShowTime = Random.Range(5, 8);
            emergencyIdx++;
        }
    }





    public void ShowEmergency(string emergencyId)
    {
        gameMode.Pause();
        mUIMgr.ShowPanel("ActBranch", true, false);
        ActBranchCtrl actrl = mUIMgr.GetCtrl("ActBranch") as ActBranchCtrl;
        EmergencyAsset ea = GetEmergencyAsset(emergencyId);
        actrl.SetEmergency(ea);
        actrl.ActBranchEvent += delegate (int idx) {
            gameMode.Resume();
            EmergencyChoice c = ea.Choices[idx];
            if (c.NextEmId != null && c.NextEmId != string.Empty)
            {

            }
            if (c.Ret == "Hot")
            {
                if(idx == 0)
                {
                    gameMode.GainScore(-10);
                    mUIMgr.ShowHint("Get " + "-10" + " Score");
                } else if(idx == 1)
                {
                    gameMode.GainScore(15);
                    mUIMgr.ShowHint("Get " + "15" + " Score");
                } else
                {
                    gameMode.GainScore(30);
                    mUIMgr.ShowHint("Get " + "30" + " Score");
                }
            }
        };
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
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "向观众介绍";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "喷走";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }

            EmergencyDict["em01"] = ea;
        }
        {
            EmergencyAsset ea = new EmergencyAsset();
            ea.EmId = "em02";
            ea.EmName = "老妈出现！";
            ea.EmDesp = "老妈突然闯了进来！";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "装傻";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "向观众介绍";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "喷走";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }

            EmergencyDict["em02"] = ea;
        }
    }

    private void LoadEmergencies(){
        FakeEmergencies();
    }

    public EmergencyAsset GetEmergencyAsset(string id)
    {
        if (EmergencyDict.ContainsKey(id))
        {
            return EmergencyDict[id];
        }
        return mResLoader.LoadResource<EmergencyAsset>("Emergencies/choufeng");
    }

    public void GenEmergency()
    {

        gameMode.state.ComingEmergencies.Add(new KeyValuePair<int, string>(Random.Range(2, 3), "em01"));
        gameMode.state.ComingEmergencies.Add(new KeyValuePair<int, string>(Random.Range(7, 9), "em02"));
        emergencyIdx = 0;
    }
}
