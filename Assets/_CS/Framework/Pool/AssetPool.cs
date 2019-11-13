using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetPool
{

	private Dictionary<string, Object> mObjectsCurScene = new Dictionary<string, Object>();

	private Dictionary<string, Object> mCacheObjects = new Dictionary<string, Object>();

	public T Get<T>(string path) where T : UnityEngine.Object
	{
		Object o = null;

		mCacheObjects.TryGetValue(path,out o);

		if (o == null)
		{
			mObjectsCurScene.TryGetValue(path, out o);
		}

		if (o != null)
		{
			return o as T;
		}

		return null;
	}

	public void AddAsset(string path, Object o)
	{
		if(o is GameObject)
		{
			return;
		}

		mCacheObjects[path] = o;
	}

	public void Release(string path){
		if (path != string.Empty && mCacheObjects.ContainsKey (path)) {
			mCacheObjects.Remove (path);
		}
	}

}
