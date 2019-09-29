using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MainModel : BaseModel
{

}

public class MainView : BaseView
{
	public Button NextStage;

}


public class UIMainCtrl : UIBaseCtrl<MainModel, MainView>
{

	IRoleModule rm;


	public override void Init(){
		view = new MainView ();
		model = new MainModel ();

		rm = GameMain.GetInstance ().GetModule<RoleModule> ();
	}


	public override void BindView(){
		//view.NextStage = root.
	}

	public override void RegisterEvent(){
		
	}

	public void NextStage(){
		
	}


}

