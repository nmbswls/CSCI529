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

    private int EmergencyShowTime;

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
            EmergencyShowTime = Random.Range(5, 8);
            emergencyIdx++;
        }
        else
        {
            EmergencyShowTime = -1;
        }
    }

    public void CheckEmergencySec(int sec)
    {
        if (EmergencyShowTime != -1 && EmergencyShowTime == sec)
        {
            //已经加过了 要-1
            ShowEmergency(gameMode.state.ComingEmergencies[emergencyIdx - 1].Value);
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
            ea.EmName = "Oh My Father！";
            ea.EmDesp = "Your dad entering your room while you are streaming, what should you do?";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Pretend to ignore him.";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Introduce him to the audiences.";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Ask him to sit down and play together.";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }

            EmergencyDict["em01"] = ea;
        }
        {
            EmergencyAsset ea = new EmergencyAsset();
            ea.EmId = "em02";
            ea.EmName = "Oh My Mother！";
            ea.EmDesp = "Your mom entering your room while you are streaming, what should you do?";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Pretend to ignore her.";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Introduce her to the audiences.";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Ask her to sit down and play together.";
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

        gameMode.state.ComingEmergencies.Add(new KeyValuePair<int, string>(Random.Range(1, 2), "em01"));
        gameMode.state.ComingEmergencies.Add(new KeyValuePair<int, string>(Random.Range(7, 9), "em02"));
        emergencyIdx = 0;
    }
}
