using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ActBranchModel : BaseModel
{

}

public class ActBranchView : BaseView
{
    public Text DespText;
    public Transform ChoiceContrainer;

    public List<ActBranchChoiceView> choices = new List<ActBranchChoiceView>();
}

public class ActBranchChoiceView
{
    public RectTransform root;
    public void BindEvent(Transform root)
    {
        this.root = (RectTransform)root;

    }

}

public delegate void OnActBranchDlg(int idx);

public class ActBranchCtrl : UIBaseCtrl<ActBranchModel, ActBranchView>
{

    public event OnActBranchDlg ActBranchEvent;

    public override void Init()
    {
        view = new ActBranchView();
        model = new ActBranchModel();

        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
    }

    public override void BindView()
    {
        view.ChoiceContrainer = root.Find("Branches");
        foreach(Transform child in view.ChoiceContrainer)
        {
            ActBranchChoiceView vv = new ActBranchChoiceView();
            vv.BindEvent(child);
            view.choices.Add(vv);
        }
    }

    public override void RegisterEvent()
    {
        foreach(ActBranchChoiceView vv in view.choices)
        {
            ClickEventListerner listener = vv.root.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = vv.root.gameObject.AddComponent<ClickEventListerner>();
                listener.OnClickEvent += delegate(PointerEventData eventData) {
                    FinishChoose(vv);
                };
            }
        }
    }

    public override void PostInit()
    {

    }

    private void AdjustChoices()
    {


    }

    public override void Release()
    {
        base.Release();
    }

    public void FinishChoose(ActBranchChoiceView vv)
    {
        if(ActBranchEvent != null)
        {
            ActBranchEvent(view.choices.IndexOf(vv));
            ActBranchEvent = null;
        }
        mUIMgr.CloseCertainPanel(this);
    }


}
