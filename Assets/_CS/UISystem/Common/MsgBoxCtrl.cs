using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GottonModel : BaseModel
{

}

public class GottonView : BaseView
{
    public Button ConfirmBtn;
    public Text Content;

}


public class MsgBoxCtrl : UIBaseCtrl<GottonModel, GottonView>
{

    public override void BindView()
    {


        view.ConfirmBtn = root.Find("ConfirmBtn").GetComponent<Button>();
        view.Content = root.Find("Content").GetComponent<Text>();
    }

    public override void RegisterEvent()
    {
        //view.ConfirmBtn
        view.ConfirmBtn.onClick.AddListener(delegate
        {
            mUIMgr.CloseCertainPanel(this);
        });
    }

    public override void PostInit()
    {
        base.PostInit();
    }

    public void ShowMsg(string content)
    {
        view.Content.text = content;
    }

}
