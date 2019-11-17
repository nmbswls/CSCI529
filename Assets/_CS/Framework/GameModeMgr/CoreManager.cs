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

    private Stack<GameModeBase> mGameModeStack = new Stack<GameModeBase>();

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



    public void ChangeScene(string sname, Action onSceneChanged = null, Action onSceneFinished = null)
    {
        if (!SceneInfoDict.ContainsKey(sname))
        {
            return;
        }
        string SceneName = SceneInfoDict[sname].SceneName;

        mResLoader.LoadLevelSync("Scene/"+ SceneName, LoadSceneMode.Single, delegate (Scene scene, LoadSceneMode mode) {
            Type t = SceneInfoDict[sname].GameModeType;
            GameModeBase gm = LoadGameMode(t);
            if(onSceneChanged != null)
            {
                onSceneChanged();
            }
            gm.GameFinishedCallback = onSceneFinished;
        });
    }





    public GameModeBase LoadGameMode(Type t)
    {
        if (!t.IsSubclassOf(typeof(GameModeBase)))
        {
            Debug.Log("type is not a game mode");
            return null;
        }

        //if(t == mGameMode.GetType())
        //{
        //    //相同模式 说明不变
        //    return mGameMode;
        //}

        GameModeBase preGm = mGameMode;

        mGameMode = (GameModeBase)Activator.CreateInstance(t);
        if (mGameMode == null)
        {
            Debug.LogError("Load Game Mode " + t.FullName + " fail");
            return null;
        }
        mGameMode.Init();

        if (preGm != null)
        {
            preGm.OnRelease();
        }

        return mGameMode;
    }


    private bool HasLoadGameMode(Type t)
    {
        foreach(GameModeBase gm in mGameModeStack)
        {
            if (gm.GetType() == t)
            {
                return true;
            }
        }
        return false;

    }

    public GameModeBase LoadGameMode<T>() where T : GameModeBase
    {
        return LoadGameMode(typeof(T));
    }


    public override void Tick(float dTime){
		if (mGameMode != null) {
			mGameMode.Tick (dTime);
		}
	}
}

