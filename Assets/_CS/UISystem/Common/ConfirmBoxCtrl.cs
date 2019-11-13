using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ConfirmBoxView : BaseView
{
    public Button ConfirmBtn;
    public Button CancelBtn;
    public Text Content;
}


public class ConfirmBoxCtrl : UIBaseCtrl<BaseModel, ConfirmBoxView>
{
    public override void BindView()
    {

        view.ConfirmBtn = root.Find("ConfirmBtn").GetComponent<Button>();
        view.CancelBtn = root.Find("CancelBtn").GetComponent<Button>();
        view.Content = root.Find("Content").GetComponent<Text>();
    }

    private Action confirmCallback;

    public override void RegisterEvent()
    {
        //view.ConfirmBtn
        view.ConfirmBtn.onClick.RemoveAllListeners();
        view.ConfirmBtn.onClick.AddListener(delegate
        {
            if(confirmCallback != null)
            {
                confirmCallback();
                confirmCallback = null;
            }
            mUIMgr.CloseCertainPanel(this);
        });
        view.CancelBtn.onClick.RemoveAllListeners();
        view.CancelBtn.onClick.AddListener(delegate
        {
            mUIMgr.CloseCertainPanel(this);
        });
    }

    public override void PostInit()
    {
        base.PostInit();
    }

    public void ShowMsg(string content, Action confirmCallback)
    {
        view.Content.text = content;
        this.confirmCallback = confirmCallback;
    }
}
