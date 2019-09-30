using UnityEngine;
using System.Collections;
using System;

public interface ICoreManager : IModule
{

    //void LoadGameMode<T>() where T : GameModeBase;

    void LoadGameMode(Type t);

    void ChangeScene(string sname);

    GameModeBase GetGameMode();
}

