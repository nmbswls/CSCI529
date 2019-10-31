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
    public Transform root;
	public Image icon;
	public void BindView(Transform root){
        this.root = root;
        this.icon = root.GetComponent<Image> ();
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
    IResLoader pResLoader;

	public override void Init(){
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();

    }

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}
		view.DetailName = root.GetChild(2).GetChild(0).GetComponent<Text>();
		view.properies = root.GetChild (2).GetChild (1).GetComponent<RadarPropertyUI> ();

		view.InitMoney = root.Find("Detail_left").Find("Ownings").Find("Text_p0_value").GetComponent<Text> ();
		view.InitAttr = root.Find("Detail_left").Find("Ownings").Find("Text_p1_value").GetComponent<Text> ();
		view.InitSkill = root.Find("Detail_left").Find("Ownings").Find("Text_p2_value").GetComponent<Text> ();

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

	}

	public override void PostInit(){
		{
			for (int i = 0; i < 5; i++) {
				RoleStoryAsset ret = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<RoleStoryAsset> ("Roles/role"+i);
				if (ret != null) {
					view.roleList[i].name.text = ret.Name;
					view.roleList[i].desp.text = ret.Desp;
				}
			}
			view.properies.Setup ();
			switchSelectedStory (0);

		}
	}

	public override void RegisterEvent(){
		{
			DragEventListener listener = view.StartGame.gameObject.GetComponent<DragEventListener>();
			if (listener == null)
			{
				listener = view.StartGame.gameObject.AddComponent<DragEventListener>();
				listener.OnClickEvent += delegate (PointerEventData eventData) {
					mUIMgr.CloseCertainPanel(this);
					AdjustInitCtrl ctrl = mUIMgr.ShowPanel("AdjustPanel") as AdjustInitCtrl;
                    ctrl.SetRoleId(nowIdx);

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

		RoleStoryAsset ret = pResLoader.LoadResource<RoleStoryAsset> ("Roles/role"+idx);
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


            for(int i = 0; i < view.extraInfoList.Count; i++)
            {
                pResLoader.ReleaseGO("UI/Role/extra", view.extraInfoList[i].root.gameObject);
            }
            view.extraInfoList.Clear();

            for(int i = 0; i < ret.initOwning.Count; i++)
            {
                ExtraInfoView vv = new ExtraInfoView();
                GameObject go = pResLoader.Instantiate("UI/Role/extra",view.extraContainer);
                vv.BindView(go.transform);
                view.extraInfoList.Add(vv);
            }
        }
	}

	public override void Release(){
		base.Release ();
		GameMain.GetInstance ().GetModule<ResLoader> ().UnloadAsset ("Roles/role0");
	}
}

