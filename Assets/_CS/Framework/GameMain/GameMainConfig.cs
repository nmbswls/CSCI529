using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMainConfig
{
	public List<ModuleConfigList> Config { get; set; }

	public ModuleConfigList GetModuleList(int t)
	{
		return Config[t];
	}

	public GameMainConfig(){
		Config = new List<ModuleConfigList> ();
	}


}


public class ModuleConfigList
{
	public List<ModuleConfig> ConfigList { get; set; }

	public ModuleConfigList(){
		ConfigList = new List<ModuleConfig> ();
	}
}

public struct ModuleConfig
{
	public string ModuleName { get; set; }

	public ModuleConfig(string ModuleName)
	{
		this.ModuleName = ModuleName;
	}
}