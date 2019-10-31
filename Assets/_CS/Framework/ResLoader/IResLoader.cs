using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public delegate void OnSceneLoadedDlg(Scene scene, LoadSceneMode mode);

public interface IResLoader : IModule {


	#region 加载资源

	#region 通用资源

	T LoadResource<T>(string path, bool cached = true) where T : UnityEngine.Object;

	GameObject Instantiate(string strPath,Transform p=null);

	GameObject Instantiate(GameObject prefab, Transform p=null);


	bool UnloadAsset(string sName);



	#endregion


	#endregion
	void LoadLevelSync(string scenePath, LoadSceneMode mode, UnityAction<Scene, LoadSceneMode> callback = null);

	IEnumerator LoadLevelAsync(string scenePath, LoadSceneMode mode);

    void ReleaseGO(string str, GameObject obj);


    void LoadWWWResAsync<T>(string path, ResLoader.AsynvLoadCallback<T> callback) where T : Object;

    T[] LoadAllResouces<T>(string path) where T : UnityEngine.Object;

}
