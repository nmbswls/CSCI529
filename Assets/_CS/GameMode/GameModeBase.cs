using UnityEngine;
using System.Collections;

public class GameModeBase
{
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

