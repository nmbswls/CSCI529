using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEventListener
{
	void RegisterEvent();
    GameEventMap GetEventMap();
}

public interface IComponent : IEventListener
{

}

public interface IModule : IEventListener
{
	void Setup();

	void Tick(float dTime);

	string GetModuleName();

	//component xiangguan
}


