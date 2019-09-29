using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGameMode : GameModeBase
{

	IUIMgr UImgr;

	List<string> UnHandledEvent = new List<string>();

	public override void Tick(float dTime){
	
	}

	public override void Init(){
		UImgr = GameMain.GetInstance ().GetModule<UIMgr> ();
		UImgr.ShowPanel ("HomeMenuCtrl");
	}



	public void BeginTurn(){
		CheckBeginEvent ();
		//get list

		//UImgr.ShowPanel();
	}

	public void NextEvent(){
		//UnHandledEvent;
		//HandledEventId++;
	}

	private void CheckBeginEvent(){

	}
}

