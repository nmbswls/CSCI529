using UnityEngine;
using System;
using System.Collections;

public class GameMain : MonoBehaviour,IGameMain
{
	private static IGameMain mInstance;

	private GameMainConfig mGameMainConfig = new GameMainConfig();

	private readonly IModuleMgr mModuleMgr = new ModuleMgr();

	private void Awake()
	{
		Init();
        GameObject.DontDestroyOnLoad(this);
	}

	public void RunCoroutine(IEnumerator c){
		StartCoroutine (c);
	}


	public void Start(){

	}

	public void Update(){
		
		mModuleMgr.Tick (Time.deltaTime);
	}


	public void Init()
	{
		mModuleMgr.Init(this);



		ModuleConfigList DefaultModuleList = new ModuleConfigList();

		DefaultModuleList.ConfigList.Add(new ModuleConfig("ResLoader"));

		DefaultModuleList.ConfigList.Add(new ModuleConfig("UIMgr"));
        DefaultModuleList.ConfigList.Add(new ModuleConfig("CardDeck"));

        DefaultModuleList.ConfigList.Add (new ModuleConfig ("LogicTree"));
		DefaultModuleList.ConfigList.Add (new ModuleConfig ("DialogModule"));

		DefaultModuleList.ConfigList.Add (new ModuleConfig ("RoleModule"));
		DefaultModuleList.ConfigList.Add (new ModuleConfig ("SpeEventMgr"));

		DefaultModuleList.ConfigList.Add (new ModuleConfig ("CoreManager"));
        DefaultModuleList.ConfigList.Add(new ModuleConfig("SkillTreeMgr"));

        DefaultModuleList.ConfigList.Add(new ModuleConfig("WeiboModule"));
        DefaultModuleList.ConfigList.Add(new ModuleConfig("ShopMgr"));


        mGameMainConfig.Config.Add(DefaultModuleList);
		LoadInitModules();
	}

	public void Release()
	{
		mInstance = null;
	}


	public static IGameMain GetInstance()
	{
		if (mInstance == null)
		{
			Type type = typeof(GameMain);
			GameMain gameMain = (GameMain)FindObjectOfType(type);
			mInstance = gameMain;
		}
		return mInstance;
	}



	private void LoadInitModules()
	{
		CreateModules(0);
	}



	private void CreateModules(int t)
	{
		ModuleConfigList newModuleList = mGameMainConfig.GetModuleList(t);
		foreach(ModuleConfig config in newModuleList.ConfigList)
		{
			mModuleMgr.CreateModule(config);
		}

        mModuleMgr.SetupModules();

    }

	public static bool HasInstance()
	{
		return mInstance != null;
	}


	public T GetModule<T>() where T : IModule
	{
		T t = mModuleMgr.GetModule<T>();
		return t;
	}

    public long GetTime(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }
}
