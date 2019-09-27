using UnityEngine;
using System.Collections;

public interface IGameMain
{

	T GetModule<T>() where T : IModule;
}

public interface IModuleMgr
{
	void Init(IGameMain gameMain);

	IModule CreateModule(ModuleConfig moduleConfig);

	T GetModule<T> () where T : IModule;


}
public interface IEventMgr
{

	void RegisterModuleEvent(IEventListener eventListener);
}