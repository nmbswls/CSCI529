using UnityEngine;
using System.Collections;

public interface IResMgr
{
	T LoadObject<T>(string path) where T : Object;
}


public class ResMgr : IResMgr
{
	public T LoadObject<T>(string path) where T : Object
	{
		T asset;
		asset = Resources.Load<T>(path);
		return asset;
	}
}