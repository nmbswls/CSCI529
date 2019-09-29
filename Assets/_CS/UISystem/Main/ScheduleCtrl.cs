using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ScheduleModel : BaseModel
{
	public List<string> schedule = new List<string>();
}

public class ScheduleView : BaseView
{
	public Transform ScheduledContainer;

	public Transform ChoicesContainer;

	public Button BtnClose;
}


public class ScheduleCtrl : UIBaseCtrl<ScheduleModel, ScheduleView>
{

	public override void Init(){
		view = new ScheduleView ();
		model = new ScheduleModel ();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
    }

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}

        view.BtnClose = root.Find("Close").GetComponent<Button>();
    }

    public override void RegisterEvent()
    {
        view.BtnClose.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });
    }
}

