using UnityEngine;
using System.Collections;
using System;

public delegate void OnCompleteDlg();
public interface ICoreManager : IModule
{

    //void LoadGameMode<T>() where T : GameModeBase;

    void LoadGameMode(Type t);

    void ChangeScene(string sname, OnCompleteDlg onComplete = null);

    GameModeBase GetGameMode();
}

