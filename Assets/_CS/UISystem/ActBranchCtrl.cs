using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ActBranchModel : BaseModel
{
    public float TimeLeft;
}

public class ActBranchView : BaseView
{
    public Text NameText;
    public Text DespText;
    public Transform ChoiceContrainer;

    //public Image TimeLeft;
    public Text TimeLeft;

    public List<ActBranchChoiceView> choices = new List<ActBranchChoiceView>();
}

public class ActBranchChoiceView
{
    public RectTransform root;
    public Text ChoiceText;


    public void BindEvent(Transform root)
    {
        this.root = (RectTransform)root;
        ChoiceText = root.Find("Text").GetComponent<Text>();

    }

}

public delegate void OnActBranchDlg(int idx);

public class ActBranchCtrl : UIBaseCtrl<ActBranchModel, ActBranchView>
{

    public event OnActBranchDlg ActBranchEvent;

    public override void Init()
    {

    }

    public override void Tick(float dTime)
    {
        base.Tick(dTime);
        //if(model.TimeLeft != -1)
        //{
        //    model.TimeLeft -= dTime;
        //    if (model.TimeLeft <= 0)
        //    {
        //        FinishChoose(view.choices[0]);
        //    }
        //    view.TimeLeft.fillAmount = model.TimeLeft / 15f;
        //}
        if (model.TimeLeft != -1)
        {
            model.TimeLeft -= dTime;
            if (model.TimeLeft <= 0)
            {
                FinishChoose(view.choices[0]);
            }
            view.TimeLeft.text = model.TimeLeft.ToString("f1");
            //view.TimeLeft.fillAmount = model.TimeLeft / 15f;
            
        }
    }
    public override void BindView()
    {
        view.ChoiceContrainer = root.Find("Branches");
        view.NameText = root.Find("Name").GetComponent<Text>();
        view.DespText = root.Find("Desp").GetComponent<Text>();

        //view.TimeLeft = root.Find("Timer").Find("TimeLeft").GetComponent<Image>();
        view.TimeLeft = root.Find("Timer").Find("TimeLeft").GetComponent<Text>();
        Debug.Log(root.Find("Timer").Find("TimeLeft").GetComponent<Text>());

        foreach (Transform child in view.ChoiceContrainer)
        {
            ActBranchChoiceView vv = new ActBranchChoiceView();
            vv.BindEvent(child);
            view.choices.Add(vv);
        }

    }

    public override void PostInit()
    {
        //view.TimeLeft.fillAmount = 1;
        view.TimeLeft.text = "15";
    }

    public void SetEmergency(EmergencyAsset ea)
    {
        view.NameText.text = ea.EmName;
        view.DespText.text = ea.EmDesp;

        model.TimeLeft = 15.0f;

        for (int i=0;i<view.choices.Count;i++)
        {
            view.choices[i].ChoiceText.text = ea.Choices[i].Content;
        }
        AdjustBranches();
    }

    private void AdjustBranches()
    {

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
