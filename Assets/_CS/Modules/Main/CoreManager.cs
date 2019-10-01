using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneInfo
{
    public string SceneName;
    public Type GameModeType;

    public SceneInfo(string SceneName, Type GameModeType)
    {
        this.SceneName = SceneName;
        this.GameModeType = GameModeType;
    }
}

public class CoreManager : ModuleBase, ICoreManager
{

	private GameModeBase mGameMode;
    private ResLoader mResLoader;

    private Dictionary<string, SceneInfo> SceneInfoDict = new Dictionary<string, SceneInfo>();



    public override void Setup ()
	{
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        InitSceneDict();
        LoadInit();
        
	}
    public GameModeBase GetGameMode()
    {
        return mGameMode;
    }

    public void InitSceneDict()
    {
        SceneInfoDict["Travel"] = new SceneInfo("Travel",typeof(TravelGameMode));
        SceneInfoDict["Main"] = new SceneInfo("Main", typeof(MainGameMode));
        SceneInfoDict["Home"] = new SceneInfo("Main", typeof(HomeGameMode));

        SceneInfoDict["Zhibo"] = new SceneInfo("Zhibo", typeof(ZhiboGameMode));
    }

    private void LoadInit()
    {
        ChangeScene("Home");
    }

    //public void ChangeScene()
    //{
    //    mResLoader.LoadLevelSync("Scene/Travel",LoadSceneMode.Single,delegate(Scene scene, LoadSceneMode mode) {
    //        LoadGameMode<TravelGameMode>();
    //    });
    //}

    public void ChangeScene(string sname)
    {
        if (!SceneInfoDict.ContainsKey(sname))
        {
            return;
        }
        string SceneName = SceneInfoDict[sname].SceneName;
        mResLoader.LoadLevelSync("Scene/"+ SceneName, LoadSceneMode.Single, delegate (Scene scene, LoadSceneMode mode) {
            Type t = SceneInfoDict[sname].GameModeType;
            LoadGameMode(t);
        });
    }


    public void LoadGameMode(Type t)
    {
        if (!t.IsSubclassOf(typeof(GameModeBase)))
        {
            Debug.Log("type is not a game mode");
            return;
        }
        GameModeBase gm = (GameModeBase)Activator.CreateInstance(t);
        if (gm == null)
        {
            Debug.LogError("Load Game Mode " + t.FullName + " fail");
        }
        if (mGameMode != null)
        {
            mGameMode.OnRelease();
        }
        mGameMode = gm;
        gm.Init();
    }

    public void LoadGameMode<T>() where T : GameModeBase
    {
        LoadGameMode(typeof(T));
    }


    public override void Tick(float dTime){
		if (mGameMode != null) {
			mGameMode.Tick (dTime);
		}
	}
}

