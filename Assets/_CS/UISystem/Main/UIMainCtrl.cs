using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MainModel : BaseModel
{

}

public class MainView : BaseView
{
	public Button NextStage;
    public Button ScheduleBtn;
    public Button InspectBtn;
}


public class UIMainCtrl : UIBaseCtrl<MainModel, MainView>
{

	IRoleModule rm;


	public override void Init(){
		view = new MainView ();
		model = new MainModel ();

		rm = GameMain.GetInstance ().GetModule<RoleModule> ();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
    }


	public override void BindView(){
        //view.NextStage = root.
        view.InspectBtn = root.Find("Deck").GetComponent<Button>();
        view.ScheduleBtn = root.Find("Schedule").GetComponent<Button>();
        view.NextStage = root.Find("NextTurn").GetComponent<Button>();
    }

	public override void RegisterEvent(){
        view.NextStage.onClick.AddListener(delegate ()
        {
            ICoreManager cm = GameMain.GetInstance().GetModule<CoreManager>();
            cm.ChangeScene();
        });

        view.ScheduleBtn.onClick.AddListener(delegate ()
        {
            mUIMgr.ShowPanel("ScheduleCtrl");
        });
        view.InspectBtn.onClick.AddListener(delegate ()
        {
            mUIMgr.ShowPanel("CardsMgr");
        });
    }

	public void NextStage(){
		
	}


}

