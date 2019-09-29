using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ChooseStoryLineModel : BaseModel
{
	public List<string> sss = new List<string>();
}

public class ChooseStoryLineView : BaseView
{

	public Text DetailName;
	public RadarPropertyUI properies;

	public Text InitMoney;
	public Text InitAttr;
	public Text InitSkill;

	public Text DetailDesp;

	public Transform extraContainer;
	public List<ExtraInfoView> extraInfoList = new List<ExtraInfoView>();

	public Transform roleContainer;
	public List<RoleItemView> roleList = new List<RoleItemView>();
	public Image StartGame;

}
public class ExtraInfoView{
	public Image icon;
	public void BindView(Transform root){
		this.icon = root.GetChild (0).GetComponent<Image> ();
	}
}

public class RoleItemView{
	public Transform root;

	public Image chooseHint;
	public Image clickArea;
	public Text name;
	public Text desp;
	public Image touxiang;

	public void BindView(Transform root){
		this.root = root;
		this.chooseHint = root.GetChild (0).GetComponent<Image> ();
		this.clickArea = root.GetChild (1).GetComponent<Image> ();
		this.name = root.GetChild (2).GetComponent<Text>();
		this.desp = root.GetChild (3).GetComponent<Text>();
		this.touxiang = root.GetChild(4).GetComponent<Image>();
	}
	public void RegisterEvent(){
		
	}
}

public class ChooseStoryLineCtrl : UIBaseCtrl<ChooseStoryLineModel, ChooseStoryLineView>
{

	int nowIdx = -1;

	public override void Init(){
		view = new ChooseStoryLineView ();
		model = new ChooseStoryLineModel ();
	}

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}
		view.DetailName = root.GetChild(2).GetChild(0).GetComponent<Text>();
		view.properies = root.GetChild (2).GetChild (1).GetComponent<RadarPropertyUI> ();
		view.InitMoney = root.GetChild(2).GetChild(3).Find("Text_p0_value").GetComponent<Text> ();
		view.InitAttr = root.GetChild(2).GetChild(3).Find("Text_p1_value").GetComponent<Text> ();
		view.InitSkill = root.GetChild(2).GetChild(3).Find("Text_p2_value").GetComponent<Text> ();

		view.DetailDesp = root.GetChild (3).GetChild (2).GetComponent<Text>();
		view.extraContainer = root.GetChild (3).GetChild (3);

		view.roleContainer = root.GetChild (1).GetChild(0).GetChild(0).Find ("Content");
		view.StartGame = root.GetChild (4).GetComponent<Image> ();

		for (int i = 0; i < 5; i++) {
			GameObject g = GameMain.GetInstance ().GetModule<ResLoader> ().Instantiate ("UI/Script1",view.roleContainer);
			RoleItemView v = new RoleItemView ();
			v.BindView (g.transform);
			view.roleList.Add (v);

		}


		//init 
		{
			for (int i = 0; i < 5; i++) {
				RoleStoryAsset ret = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<RoleStoryAsset> ("role"+i);
				if (ret != null) {
					view.roleList[i].name.text = ret.Name;
					view.roleList[i].desp.text = ret.Desp;
				}
			}


			view.properies.Setup ();
			switchSelectedStory (0);

		}





		Debug.Log ("");
	}

	public override void RegisterEvent(){
		{
			DragEventListener listener = view.StartGame.gameObject.GetComponent<DragEventListener>();
			if (listener == null)
			{
				listener = view.StartGame.gameObject.AddComponent<DragEventListener>();
				listener.OnClickEvent += delegate (PointerEventData eventData) {
					IUIMgr m = GameMain.GetInstance().GetModule<UIMgr>();
					m.CloseFirstPanel();
					m.ShowPanel("UIMain");
				};
			}
		}

		foreach (RoleItemView vv in view.roleList) {
			ClickEventListerner listener = vv.clickArea.gameObject.GetComponent<ClickEventListerner>();
			if (listener == null)
			{
				listener = vv.clickArea.gameObject.AddComponent<ClickEventListerner>();
				listener.OnClickEvent += delegate (PointerEventData eventData) {
					switchSelectedStory(view.roleList.IndexOf(vv));
				};
			}
		}
	}

	public void switchSelectedStory(int idx){
		if (nowIdx == idx) {
			return;
		}
		foreach (RoleItemView roleView in view.roleList) {
			roleView.chooseHint.enabled = false;
		}
		view.roleList [idx].chooseHint.enabled = true;
		//view.properies.SetPointValues (new int[]{Random.Range(10,20),Random.Range(10,20),Random.Range(10,20),Random.Range(10,20),Random.Range(10,20)}); 
		nowIdx = idx;

		RoleStoryAsset ret = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<RoleStoryAsset> ("role"+idx);
		if (ret != null) {
			view.DetailName.text = ret.Name;
			view.DetailDesp.text = "";
			foreach (string ss in ret.specialList) {
				view.DetailDesp.text += ss + "\n";
			}
			view.InitMoney.text = ret.initMoney + "";
			view.InitAttr.text = ret.initFreePoint + "";
			view.InitSkill.text = ret.initSkillPoint + "";

			view.properies.SetPointValues (ret.initProperties);

		}
	}

	public override void Release(){
		base.Release ();
		GameMain.GetInstance ().GetModule<ResLoader> ().UnloadAsset ("role0");
	}
}

