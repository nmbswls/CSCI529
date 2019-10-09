using UnityEngine;
using System.Collections;
using System;

public interface IUIBaseCtrl{
	void Setup(string nameStr, IUIMgr mUIMgr);
	void Release ();

	string nameStr{ get; set; }
	Transform GetTransform();
	void RegisterEvent();
	void BindView();

	void Tick(float dTime);

    bool Zhiding { get; set; }
}
public class UIBaseCtrl<T1, T2> : IUIBaseCtrl where T1 : BaseModel where T2 : BaseView
{

	protected T1 model;
	protected T2 view;
	protected IUIMgr mUIMgr;
	public string nameStr{ get; set;}

    private bool zhiding;
    public bool Zhiding
    {
        get
        {
            return zhiding;
        }

        set
        {
            zhiding = value;
        }
    }

    protected Transform root = null;


	public virtual void Setup(string nameStr, IUIMgr mUIMgr){
		this.mUIMgr = mUIMgr;
		this.nameStr = nameStr;
		ResLoader resLoader = GameMain.GetInstance ().GetModule<ResLoader> ();
        zhiding = false;
        GameObject prefab = resLoader.LoadResource<GameObject> ("UI/UIPanels/" + nameStr,false);
		if (prefab == null) {
			Debug.LogError ("load "+nameStr+" main menu fail");
			return;
		}
		GameObject panel = resLoader.Instantiate (prefab,mUIMgr.GetUIRoot().transform);
		if (panel == null) {
			Debug.LogError ("instantiate "+nameStr+" main menu fail");
			return;
		}
		root = panel.transform;


        model = Activator.CreateInstance<T1>();
        view = Activator.CreateInstance<T2>();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();


        Init();
		BindView ();
		RegisterEvent ();
		PostInit ();
	}


	public virtual Transform GetTransform (){
		return root;
	}
	public virtual void BindView(){
	
	}
	public virtual void RegisterEvent(){
	}

	public virtual void Init(){
	}

	public virtual void PostInit(){
	
	}
	public virtual void Release(){
		if (root != null) {
			GameObject.Destroy (root.gameObject);
		}
	}
	public virtual void Tick(float dTime){
	
	}
}

