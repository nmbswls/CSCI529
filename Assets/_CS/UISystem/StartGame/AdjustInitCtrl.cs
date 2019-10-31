using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class Tezhi{
	public int Score;
	public string Name;
    public List<string> Effects = new List<string>();
}
public class AdjustInitModel : BaseModel{

    public string roleId;
	
	public List<Tezhi> availabelTezhi = new List<Tezhi>();
	public List<int> selectedOne = new List<int>();

	public int LeftPoint = 10;
	public int LeftSkillPoint = 10;

	public int[] bas = new int[]{10,8,8,8,8};
	public int[] extra;
}

public class AdjustInitView : BaseView{

	public InputField NetName;

	public List<BasePropertyLineView> baseLines = new List<BasePropertyLineView> ();

	public Text PointLeft;
	public Text SKillLeft;

	public Transform PosSkillContainer;
	public List<SpecilistView> choosedList = new List<SpecilistView> ();

	public Transform AvailableContainer;
	public List<SpecilistView> avalableList = new List<SpecilistView> ();

	public Text DetailName;
	public Text DetailDesp;


	public Button NextStage;
}

public class SpecilistView{

	public RectTransform root;
	public bool selected;
	public Text Name;
    public Text Cost;
	public void BindView(Transform root){
		this.root = (RectTransform)root;
		this.Name = root.Find("Text").GetComponent<Text> ();
        this.Cost = root.Find("Cost").GetComponent<Text>();
    }
}

public class BasePropertyLineView{
	public Text BaseValue;
	public Text ExtraValue;
	public Image AddButton;
	public Image MinusButton;
	public Transform root;
	public void BindView(Transform root){
		this.root = root;
		BaseValue = root.GetChild (1).GetComponent<Text>();
		ExtraValue = root.GetChild (2).GetComponent<Text>();
		AddButton = root.GetChild (3).GetComponent<Image>();
		MinusButton = root.GetChild (4).GetComponent<Image>();
	}

}
public class AdjustInitCtrl : UIBaseCtrl<AdjustInitModel,AdjustInitView>
{

    IRoleModule pRoleMgr;

    public void SetRoleId(int idx)
    {
        model.roleId = idx + "";
    }


    public override void Init(){

        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();


        model.extra = new int[5];
        SetupAvailableTezhi();

    }

	public List<Tezhi> FakeReadTezhi(){
		List<Tezhi> ret = new List<Tezhi> ();
		{
			Tezhi tezhi = new Tezhi ();
			tezhi.Score = 2;
			tezhi.Name = "身强力壮";
			ret.Add (tezhi);
		}
		{
			Tezhi tezhi = new Tezhi ();
			tezhi.Score = 4;
			tezhi.Name = "天命所归";
			ret.Add (tezhi);
		}
		{
			Tezhi tezhi = new Tezhi ();
			tezhi.Score = 1;
			tezhi.Name = "口才";
			ret.Add (tezhi);
		}
		{
			Tezhi tezhi = new Tezhi ();
			tezhi.Score = -3;
			tezhi.Name = "自爆";
			ret.Add (tezhi);
		}

		return ret;
	}

	public void SetupAvailableTezhi(){
		model.availabelTezhi = FakeReadTezhi();
	}

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}
		view.NextStage = root.Find ("NextStage").GetComponent<Button>();

		view.SKillLeft = root.Find ("Text_Left").GetComponent<Text> ();
		view.PointLeft = root.Find ("属性调整").GetChild(0).GetChild(1).GetComponent<Text> ();

		view.SKillLeft.text = model.LeftSkillPoint + "";
		view.PointLeft.text = model.LeftPoint + "";

		view.PosSkillContainer = root.Find ("已选优势");
		view.AvailableContainer = root.Find ("可选特质").GetChild(1).GetChild(0).GetChild(0);

		view.DetailName = root.Find ("SpeDetail").GetChild (0).GetComponent<Text>();
		view.DetailDesp = root.Find ("SpeDetail").GetChild (1).GetComponent<Text>();


		Transform pRoot = root.Find ("属性调整").GetChild(1);

		foreach (Transform child in pRoot) {
			BasePropertyLineView vv = new BasePropertyLineView ();
			vv.BindView (child);
			vv.BaseValue.text = model.bas[child.GetSiblingIndex()] + "";
			view.baseLines.Add (vv);
		}

        foreach (Tezhi tezhi in model.availabelTezhi)
        {
            GameObject go = GameMain.GetInstance().GetModule<ResLoader>().Instantiate("UI/AvailableTezhi", view.AvailableContainer);
            SpecilistView vv = new SpecilistView();
            vv.BindView(go.transform);
            vv.Name.text = tezhi.Name;
            view.avalableList.Add(vv);
        }
    }

	public override void PostInit(){

    }

    public void SetupPlayerInfo()
    {
        //将数值写入rolemgr
        List<Tezhi> AddTezhiList = new List<Tezhi>();
        for(int i = 0; i < model.selectedOne.Count; i++)
        {
            AddTezhiList.Add(model.availabelTezhi[model.selectedOne[i]]);
        }
        pRoleMgr.InitRole(model.roleId);
        pRoleMgr.AddTezhi(AddTezhiList);


    }


    public override void RegisterEvent(){

		view.NextStage.onClick.AddListener (delegate() {
			mUIMgr.CloseCertainPanel(this);
            SetupPlayerInfo();
            GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main",delegate {
                ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
                MainGameMode gm = cm.GetGameMode() as MainGameMode;
                gm.NextTurn();

            });

            //mUIMgr.ShowPanel("UIMain");	
        });

		for (int i = 0; i < view.avalableList.Count; i++) {
			DragEventListener listener = view.avalableList[i].root.gameObject.GetComponent<DragEventListener>();
			if (listener == null)
			{
				listener = view.avalableList[i].root.gameObject.AddComponent<DragEventListener>();
				SpecilistView speView = view.avalableList [i];
				//GameObject go = view.avalableList [i].root.gameObject;
				RegisterAsAvailable(speView);
			}
		}

		foreach(BasePropertyLineView vv in view.baseLines){
			BasePropertyLineView nowV = vv;
			int idx = view.baseLines.IndexOf (nowV);
			{
				ClickEventListerner listener = vv.AddButton.gameObject.GetComponent<ClickEventListerner>();
				if (listener == null)
				{
					listener = vv.AddButton.gameObject.AddComponent<ClickEventListerner>();
					listener.OnClickEvent += delegate (PointerEventData eventData) {
						if(model.LeftPoint<=0){
							return;
						}
						model.LeftPoint--;
						model.extra[idx]++;
						view.PointLeft.text = model.LeftPoint+"";
						view.baseLines[idx].ExtraValue.text = model.extra[idx] + "";
					};
				}
			}
			{
				ClickEventListerner listener = vv.MinusButton.gameObject.GetComponent<ClickEventListerner>();
				if (listener == null)
				{
					listener = vv.MinusButton.gameObject.AddComponent<ClickEventListerner>();
					listener.OnClickEvent += delegate (PointerEventData eventData) {
						if(model.extra[idx]<=0){
							return;
						}
						model.LeftPoint++;
						model.extra[idx]--;
						view.PointLeft.text = model.LeftPoint+"";
						view.baseLines[idx].ExtraValue.text = model.extra[idx] + "";
					};
				}
			}
		}




	}

	public void RegisterAsAvailable(SpecilistView vv){
		DragEventListener listener = vv.root.GetComponent<DragEventListener> ();
		if (listener == null) {
			Debug.Log ("error");
			return;
		}
		listener.ClearDragEvent();
		listener.ClearClickEvent ();

		listener.OnClickEvent += delegate(PointerEventData eventData) {
			ShowDetail(vv);
		};

		listener.OnBeginDragEvent += delegate (PointerEventData eventData) {
			vv.root.SetParent(root,true);
		};
		listener.OnDragEvent += delegate (PointerEventData eventData) {
			vv.root.position = mUIMgr.GetWorldPosition(eventData.position);
        };
		listener.OnEndDragEvent += delegate (PointerEventData eventData) {

			List<RaycastResult> results = new List<RaycastResult> ();
			bool used = false;
			EventSystem.current.RaycastAll (eventData,results);
			foreach(RaycastResult ret in results){
				if(ret.gameObject != null){
					if(ret.gameObject == view.PosSkillContainer.gameObject)
					{
						vv.root.SetParent(view.PosSkillContainer,false);
						used = true;
						RegisterAsChoosed(vv);
						model.selectedOne.Add(view.avalableList.IndexOf (vv));
					}
				}
			}
			if(!used){
				vv.root.SetParent(view.AvailableContainer,false);
			}
		};
	}

	public void RegisterAsChoosed(SpecilistView vv){
		DragEventListener listener = vv.root.GetComponent<DragEventListener> ();
		if (listener == null) {
			Debug.Log ("error");
			return;
		}
		listener.ClearClickEvent ();
		listener.ClearDragEvent ();
		listener.OnClickEvent += delegate(PointerEventData eventData) {
			vv.root.SetParent(view.AvailableContainer,false);
			RegisterAsAvailable(vv);
			vv.selected = false;
			model.selectedOne.Remove(view.avalableList.IndexOf (vv));
		};

	}

	public void OrderAllSpe(){
		foreach (SpecilistView vv in view.avalableList) {
			if (!vv.selected) {
				//vv.root.SetSiblingIndex ();
			}
		}
	}

	public void ShowDetail(SpecilistView vv){
		int idx = view.avalableList.IndexOf (vv);
		if (idx != -1) {
			
			Tezhi tezhi = model.availabelTezhi [idx];
			ShowDetail (tezhi);
		}
	}
	public void ShowDetail(Tezhi tezhi){
		view.DetailDesp.text = tezhi.Name;
		view.DetailName.text = tezhi.Score+"";
	}
}

