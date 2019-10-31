using UnityEngine;
using System.Collections;

public interface IGameMain
{
	void RunCoroutine(IEnumerator e);
	T GetModule<T>() where T : IModule;

    long GetTime(bool bflag = true);
}

public interface IModuleMgr
{
	void Init(IGameMain gameMain);

	void Tick (float dTime);

	IModule CreateModule(ModuleConfig moduleConfig);

	T GetModule<T> () where T : IModule;

    void SetupModules();


}
public interface IEventMgr
{

	void RegisterModuleEvent(IEventListener eventListener);
}