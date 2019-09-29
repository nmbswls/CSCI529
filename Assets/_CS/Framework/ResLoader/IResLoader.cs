using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IResLoader : IModule {


	#region 加载资源

	#region 通用资源

	T LoadResource<T>(string path, bool cached = true) where T : UnityEngine.Object;

	GameObject Instantiate(string strPath,Transform p=null);

	GameObject Instantiate(GameObject prefab, Transform p=null);


	bool UnloadAsset(string sName);



	#endregion


	#endregion
	void LoadLevelSync(string scenePath, LoadSceneMode mode);

	IEnumerator LoadLevelAsync(string scenePath, LoadSceneMode mode);



}
