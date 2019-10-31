using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class HomeMenuModel:BaseModel{
}

public class HomeMenuView:BaseView{
	public Button NewGame;
}

public class HomeMenuCtrl : UIBaseCtrl<HomeMenuModel,HomeMenuView>
{

	public override void Init(){
		model = new HomeMenuModel ();
		view = new HomeMenuView ();

		mUIMgr = GameMain.GetInstance ().GetModule<UIMgr> ();

	}

	// Use this for initialization
	public override void BindView(){
		view.NewGame = root.GetChild (0).GetComponent<Button> ();
	}

	public override void RegisterEvent(){
		view.NewGame.onClick.AddListener (delegate() {
			mUIMgr.CloseCertainPanel(this);
			mUIMgr.ShowPanel("StartNewGame");
		});
	}
}

