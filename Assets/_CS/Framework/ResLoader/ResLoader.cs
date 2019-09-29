using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResLoader : ModuleBase, IResLoader
{




	string lastLoadSyncPath = string.Empty;
	string lastLoadAsyncPath = string.Empty;


	private GameObjectPool mGOPool = new GameObjectPool();
	private AssetPool mAssetPool = new AssetPool();

	public delegate void AsynvLoadCallback<T>(T t) where T : Object;

	private IResMgr mResMgr = new ResMgr(); 


	public override void Setup(){
	}

	public IEnumerator LoadLevelAsync(string scenePath, LoadSceneMode mode)
	{

		//GameEvent e = null;
		yield break;
	}

	public void LoadLevelSync(string scenePath, LoadSceneMode mode)
	{
		
	}



	public T LoadResource<T>(string path, bool cached = true) where T : Object
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






}
