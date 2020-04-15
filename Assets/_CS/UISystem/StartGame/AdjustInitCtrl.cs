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

	public int[] bas = new int[]{1,2,3,10,10};
	public int[] extra;
}

public class AdjustInitView : BaseView{

	public InputField NetName;

	public List<BasePropertyLineView> baseLines = new List<BasePropertyLineView> ();
    public List<SpecialityView> specLines = new List<SpecialityView>();

	public Text PointLeft;
	public Text SKillLeft;

	public Transform PosSkillContainer;
	public List<SpecilistView> choosedList = new List<SpecilistView> ();

	public Transform AvailableContainer;
	public List<SpecilistView> avalableList = new List<SpecilistView> ();

	public Text DetailName;
	public Text DetailDesp;


	public Button NextStage;

    public Toggle ToggleMan;
    public Toggle ToggleWoman;

    public Text WarningMsg;





    //新UI
    public Transform Sections;

    public Button Next_Button;
    public Button Back_Button;

    public Transform Layout;

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
        //Debug.Log(AddButton.name);
		MinusButton = root.GetChild (4).GetComponent<Image>();
        //Debug.Log(MinusButton.name);
    }

}

public class SpecialityView
{
    public Transform root;
    public Toggle tog;
    public int idx = 0;

    public SpecialityView(int idx)
    {
        this.idx = idx;
    }
    
    public void BindView(Transform root)
    {
        this.root = (Transform)root;
        this.tog = root.GetComponent<Toggle>();
    }

}
public class AdjustInitCtrl : UIBaseCtrl<AdjustInitModel,AdjustInitView>
{

    IRoleModule pRoleMgr;

    IResLoader pResMgr;

    int cur_section = 1;

    int cur_speciatly = -1;

    public static int RELOCATE_LIMIT = 6;

    public void SetRoleId(int idx)
    {
        model.roleId = idx + "";
		model.bas = pRoleMgr.LoadStates(model.roleId);
        loadBas();//更新选择后的base
	}


    public override void Init(){

        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        pResMgr = GameMain.GetInstance().GetModule<ResLoader>();

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

        if (root == null)
        {
            Debug.Log("bind fail no root found");
        }

        view.Sections = root.Find("Sections");

        view.Next_Button = view.Sections.Find("Next").GetComponent<Button>();
        view.Back_Button = view.Sections.Find("Back").GetComponent<Button>();

        view.ToggleMan = view.Sections.Find("Section1").Find("基本信息").Find("Toggle_Man").GetComponent<Toggle>();
        view.ToggleWoman = view.Sections.Find("Section1").Find("基本信息").Find("Toggle_Woman").GetComponent<Toggle>();


        view.Layout = view.Sections.Find("Section2").Find("Layout");
        
        foreach(Transform child in view.Layout)
        {
            SpecialityView vv = new SpecialityView(child.GetSiblingIndex());
            view.specLines.Add(vv);
            vv.BindView(child);
        }


        view.PointLeft = view.Sections.Find("Section3").Find ("属性调整").GetChild(0).GetChild(0).GetComponent<Text> ();
        view.PointLeft.text = model.LeftPoint + "";

        Transform pRoot = view.Sections.Find("Section3").Find("属性调整").GetChild(1);
        view.WarningMsg = view.Sections.Find("Section3").Find("属性调整").Find("提示").GetComponent<Text>();



        view.NextStage = root.Find ("NextStage").GetComponent<Button>();

		view.SKillLeft = root.Find ("Text_Left").GetComponent<Text> ();
		//view.PointLeft = root.Find ("属性调整").GetChild(0).GetChild(1).GetComponent<Text> ();

		view.SKillLeft.text = model.LeftSkillPoint + "";
		
		view.PosSkillContainer = root.Find ("已选优势");
		view.AvailableContainer = root.Find ("可选特质").GetChild(1).GetChild(0).GetChild(0);

		view.DetailName = root.Find ("SpeDetail").GetChild (0).GetComponent<Text>();
		view.DetailDesp = root.Find ("SpeDetail").GetChild (1).GetComponent<Text>();

        //Transform pRoot = root.Find ("属性调整").GetChild(1);

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

    public void loadBas()
    {
        for(int i = 0; i<5; i++)
        {
            view.baseLines[i].BaseValue.text = model.bas[i] + model.extra[i] + ""; 
        }
        view.PointLeft.text = model.LeftPoint + "";
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
		pRoleMgr.AllocateStats(model.extra);

	}




    public override void RegisterEvent(){

		view.NextStage.onClick.AddListener (delegate() {
			mUIMgr.CloseCertainPanel(this);
            SetupPlayerInfo();

            MainGMInitData data = new MainGMInitData();
            data.isNextTurn = true;
            GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main", data);

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
                //Debug.Log(idx + ":" + nowV.root.name);
				ClickEventListerner listener = vv.AddButton.gameObject.GetComponent<ClickEventListerner>();
				if (listener == null)
				{
					listener = vv.AddButton.gameObject.AddComponent<ClickEventListerner>();
                    
                    listener.OnClickEvent += delegate (PointerEventData eventData) {
						if(model.LeftPoint<=0){
							return;
						}
                        if (model.extra[idx] >= RELOCATE_LIMIT)
                        {
                            view.WarningMsg.gameObject.SetActive(true);
                            return;
                        }
                        else
                        {
                            view.WarningMsg.gameObject.SetActive(false);
                        }

                        Debug.Log("+++++");
                        model.LeftPoint--;
						model.extra[idx]++;
						view.PointLeft.text = model.LeftPoint+"";
						view.baseLines[idx].BaseValue.text = model.bas[idx] + model.extra[idx] + "";
					};
				}
			}
			{
				ClickEventListerner listener = vv.MinusButton.gameObject.GetComponent<ClickEventListerner>();
				if (listener == null)
				{
					listener = vv.MinusButton.gameObject.AddComponent<ClickEventListerner>();
					listener.OnClickEvent += delegate (PointerEventData eventData) {
                        if (model.extra[idx] > RELOCATE_LIMIT)
                        {
                            return;
                        }
                        if (model.extra[idx]<=0){
							return;
						}
                        {
                            view.WarningMsg.gameObject.SetActive(false);
                        }

                        Debug.Log("-----");
                        model.LeftPoint++;
						model.extra[idx]--;
						view.PointLeft.text = model.LeftPoint+"";
                        view.baseLines[idx].BaseValue.text = model.bas[idx] + model.extra[idx] + "";
                    };
				}
			}
		}

        view.Next_Button.onClick.AddListener(delegate () {
            if(cur_section==3)
            {
                mUIMgr.CloseCertainPanel(this);
                SetupPlayerInfo();

                MainGMInitData data = new MainGMInitData();
                data.isNextTurn = true;
                GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main", data);
            }
            else
            {
                ShowNextSection();
                if(cur_section == 2)
                {
                    view.Back_Button.gameObject.SetActive(true);
                }
                if (cur_section == 3)
                {
                    view.WarningMsg.gameObject.SetActive(false);
                    view.Next_Button.GetComponent<Image>().sprite = pResMgr.LoadResource<Sprite>("Adjust/AdjustButton/go_stage");
                }
            }
        });

        view.Back_Button.onClick.AddListener(delegate () {
            ShowLastSection();
            if (cur_section == 1)
            {
                view.Back_Button.gameObject.SetActive(false);
            }
            view.Next_Button.GetComponent<Image>().sprite = pResMgr.LoadResource<Sprite>("Adjust/AdjustButton/go_next");
        });

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

    public void ShowSection()
    {
        view.Sections.Find("Section" + cur_section).gameObject.SetActive(true);
    }

    public void CloseSection()
    {
        view.Sections.Find("Section" + cur_section).gameObject.SetActive(false);
    }

    public void ShowNextSection()
    {
        CloseSection();
        
        if (cur_section == 2)
        {
            for(int i = 0; i<5; i++)
            {
                if(view.specLines[i].tog.isOn)
                {
                    cur_speciatly = view.specLines[i].idx;
                }
                model.extra[i] = 0;
            }
            model.LeftPoint = 10;
            if(cur_speciatly!=-1)model.extra[cur_speciatly] += 10;

            loadBas();
        }
        cur_section++;
        ShowSection();
    }

    public void ShowLastSection()
    {
        CloseSection();
        cur_section--;
        if (cur_section == 2)
        {
            if (cur_speciatly != -1) model.extra[cur_speciatly] -= 10;
            loadBas();
        }
        ShowSection();
    }
}

