using UnityEngine;
using System.Collections;
using System;

public interface ICoreManager : IModule
{

    //void LoadGameMode<T>() where T : GameModeBase;

    GameModeBase LoadGameMode(Type t);

    void ChangeScene(string sname, Action onSceneChanged = null, Action onSceneFinished = null);

    GameModeBase GetGameMode();
}

