using UnityEngine;
using System.Collections;

public interface IUIMgr : IModule
{
	GameObject GetUIRoot();

	void LockUI();
	void UnlockUI();

	void ShowPanel (string panelStr);
	void CloseFirstPanel();

	//mainMask
	void showHint(string text);

	void Loading ();
	void FinishLoading();


}

