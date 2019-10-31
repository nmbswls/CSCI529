using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseRewardModel : BaseModel
{

}

public class ChooseRewardView : BaseView
{
    public Button ConfirmBtn;
    public Transform AwardContainer;

}

public class ChooseRewardCtrl : UIBaseCtrl<ChooseRewardModel, ChooseRewardView>
{


    public override void BindView()
    {

        view.ConfirmBtn = root.Find("ConfirmBtn").GetComponent<Button>();
        view.AwardContainer = root.Find("Awards");
    }

    public override void RegisterEvent()
    {
        //view.ConfirmBtn
        view.ConfirmBtn.onClick.AddListener(delegate {
            GetAward();
        });
    }

    public override void PostInit()
    {
    }

    public void SetContent()
    {
        //
    }

    public void GetAward()
    {
        mUIMgr.CloseCertainPanel(this);
    }

}
