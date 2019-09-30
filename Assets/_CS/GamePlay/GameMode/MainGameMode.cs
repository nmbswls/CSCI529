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
		UImgr.ShowPanel ("UIMain");
	}



	public void BeginTurn(){
		CheckBeginEvent ();
		//get list

		//UImgr.ShowPanel();
	}

	public void NextEvent(){
		//UnHandledEvent;
		//HandledEventId++;
		//get first event
		SpecialEvent se = null;
		//handle
	}

	public void NextTurn(){
	
	}

	private void CheckBeginEvent(){

	}
}

