using UnityEngine;
using System.Collections;

public interface ICoreManager : IModule
{

    void LoadGameMode<T>() where T : GameModeBase;

    void ChangeScene();
}

