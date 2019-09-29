using UnityEngine;
using System.Collections;

public interface IGameMain
{
	void RunCoroutine(IEnumerator e);
	T GetModule<T>() where T : IModule;
}

public interface IModuleMgr
{
	void Init(IGameMain gameMain);

	void Tick (float dTime);

	IModule CreateModule(ModuleConfig moduleConfig);

	T GetModule<T> () where T : IModule;


}
public interface IEventMgr
{

	void RegisterModuleEvent(IEventListener eventListener);
}