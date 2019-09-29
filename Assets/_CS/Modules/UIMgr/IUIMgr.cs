using UnityEngine;
using System.Collections;

public interface IUIMgr : IModule
{
	GameObject GetUIRoot();

	void LockUI();
	void UnlockUI();

	void ShowPanel (string panelStr);
	void CloseFirstPanel();
	void CloseCertainPanel (IUIBaseCtrl toClose);
	//mainMask
	void showHint(string text);

	void Loading ();
	void FinishLoading();


}

