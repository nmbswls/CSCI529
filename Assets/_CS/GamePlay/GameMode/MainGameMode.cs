using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TurnMsg
{
    public string content;

    public TurnMsg(string content)
    {
        this.content = content;
    }
}
public class MainGameMode : GameModeBase
{

	IUIMgr pUIMgr;
    IRoleModule rm;
    ISpeEventMgr pEventMgr;

    UIMainCtrl mainUI;

    Queue<SpecialEvent> UnHandledEvent = new Queue<SpecialEvent>();

    List<string> TurnInfos = new List<string>();

    public int TurnPracticeNum = 0;

    

    public int GetPracticeCost()
    {
        return TurnPracticeNum * 5;
    }

    public override void Tick(float dTime){


    }

	public override void Init(){
		pUIMgr = GameMain.GetInstance ().GetModule<UIMgr> ();
        rm = GameMain.GetInstance().GetModule<RoleModule>();
        pEventMgr = GameMain.GetInstance().GetModule<SpeEventMgr>();
        
        mainUI = pUIMgr.ShowPanel("UIMain") as UIMainCtrl;
    }

    public void NextTurn()
    {
        TurnInfos.Clear();
        rm.NextTurn();
        HandleEvents(pEventMgr.CheckEvent());
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckOverdue();
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckTurnBonux();
        GameMain.GetInstance().GetModule<WeiboModule>().resetShua();
        GameMain.GetInstance().GetModule<WeiboModule>().enableRealRandom();
        mainUI.ShowMsg(new List<TurnMsg>());
        TurnPracticeNum = 0;
    }

    public void HandleEvents(List<SpecialEvent> list)
    {
        UnHandledEvent.Clear();
        foreach (SpecialEvent e in list)
        {
            UnHandledEvent.Enqueue(e);

        }
        HandleNextEvent();
    }






    public void HandleNextEvent()
    {
        if (UnHandledEvent.Count > 0)
        {

            SpecialEvent head = UnHandledEvent.Dequeue();
            pEventMgr.RemoveListener(head.EventId);
            foreach (string action in head.actions)
            {

                string[] cmd = action.Split(',');
                if (cmd[0] == "dialog")
                {
                    DialogManager dm = pUIMgr.ShowPanel("DialogManager") as DialogManager;
                    if (dm == null)
                    {
                        Debug.LogError("dialog mgr load fail");
                    }
                    dm.StartDialog(cmd[1], delegate (string[] args)
                    {
                        HandleNextEvent();
                    });
                }else if (cmd[0] == "event")
                {
                    pEventMgr.AddListener(cmd[1]);
                }

            }

        }

    }


}

