using UnityEngine;
using System.Collections;
using System;

public class GameStateBase
{

}

public class HudBase
{

}

public class GameModeInitData
{

}



public class GameModeBase
{
    public Action GameFinishedCallback;
    public bool Initialized = false;
    public GameModeInitData InitData;

    public virtual void Tick(float dTime){
		return;
	}
	public virtual void Init(GameModeInitData InitData)
    {
		return;
	}

    public virtual void OnRelease()
    {

    }
}

