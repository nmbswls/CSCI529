using UnityEngine;
using System.Collections;

public class CoreManager : ModuleBase, ICoreManager
{

	public GameModeBase mGameMode;  

	public override void Setup ()
	{
		mGameMode = new MainGameMode ();
		mGameMode.Init ();
	}

	public override void Tick(float dTime){
		if (mGameMode != null) {
			mGameMode.Tick (dTime);
		}
	}
}

