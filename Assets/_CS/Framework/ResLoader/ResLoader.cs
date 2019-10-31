using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class ResLoader : ModuleBase, IResLoader
{




    string lastLoadSyncPath = string.Empty;
	string lastLoadAsyncPath = string.Empty;

    UnityAction<Scene, LoadSceneMode> lastCallback = null;

    private IWWWMgr mWWWMgr;

    private GameObjectPool mGOPool = new GameObjectPool();
	private AssetPool mAssetPool = new AssetPool();

	public delegate void AsynvLoadCallback<T>(T t) where T : UnityEngine.Object;

	private IResMgr mResMgr = new ResMgr(); 


	public override void Setup(){
	}

    public override void Tick(float dTime)
    {
        if (mWWWMgr != null)
        {
            mWWWMgr.Update(dTime);
        }
    }

    public IEnumerator LoadLevelAsync(string scenePath, LoadSceneMode mode)
	{
        AsyncOperation async = SceneManager.LoadSceneAsync(scenePath,mode);
        //GameEvent e = null;
        yield break;
	}

	public void LoadLevelSync(string scenePath, LoadSceneMode mode, UnityAction<Scene,LoadSceneMode> callback = null)
	{
        SceneManager.LoadScene(scenePath, mode);
        if(lastCallback != null)
        {
            SceneManager.sceneLoaded -= lastCallback;
        }
        if (callback != null)
        {
            SceneManager.sceneLoaded += callback;
        }
        lastCallback = callback;
    }



	public T LoadResource<T>(string path, bool cached = true) where T : UnityEngine.Object
    {
        T ret = null;
		if (!string.IsNullOrEmpty(path))
		{
			ret = mGOPool.GetPoolPrefab(path) as T;
			if (ret == null)
			{
				ret = mAssetPool.Get<T>(path);
			}

			if(ret == null)
			{
				ret = _LoadResPrefab<T>(path);

				if(ret != null && cached)
				{
					GameObject prefab = ret as GameObject;
					if (prefab != null) {
						mGOPool.CachePoolPrefab (path, prefab, true);
					} else {
						mAssetPool.AddAsset (path, ret);
					}
				}
			}

		}
		return ret;

	}

	private T _LoadResPrefab<T>(string path) where T : UnityEngine.Object
	{
		if(path != string.Empty)
		{
			T obj = default(T);

			if(mResMgr != null)
			{
				obj = mResMgr.LoadObject<T>(path);
			}

			return obj;
		}
		return null;
	}

    public void ReleaseGO(string str, GameObject obj)
    {
        GameObject prefab = mGOPool.GetPoolPrefab(str);
        if(prefab == null)
        {
            prefab = LoadResource<GameObject>(str);
        }
        if(prefab == null)
        {
            GameObject.Destroy(obj);
        }
        else
        {
            mGOPool.Release(prefab, obj);
        }
    }

    public T[] LoadAllResouces<T>(string path) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(path);
    }


    public GameObject Instantiate(string strPath,Transform p=null)
	{
		GameObject prefab = LoadResource<GameObject>(strPath);
		if(prefab != null)
		{
			return Instantiate(prefab, Vector3.zero, Quaternion.identity,p);
		}
		return null;
	}


	public GameObject Instantiate (GameObject prefab, Transform p=null)
	{
		if(prefab != null)
		{
			return Instantiate(prefab, Vector3.zero, Quaternion.identity,p);
		}
		return null;
	}

	public GameObject Instantiate(string strPath, Vector3 localPos, Quaternion localRot)
	{
		GameObject prefab = LoadResource<GameObject>(strPath);
		if(prefab != null)
		{
			return Instantiate(prefab, localPos,localRot);
		}
		return null;
	}


	public GameObject Instantiate(GameObject prefab, Vector3 localPos, Quaternion localRot,Transform parent = null)
	{
		if(prefab == null)
		{
			return null;
		}

		bool isInstantiate = false;
		GameObject obj = null;

		obj = mGOPool.Spawn(prefab, localPos, localRot, out isInstantiate);
		if (isInstantiate)
		{
			obj.name = prefab.name;
		}

		if(obj != null)
		{
			obj.transform.SetParent(parent,false);
			obj.transform.localPosition = localPos;
			obj.transform.localRotation = localRot;
		}

		ResetInstant(obj);
		return obj;
	}

	private void ResetInstant(GameObject obj){
		//do something
	}

	public bool UnloadAsset(string sName)
	{
		mAssetPool.Release (sName);
		return true;
	}



    private void HandleLoadWWWResAsync<T>(string url, AsynvLoadCallback<T> callback) where T : UnityEngine.Object
    {
        IWWWMgr wwwMgr = GetWWWMgr();

        T asset = wwwMgr.TryLoadFromCache<T>(WWWType.DEFAULT, url);

        if (asset != null && callback != null)
        {
            callback(asset);
            return;
        }
        wwwMgr.BuildRequest(WWWType.DEFAULT, url)
            .SetSuccessCallback(delegate (WWWRequestHandle handle)
            {
                asset = wwwMgr.TryLoadFromCache<T>(WWWType.DEFAULT, url);
                if (callback != null)
                {
                    callback(asset);
                }
            }).Get();
    }

    public IWWWMgr GetWWWMgr()
    {
        if (mWWWMgr == null)
        {
            mWWWMgr = new WWWMgr(mAssetPool);
        }
        return mWWWMgr;
    }

    public void LoadWWWResAsync<T>(string path, AsynvLoadCallback<T> callback) where T : UnityEngine.Object
    {
        HandleLoadWWWResAsync(path, callback);
    }


}
