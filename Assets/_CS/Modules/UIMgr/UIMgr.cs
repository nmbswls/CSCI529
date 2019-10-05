using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMgr : ModuleBase, IUIMgr
{

	private GameObject mUIRoot;
    private GameObject mEventSystem;
    private Camera mCamera;
	private CanvasGroup RootCanvasGroup;

	private int lockCount = 0;

	private readonly Dictionary<string, IUIBaseCtrl> mUIPanelMap = new Dictionary<string, IUIBaseCtrl>();
	public readonly Dictionary<string, Type> mUITypeMap = new Dictionary<string, Type>();

	private List<IUIBaseCtrl> mUILayerList = new List<IUIBaseCtrl> ();

	public override void Setup(){
		mUIRoot = GameObject.Find ("UIRoot");
        mEventSystem = GameObject.Find("EventSystem");
        mCamera = Camera.main;
        if (mUIRoot == null)
        {
            Debug.Log("No Ui root found!");
        }
        GameObject.DontDestroyOnLoad(mUIRoot);
        GameObject.DontDestroyOnLoad(mEventSystem);
        GameObject.DontDestroyOnLoad(mCamera);
		RootCanvasGroup = mUIRoot.GetComponent<CanvasGroup> ();
		RegisterUIPanel ();
		//InitUI ();
	}

	public override void Tick(float dTime){
        for(int i = mUILayerList.Count - 1; i >= 0; i--) { 
            mUILayerList[i].Tick (dTime);
		}
	}

	public GameObject GetUIRoot(){
		return mUIRoot;
	}




	private void RegisterUIPanel(){
		
		mUITypeMap["UIMain"] = typeof(UIMainCtrl);
		mUITypeMap["StartNewGame"] = typeof(ChooseStoryLineCtrl);
		mUITypeMap["CardsMgr"] = typeof(ManageCardsPanelCtrl);

		mUITypeMap["AdjustPanel"] = typeof(AdjustInitCtrl);

		mUITypeMap["DialogManager"] = typeof(DialogManager);
		mUITypeMap["HomeMenuCtrl"] = typeof(HomeMenuCtrl);

        mUITypeMap["SchedulePanel"] = typeof(ScheduleCtrl);

        mUITypeMap["ZhiboPanel"] = typeof(ZhiboUI);
        mUITypeMap["ActBranch"] = typeof(ActBranchCtrl);

        mUITypeMap["HintCtrl"] = typeof(HintCtrl);

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

	public void CloseCertainPanel(IUIBaseCtrl toClose){
		string name = toClose.nameStr;
		if (mUIPanelMap.ContainsKey (name)) {
			mUILayerList.Remove (toClose);
            mUIPanelMap.Remove(name);
            toClose.Release();
        }
        else
        {
            if (mUILayerList.Contains(toClose))
            {
                toClose.Release();
                mUILayerList.Contains(toClose);
                mUILayerList.Remove(toClose);
            }
            else
            {
                Debug.Log("close not exitst ui panel");
            }
        }
    }

    public void showHint(string text)
    {
        HintCtrl UICtrl = new HintCtrl();
        if (UICtrl != null)
        {
            UICtrl.Setup("hint", this);
            UICtrl.SetContent(text);
            UICtrl.GetTransform().SetSiblingIndex(100);
            mUILayerList.Add(UICtrl);
        }

    }

    public IUIBaseCtrl ShowPanel (string panelStr)
	{
		string nname = panelStr;
        IUIBaseCtrl UICtrl = null;

        if (mUIPanelMap.ContainsKey(panelStr))
		{
			UICtrl = mUIPanelMap[panelStr];
			//更换顺序
			mUILayerList.Remove(UICtrl);
			mUILayerList.Add(UICtrl);

		}
		else
		{
			Type type = mUITypeMap[nname];
            UICtrl = (IUIBaseCtrl)Activator.CreateInstance(type);
			if (UICtrl != null)
			{
				UICtrl.Setup (panelStr,this);
				mUIPanelMap[nname] = UICtrl;
				mUILayerList.Add(UICtrl);
			}
		}
		AdjustLayerOrder ();
        return UICtrl;
    }

	private void AdjustLayerOrder(){
		for (int i = 0; i < mUILayerList.Count; i++) {
			Transform tr = mUILayerList [i].GetTransform ();
			if (tr != null) {
                if (mUILayerList[i].Zhiding)
                {
                    tr.SetSiblingIndex(100);
                }
                else
                {
                    tr.SetSiblingIndex(i);
                }

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

    public IUIBaseCtrl GetCtrl(string str)
    {
        if (mUIPanelMap.ContainsKey(str))
        {
            return mUIPanelMap[str];
        }
        return null;
    }

   

	public void Loading ()
	{
		throw new System.NotImplementedException ();
	}

	public void FinishLoading ()
	{
		throw new System.NotImplementedException ();
	}

    public Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Vector3 ret = mCamera.ScreenToWorldPoint(screenPos);
        ret.z = 0;
        return ret;
    }

    public Vector3 GetLocalPosition(Vector2 screenPos, RectTransform target)
    {
        Vector3 rootPos = GetWorldPosition(screenPos);
        Vector4 origin = new Vector4(rootPos.x, rootPos.y, rootPos.z,1);
        Vector3 localPos = target.worldToLocalMatrix.MultiplyVector(origin);
        localPos.z = 0;
        return localPos;
    }


}

