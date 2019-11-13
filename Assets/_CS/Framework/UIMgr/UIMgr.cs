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
    private ModelMask mask;

	private readonly Dictionary<string, IUIBaseCtrl> mUIPanelMap = new Dictionary<string, IUIBaseCtrl>();
	public readonly Dictionary<string, Type> mUITypeMap = new Dictionary<string, Type>();

	private List<IUIBaseCtrl> mUILayerList = new List<IUIBaseCtrl> ();
    private List<HintCtrl> mHints = new List<HintCtrl>();
    private static int ZhidingSeblingIdx = 100; 

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
        mask = new ModelMask();
        mask.Setup("ModelMask",this);
        //InitUI ();
    }

	public override void Tick(float dTime){
        for(int i = mUILayerList.Count - 1; i >= 0; i--) { 
            mUILayerList[i].Tick (dTime);
		}

        for(int i=mHints.Count - 1; i>=0; i--)
        {
            mHints[i].Tick(dTime);
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

        mUITypeMap["SchedulePanel"] = typeof(SKillCtrl);
        mUITypeMap["WeiboPanel"] = typeof(WeiboUI);

        mUITypeMap["ZhiboPanel"] = typeof(ZhiboUI);
        mUITypeMap["ActBranch"] = typeof(ActBranchCtrl);

        mUITypeMap["HintCtrl"] = typeof(HintCtrl);

        mUITypeMap["TravelPanel"] = typeof(TravelUI);
        mUITypeMap["ModelMask"] = typeof(ModelMask);

        mUITypeMap["MsgBox"] = typeof(MsgBoxCtrl);
        mUITypeMap["ConfirmBox"] = typeof(ConfirmBoxCtrl);


        mUITypeMap["ZhiboJiesuanPanel"] = typeof(ZhiboJiesuanUI);

        mUITypeMap["TaobaoPanel"] = typeof(TaobaoUI);

        mUITypeMap["AdjustSkillCardsPanel"] = typeof(AdjustSkillCardCtrl);


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
                mUILayerList.Remove(toClose);

            }
            else
            {
                Debug.Log("close not exitst ui panel");
            }
        }
        AdjustLayerOrder();
    }

    public void ShowHint(string text)
    {
        HintCtrl UICtrl = new HintCtrl();
        if (UICtrl != null)
        {
            UICtrl.Setup("hint", this);
            UICtrl.SetContent(text);
            UICtrl.GetTransform().SetSiblingIndex(ZhidingSeblingIdx);
            mHints.Add(UICtrl);
        }

    }

    public void CloseHint(HintCtrl hint)
    {
        hint.Release();
        mUILayerList.Remove(hint); 
    }

    public Camera GetCamera()
    {
        return mCamera;
    }
    public IUIBaseCtrl ShowPanel (string panelStr, bool modal = true)
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
		AdjustLayerOrder (modal);
        return UICtrl;
    }

	private void AdjustLayerOrder(bool modal = true){

		for (int i = 0; i < mUILayerList.Count-1; i++) {
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
        if (modal)
        {
            mask.GetTransform().SetSiblingIndex(mUILayerList.Count - 1);
            mask.GetTransform().gameObject.SetActive(true);
        }
        else
        {
            mask.GetTransform().gameObject.SetActive(false);
        }
        if (mUILayerList.Count > 0)
        {
            mUILayerList[mUILayerList.Count - 1].GetTransform().SetSiblingIndex(mUILayerList.Count);
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

    public void ShowMsgBox(string content)
    {
        MsgBoxCtrl msgBox = ShowPanel("MsgBox") as MsgBoxCtrl;
        msgBox.ShowMsg(content);
    }

    public void ShowConfirmBox(string content, Action cb)
    {
        ConfirmBoxCtrl msgBox = ShowPanel("ConfirmBox") as ConfirmBoxCtrl;
        msgBox.ShowMsg(content,cb);
    }

}

