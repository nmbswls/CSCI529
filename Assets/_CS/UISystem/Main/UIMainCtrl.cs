using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MainModel : BaseModel
{

}

public class MainView : BaseView
{
	public Text text2Filed;

}


public class UIMainCtrl : UIBaseCtrl<MainModel, MainView>
{



	public override void Init(){
		view = new MainView ();
	}


	public override void BindView(){
	}

	public override void RegisterEvent(){
		
	}
}

