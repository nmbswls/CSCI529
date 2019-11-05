using UnityEngine;
using System.Collections;

public class ModuleBase : IModule
{
	private string mModuleName = string.Empty;
	protected readonly GameEventMap mEventMap = new GameEventMap();

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
        //mEventMap.RegisterEvent(CEventType.TestEvent, (int)TestEventId.Test1, OnTest01Event);
        return;
	}

	public virtual void Setup()
	{
		return;
	}

	public virtual void Tick(float dTime)
	{
		return;
	}

	public void RegisterModuleEvent()
	{

	}


    public GameEventMap GetEventMap()
    {
        return null;
    }
}
