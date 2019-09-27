using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMgr : ModuleBase, IUIMgr
{

	private GameObject UIRoot;
	private CanvasGroup RootCanvasGroup;

	private int lockCount = 0;

	private readonly Dictionary<string, IUIBaseCtrl> mUIPanelMap = new Dictionary<string, IUIBaseCtrl>();
	public readonly Dictionary<string, Type> mUITypeMap = new Dictionary<string, Type>();

	private List<IUIBaseCtrl> mUILayerList = new List<IUIBaseCtrl> ();

	public override void Setup(){
		UIRoot = GameObject.Find ("UIRoot");
		RootCanvasGroup = UIRoot.GetComponent<CanvasGroup> ();
		RegisterUIPanel ();
		InitUI ();
	}

	public GameObject GetUIRoot(){
		return UIRoot;
	}


	private void InitUI(){
		ShowPanel ("StartNewGame");
	}

	private void RegisterUIPanel(){
		
		mUITypeMap["UIMain"] = typeof(UIMainCtrl);
		mUITypeMap["StartNewGame"] = typeof(ChooseStoryLineCtrl);
	}

	public void LockUI ()
	{
		lockCount++;
		RootCanvasGroup.blocksRaycasts = false;
	}

	public void UnlockUI ()
	{
		if (lockCount > 0) {
			lockCount--;
		}
		if(lockCount==0){
			RootCanvasGroup.blocksRaycasts = true;
		}
	}

	public void ShowPanel (string panelStr)
	{
		string nname = panelStr;
		if (mUIPanelMap.ContainsKey(panelStr))
		{
			IUIBaseCtrl UICtrl = mUIPanelMap[panelStr];
			//更换顺序
			mUILayerList.Remove(UICtrl);
			mUILayerList.Add(UICtrl);

		}
		else
		{
			Type type = mUITypeMap[nname];
			IUIBaseCtrl UICtrl = (IUIBaseCtrl)Activator.CreateInstance(type);
			if (UICtrl != null)
			{
				UICtrl.Setup (panelStr,this);
				mUIPanelMap[nname] = UICtrl;
				mUILayerList.Add(UICtrl);
			}
		}
		AdjustLayerOrder ();
	}

	private void AdjustLayerOrder(){
		for (int i = 0; i < mUILayerList.Count; i++) {
			Transform tr = mUILayerList [i].GetTransform ();
			if (tr != null) {
				tr.SetSiblingIndex (i);
			}
		}
	}



	public void CloseFirstPanel ()
	{
		if (mUILayerList.Count == 0) {
			return;
		}
		IUIBaseCtrl ctrl =  mUILayerList[mUILayerList.Count-1];
		ctrl.Release ();
		mUILayerList.RemoveAt (mUILayerList.Count-1);
		mUIPanelMap.Remove(ctrl.nameStr);
		AdjustLayerOrder ();
	}

	public void showHint (string text)
	{
		throw new System.NotImplementedException ();
	}

	public void Loading ()
	{
		throw new System.NotImplementedException ();
	}

	public void FinishLoading ()
	{
		throw new System.NotImplementedException ();
	}



}

