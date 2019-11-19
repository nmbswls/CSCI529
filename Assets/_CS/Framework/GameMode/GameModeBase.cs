using UnityEngine;
using System.Collections;
using System;

public class GameStateBase
{

}

public class HudBase
{

}

public class GameModeBase
{
    public Action GameFinishedCallback;
    public bool Initialized = false;

	public virtual void Tick(float dTime){
		return;
	}
	public virtual void Init(){
		return;
	}

    public virtual void OnRelease()
    {

    }
}

