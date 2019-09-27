using UnityEngine;
using System.Collections;

public class ModuleBase : IModule
{
	private string mModuleName = string.Empty;
//	protected readonly CtmEventMap mEventMap = new CtmEventMap();

	protected IGameMain mGameMain;
	public virtual string GetModuleName()
	{
		return mModuleName;
	}

	public void SetModuleName(IGameMain gameMain, string name)
	{
		mGameMain = gameMain;
		mModuleName = name;
	}

	public virtual void RegisterEvent()
	{
		return;
	}

	public virtual void Setup()
	{
		return;
	}

	public virtual void Update(float dTime)
	{
		return;
	}

	public void RegisterModuleEvent()
	{

	}

}
