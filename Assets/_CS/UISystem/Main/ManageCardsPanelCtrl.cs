using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class ManageCardsModel : BaseModel
{

}

public class ManageCardsView : BaseView
{
	public TabGroup tabGroup;
	public Dropdown filter01;

	public Transform CardsContainer;
}

public class CardsTabView : TabGroupChildView{

	public Text Title;
	public Image BG;
	public override void BindView ()
	{
		base.BindView ();
		Title = root.GetChild (0).GetComponent<Text>();
		BG = root.GetComponent<Image> ();
	}
}

public class ManageCardsPanelCtrl : UIBaseCtrl<ManageCardsModel, ManageCardsView>
{

	public override void Init(){
		view = new ManageCardsView ();
		model = new ManageCardsModel ();
	}

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}

		view.tabGroup = root.GetChild(0).GetComponent<TabGroup>();
		view.CardsContainer = root.GetChild (1);
		view.filter01 = root.GetChild (3).GetChild (0).GetComponent<Dropdown> ();

	}

	public override void RegisterEvent(){
		view.tabGroup.InitTab (typeof(CardsTabView));
		view.tabGroup.OnValueChangeEvent += SwitchChoose;
		view.tabGroup.switchTab (0);

		view.filter01.onValueChanged.AddListener (delegate(int arg0) {
			Debug.Log(arg0);	
		});

	}

	public void SwitchChoose(int newTab){
		
		for (int i = 0; i < view.tabGroup.tabs.Count; i++) {
			CardsTabView childView = view.tabGroup.tabs[i] as CardsTabView;
			childView.BG.color = Color.white;
		}
		{
			CardsTabView childView = view.tabGroup.tabs [newTab] as CardsTabView;
			childView.BG.color = Color.red;
		}
	}
}


