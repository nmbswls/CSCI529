using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CheckFunWrap{
	public CheckFunc func;
	public int argNeed;

	public CheckFunWrap(CheckFunc func, int argNeed){
		this.func = func;
		this.argNeed = argNeed;
	}
}
public class LogicTree : ModuleBase, ILogicTree{



	public LogicNode ConstructFromString (string str){
		LogicNode ret = null;
		try{
			ret = LogicNode.ConstructFromString (str, FuncDict);
		}catch(Exception e){
			Debug.Log (e);
		}
		return ret;
	}

	private Dictionary<string, CheckFunWrap> FuncDict = new Dictionary<string,CheckFunWrap>();

	public override void Setup(){
		//InstId = 0;
		BindCheckFunc();
	}

	bool True(string[] args){
		return true;
	}

	bool False(string[] args){
		return false;
	}

	bool CheckLevel(string[] args){
		int level = 10;
		return false;
	}

	private void BindCheckFunc(){
		FuncDict ["True"] = new CheckFunWrap(True,0);
		FuncDict ["False"] = new CheckFunWrap(False,0);
	}
}
