﻿using UnityEngine;
using System.Collections;
using System;

public interface IUIMgr : IModule
{
	GameObject GetUIRoot();

	void LockUI();
	void UnlockUI();

	IUIBaseCtrl ShowPanel (string panelStr,bool modal = true, bool canClose = true);
	void CloseFirstPanel();
	void CloseCertainPanel (IUIBaseCtrl toClose);

    void CloseMasks();
    void CloseLastMask();
	//mainMask
	void ShowHint(string text);
    void CloseHint(HintCtrl hint);

    void Loading ();
	void FinishLoading();

    IUIBaseCtrl GetCtrl(string str);

    Vector3 GetWorldPosition(Vector2 screenPos);
    Vector3 GetLocalPosition(Vector2 screenPos, RectTransform target);

    Camera GetCamera();

    void ShowMsgBox(string content);
    void ShowConfirmBox(string content, Action okCallback, Action cancelCallback = null);

    bool CanMaskClose { get; set; }
}

