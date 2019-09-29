using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ScheduleModel : BaseModel
{
	public List<string> schedule;
}

public class ScheduleView : BaseView
{
	public Transform ScheduledContainer;

	public Transform ChoicesContainer;

	public Button BtnConfirm;
}


public class ScheduleCtrl : UIBaseCtrl<ScheduleModel, ScheduleView>
{

	public override void Init(){
		view = new ScheduleView ();
		model = new ScheduleModel ();
	}

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}



	}
}

