using UnityEngine;
using System.Collections;

public interface IUIMgr : IModule
{
	GameObject GetUIRoot();

	void LockUI();
	void UnlockUI();

	IUIBaseCtrl ShowPanel (string panelStr);
	void CloseFirstPanel();
	void CloseCertainPanel (IUIBaseCtrl toClose);
	//mainMask
	void showHint(string text);

	void Loading ();
	void FinishLoading();

    IUIBaseCtrl GetCtrl(string str);

    Vector3 GetWorldPosition(Vector2 screenPos);
    Vector3 GetLocalPosition(Vector2 screenPos, RectTransform target);

    Camera GetCamera();
}

