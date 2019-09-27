using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEventListener
{
	void RegisterEvent();

}

public interface IComponent : IEventListener
{

}

public interface IModule : IEventListener
{
	void Setup();

	void Update(float dTime);

	string GetModuleName();

	//component xiangguan
}


