using UnityEngine;
using System.Collections;
using System;

public interface ICoreManager : IModule
{

    //void LoadGameMode<T>() where T : GameModeBase;

    GameModeBase LoadGameMode(Type t, GameModeInitData initData);

    void ChangeScene(string sname, GameModeInitData initData = null, Action onSceneFinished = null);

    GameModeBase GetGameMode();
}

