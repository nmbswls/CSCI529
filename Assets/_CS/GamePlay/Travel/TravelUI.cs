using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TravelView: BaseView
{
    public Text TurnLeft;
    public Transform RightBottomPanel;

    public Text PotName;
    public Text PotDesp;
    public List<Transform> PotReactList = new List<Transform>();

    public int MaxPotReactNum = 4;
}

public class TravelModel:BaseModel
{

}

public class TravelUI : UIBaseCtrl<TravelModel, TravelView>
{

    IResLoader mResLoader;
    public TravelGameMode gameMode;

    public override void Init()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as TravelGameMode;
    }

    public override void BindView()
    {
        view.TurnLeft = root.Find("LeftButtom").GetComponentInChildren<Text>();
        view.RightBottomPanel = root.Find("RightBottom");

        view.PotName = view.RightBottomPanel.Find("PotInfo").Find("Name").GetComponent<Text>();
        view.PotDesp = view.RightBottomPanel.Find("PotInfo").Find("Desp").GetComponent<Text>();

        Transform pots = view.RightBottomPanel.Find("Reacts");
        foreach (Transform child in pots)
        {
            view.PotReactList.Add(child);
        }

        view.RightBottomPanel.Find("Reacts");
    }

    public override void RegisterEvent()
    {

        foreach(Transform btn in view.PotReactList)
        {
            ClickEventListerner listener = btn.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = btn.gameObject.AddComponent<DragEventListener>();
            }
            listener.OnClickEvent += delegate {

                ChooseAct(0);
            };

        }
    }

    public void ChooseAct(int idx)
    {

    }

    public override void PostInit()
    {

    }

    public void ChangeDetail(TravelPot pot)
    {
        view.PotName.text = pot.potInfo.Name;
        view.PotDesp.text = pot.potInfo.Desp;

        for(int i = 0; i < pot.potInfo.Opts.Count; i++)
        {
            view.PotReactList[i].gameObject.SetActive(true);
        }
        for (int i = pot.potInfo.Opts.Count; i < view.MaxPotReactNum; i++)
        {
            view.PotReactList[i].gameObject.SetActive(false);
        }

    }

}
