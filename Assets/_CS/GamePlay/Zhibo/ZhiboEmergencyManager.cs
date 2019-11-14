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
            ea.EmName = "Oh my father！";
            ea.EmDesp = "My father is coming！";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Ignore";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Introduce him to the audience";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Drive away";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }

            EmergencyDict["em01"] = ea;
        }
        {
            EmergencyAsset ea = new EmergencyAsset();
            ea.EmId = "em02";
            ea.EmName = "Oh my mother！";
            ea.EmDesp = "My mother is coming！";
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Ignore";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Introduce her to the audience";
                choice.Ret = "Hot";
                ea.Choices.Add(choice);
            }
            {
                EmergencyChoice choice = new EmergencyChoice();
                choice.Content = "Drive away";
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
}
