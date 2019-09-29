using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class CoreManager : ModuleBase, ICoreManager
{

	private GameModeBase mGameMode;
    private ResLoader mResLoader;

    public override void Setup ()
	{
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        LoadInitGameMode();
        
	}

    private void LoadInitGameMode()
    {
        LoadGameMode<MainGameMode>();
    }

    public void ChangeScene()
    {
        mResLoader.LoadLevelSync("Scene/Travel",LoadSceneMode.Single,delegate(Scene scene, LoadSceneMode mode) {
            LoadGameMode<TravelGameMode>();
        });
    }

    public void LoadGameMode<T>() where T : GameModeBase
    {
        T gm = Activator.CreateInstance<T>();
        if (gm == null)
        {
            Debug.LogError("Load Game Mode "+typeof(T)+" fail");
        }
        if(mGameMode != null)
        {
            mGameMode.OnRelease();
        }
        mGameMode = gm;
        gm.Init();
    }


    public override void Tick(float dTime){
		if (mGameMode != null) {
			mGameMode.Tick (dTime);
		}
	}
}

