using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModuleMgr: IModuleMgr
{
	public bool HasInstance ()
	{
		throw new NotImplementedException ();
	}

	private IGameMain mGameMain;

	public readonly Dictionary<string, Type> RegModuleList = new Dictionary<string, Type>();

	private readonly Dictionary<string, IModule> mModuleMap = new Dictionary<string, IModule>();
	private readonly Dictionary<Type, string> mMapType = new Dictionary<Type, string>();

	private readonly List<IModule> mModuleList = new List<IModule>();

	//private Dictionary<string, string> moduleName2AssemblyName = new Dictionary<string, string>();


	public void Init(IGameMain gameMain)
	{
		mGameMain = gameMain;
		RegModuleList.Clear();

		RegModuleList["CardDeck"] = typeof(CardDeckModule);
		RegModuleList["UIMgr"] = typeof(UIMgr);
		RegModuleList["ResLoader"] = typeof(ResLoader);
		RegModuleList["LogicTree"] = typeof(LogicTree);
		RegModuleList["DialogModule"] = typeof(DialogModule);
		RegModuleList["SpeEventMgr"] = typeof(SpeEventMgr);
		RegModuleList["RoleModule"] = typeof(RoleModule);

		RegModuleList["CoreManager"] = typeof(CoreManager);

        RegModuleList["SkillTreeMgr"] = typeof(SkillTreeMgr);

        RegModuleList["WeiboModule"] = typeof(WeiboModule);
        RegModuleList["ShopMgr"] = typeof(ShopMgr);



        mMapType[typeof(CardDeckModule)] = "CardDeck";
        mMapType[typeof(UIMgr)] = "UIMgr";
        mMapType[typeof(ResLoader)] = "ResLoader";
        mMapType[typeof(LogicTree)] = "LogicTree";
        mMapType[typeof(DialogModule)] = "DialogModule";
        mMapType[typeof(SpeEventMgr)] = "SpeEventMgr";
        mMapType[typeof(RoleModule)] = "RoleModule";
        mMapType[typeof(CoreManager)] = "CoreManager";
        mMapType[typeof(WeiboModule)] = "WeiboModule";

        mMapType[typeof(ShopMgr)] = "ShopMgr";
    }

	public void Tick(float dTime){
		foreach(IModule module in mModuleList){
			module.Tick (dTime);
		}
	}

    public void SetupModules()
    {
        for(int i=0;i< mModuleList.Count; i++)
        {
            mModuleList[i].Setup();
        }
    }

    public IModule CreateModule(ModuleConfig config)
	{
		string nname = config.ModuleName;
		if (mModuleMap.ContainsKey(nname))
		{
			IModule m = mModuleMap[nname];
			//更换顺序
			mModuleList.Remove(m);
			mModuleList.Add(m);
			return m;
		}
		else
		{
			if (!RegModuleList.ContainsKey(nname))
			{
				return null;
			}
			Type type = RegModuleList[nname];
			//ModuleBase module = new NetModule();
			ModuleBase module = (ModuleBase)Activator.CreateInstance(type);
			if (module != null)
			{
				module.SetModuleName(mGameMain, nname);
				module.RegisterModuleEvent();
				//module.Setup ();
				mModuleMap[nname] = module;
				mModuleList.Add(module);
			}
			return module;
		}
	}

	public T GetModule<T>() where T : IModule
	{
		System.Type kType = typeof(T);
		string moduleInterfaceName = "";

		if (!mMapType.TryGetValue(kType,out moduleInterfaceName))
		{
			moduleInterfaceName = kType.Name;
		}

		IModule module = null;

		if (mModuleMap.TryGetValue(moduleInterfaceName, out module))
		{
			return (T)module;
		}
		else
		{
			return default(T);
		}

	}


}
